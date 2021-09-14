using MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class ReservationController : Controller
    {
		private List<Reservation> reservations = new List<Reservation>();
		private List<Arrangement> arrangements = new List<Arrangement>();
		private List<Accommodation> accommodations = new List<Accommodation>();
		private List<AccommodationUnit> accommodationUnits = new List<AccommodationUnit>();
		private List<User> users = new List<User>();

		#region Akcije
		//Akcija koja otvara prozor za ispis svih rezervacija u okviru menadzerovih aranzmana
		public ActionResult OpenManagerReservationsListPage()
		{
			//Ocitavamo sve rezervacije i aranzmane
			LoadReservations();
			LoadArrangements();

			//Vadimo korisnika iz sesija i pravimo praznu listu rezervacija
			User user = (User)System.Web.HttpContext.Current.Application["user"];
			List<Reservation> output = new List<Reservation>();

			//Prolazimo kroz sve rezervacije
			foreach (var reservation in reservations)
			{
				//Proveravamo da li u menadzerovoj listi kreiranih aranzmana sadrzi id aranzmana date rezervacije
				if (user.CreatedArrangemetsID.Contains(reservation.ArrangementID))
				{
					//Ako sadrzi dodaj u ispisnu listu
					output.Add(reservation);
				}
			}

			//Listu rezervacija i aranzmana ubacujemo u sesiju
			System.Web.HttpContext.Current.Application["reservations"] = output;
			System.Web.HttpContext.Current.Application["arrangements"] = arrangements;

			return View("ManagerReservationList");
		}

		//Akcija za otvaranje prozora za rezervaciju
		public ActionResult OpenReservationPage(int id)
		{
			//Iz baze ucitavamo sve smestaje, smestajne jedinice i aranzmane
			LoadAccommodations();
			LoadAccommodationUnits();
			LoadArrangements();

			//Trazimo aranzman iz baze na osnovu idija
			Arrangement arrangement = GetArrangement(id);
			
			//Trazimo smestaj iz baze na osnovu idija
			Accommodation accommodation = GetAccommodation(arrangement.AccommodationID);

			//Pravimo praznu listu smestajnih jedinica
			List<AccommodationUnit> accommodationUnits = new List<AccommodationUnit>();

			//Prolazimo kroz svaku smestajnu jedinicu u okviru smestaja
			foreach (var unit in accommodation.AccommodationUnitID)
			{
				//I dodajemo objekat smestajne jedinice u listu
				accommodationUnits.Add(GetAccommodationUnit(unit));
			}
			
			//Listu smestajnih jedinica i aranzman ubacujemo u sesiju
			System.Web.HttpContext.Current.Application["accommodationUnits"] = accommodationUnits;
			System.Web.HttpContext.Current.Application["arrangement"] = arrangement;

			return View("Reserve");
		}

		//Akcija koja vrsi rezervaciju nekog aranzmana
		public ActionResult Reserve(Reservation reservation)
		{
			//Vrsi se validacija unetih vrednosti
			bool validate = reservation.Validate();
			if (validate == false)
			{
				ViewBag.Message = "Input fields can't be empty!";
				return View("Reserve");
			}

			//Iz baze ocitavamo sve rezervacije i smestajne jedinice
			LoadReservations();
			LoadAccommodationUnits();

			//Iz sesije izvlacimo korisnika i aranzman
			User user = (User)System.Web.HttpContext.Current.Application["user"];
			Arrangement arrangement = (Arrangement)System.Web.HttpContext.Current.Application["arrangement"];

			//Generisemo id za rezervaciju
			reservation.GenerateID();

			//Postavljamo id aranzmana rezervaciji
			reservation.ArrangementID = arrangement.ID;

			//Postavljamo korisnicko ime rezervaciji
			reservation.TouristUsername = user.Username;

			//Postavljamo status rezervacije da bude aktivna
			reservation.ReservationStatus = ReservationStatus.Active;

			//Izvlacimo iz baze smestajnu jedinicu
			AccommodationUnit accommodationUnit = GetAccommodationUnit(reservation.AccommodationUnitID);

			//Smanjujemo joj broj soba
			accommodationUnit.NumberOfFreeRooms--;

			//Dodajemo rezervaciju u listu
			reservations.Add(reservation);

			//Sacuvamo izmene za rezervacije i smestajne jedinice
			SaveReservations();
			SaveAccommodationUnits();

			return View("../Home/Index");
		}

		//Akcija koja otvara listu turistovih rezervacija
		public ActionResult OpenTouristReservationsListPage()
		{
			//Iz baze ocitavamo sve rezervacije, aranzmane i smestajne jedinice
			LoadReservations();
			LoadArrangements();
			LoadAccommodationUnits();

			//Korisnika ocitavamo iz sesije
			User user = (User)System.Web.HttpContext.Current.Application["user"];

			//Pravimo praznu listu rezervacije
			List<Reservation> output = new List<Reservation>();

			//Prolazimo kroz sve rezervacije
			foreach (var reservation in reservations)
			{
				//Vrsimo proveru isteka rezervacije
				UpdateReservationStatus(reservation);

				//Ako korisnicko ime turiste u okviru rezervacije se poklapa sa turistom koji trenutno je ulogovan
				if (reservation.TouristUsername == user.Username)
				{
					//Dodaj u ispisnu listu
					output.Add(reservation);
				}
			}

			//Rezervacije i aranzmane ubaci u sesiju
			System.Web.HttpContext.Current.Application["arrangements"] = arrangements;
			System.Web.HttpContext.Current.Application["reservations"] = output;

			return View("UserReservationList");
		}

		//Akcija sluzi za pretragu rezervacija
		public ActionResult SearchReservations(ReservationSearchModel searchModel)
		{
			//Ocitavamo sve rezervacije i aranzmane iz baze
			LoadReservations();
			LoadArrangements();

			//Trazimo korisnika iz sesije
			User user = (User)System.Web.HttpContext.Current.Application["user"];

			//Pravimo praznu listu rezervacije
			List<Reservation> userReservations = new List<Reservation>();

			//Prolazimo kroz sve rezervacije
			foreach (var reservation in reservations)
			{
				//Gledamo da li korisnicko ime u rezervaciji odgovara korisnickom imenu trenutnu ulogovanog korisnika
				if (reservation.TouristUsername == user.Username)
				{
					//Ako odgovara dodaj ga u listu za pretragu
					userReservations.Add(reservation);
				}
			}

			//Pozivamo funkciju za pretragu
			List<Reservation> output = SearchReservationsPrivate(userReservations, searchModel);

			//Ubacujemo rezervacije u sesiju
			System.Web.HttpContext.Current.Application["reservations"] = output;

			return View("UserReservationList");
		}

		//Akcija koja vrsi otkazivanje rezervacije
		public ActionResult Cancel(string id)
		{
			//Iz baze ocitavamo sve rezervacije, smestajne jedinice i korisnike
			LoadReservations();
			LoadAccommodationUnits();
			LoadUsers();

			//Iz baze izvlacimo trazenu rezervaciju na osnovu idija
			Reservation reservation = GetReservation(id);

			//Proveravamo da li je aktivna
			if (reservation.ReservationStatus != ReservationStatus.Active)
			{
				//Ako jeste ispisujemo poruku greske
				ViewBag.Message = "Reservation needs to be active, so you can cancel it";
				return View("UserReservationList");
			}

			//Pozivamo funkcije za promenu statusa rezervacije, povecanje otkazanih putovanja korisnika i povecanje slobodnih soba smestajne jedinice
			ChangeReservationStatus(id, ReservationStatus.Canceled);
			UpdateCancelReservations(reservation.TouristUsername);
			IncrementAccommodaitonUnitRooms(reservation.AccommodationUnitID);

			//Ubacujemo rezervacije u sesiju
			System.Web.HttpContext.Current.Application["reservations"] = reservations;

			return View("UserReservationList");
		}
		#endregion

		#region Load funkcije
		//Ucitavamo podatke iz baze i upisujemo u listu
		private void LoadReservations()
		{
			reservations = XMLHelper.LoadReservations();
		}

		//Ucitavamo podatke iz baze i upisujemo u listu
		private void LoadArrangements()
		{
			arrangements = XMLHelper.LoadArrangements();
		}

		//Ucitavamo podatke iz baze i upisujemo u listu
		private void LoadAccommodations()
		{
			accommodations = XMLHelper.LoadAccommodations();
		}

		//Ucitavamo podatke iz baze i upisujemo u listu
		private void LoadAccommodationUnits()
		{
			accommodationUnits = XMLHelper.LoadAccommodationUnits();
		}

		//Ucitavamo podatke iz baze i upisujemo u listu
		private void LoadUsers()
		{
			users = XMLHelper.LoadUsers();
		}
		#endregion

		#region Get funkcije
		//Trazi se rezervacija iz liste na osnovu idija
		private Reservation GetReservation(string id)
		{
			//Prolazimo kroz sve reservacije
			foreach (var reservation in reservations)
			{
				//Ako je id rezervacije isti kao onaj u parametru
				if (reservation.ID == id)
				{
					//Nasli smo ga
					return reservation;
				}
			}

			//Ako ga nismo nasli, baci null
			return null;
		}

		//Trazi se korisnik iz liste na osnovu idija
		private User GetUser(string username)
		{
			//Prolazimo kroz sve korisnike
			foreach (var user in users)
			{
				//Ako je id korisnik isti kao onaj u parametru
				if (user.Username == username)
				{
					//Nasli smo ga
					return user;
				}
			}

			//Ako ga nismo nasli, baci null
			return null;
		}

		//Trazi se aranzman iz liste na osnovu idija
		private Arrangement GetArrangement(int id)
		{
			//Prolazimo kroz sve aranzmane
			foreach (var arrangement in arrangements)
			{
				//Ako je id arazmana isti kao onaj u parametru
				if (arrangement.ID == id)
				{
					//Nasli smo ga
					return arrangement;
				}
			}

			//Ako ga nismo nasli, baci null
			return null;
		}

		//Trazi se smestaj iz liste na osnovu idija
		private Accommodation GetAccommodation(int id)
		{
			//Prolazimo kroz sve smestaje
			foreach (var accommodation in accommodations)
			{
				//Ako je id smestaja isti kao onaj u parametru
				if (accommodation.ID == id)
				{
					//Nasli smo ga
					return accommodation;
				}
			}

			//Ako ga nismo nasli, baci null
			return null;
		}

		//Trazi se smestajna jedinica iz liste na osnovu idija
		private AccommodationUnit GetAccommodationUnit(int id)
		{
			//Prolazimo kroz sve smestajene jedinice
			foreach (var accommodationUnit in accommodationUnits)
			{
				//Ako je id smestajne jedinice isti kao onaj u parametru
				if (accommodationUnit.ID == id)
				{
					//Nasli smo ga
					return accommodationUnit;
				}
			}

			//Ako ga nismo nasli, baci null
			return null;
		}
		#endregion

		#region Save funkcije
		//Listu rezervacija upisujemo u bazu
		private void SaveReservations()
		{
			XMLHelper.SaveReservations(reservations);
		}

		//Listu korisnika upisujemo u bazu
		private void SaveUsers()
		{
			XMLHelper.SaveUsers(users);
		}

		//Listu smestajnih jedinica upisujemo u bazu
		private void SaveAccommodationUnits()
		{
			XMLHelper.SaveAccommodationUnits(accommodationUnits);
		}
		#endregion

		#region Pomocne funkcije
		//Funkcija koja utvrdjuje da li je rezervacija istekla
		private void UpdateReservationStatus(Reservation reservation)
		{
			//Izvlacimo aranzman iz rezervacije
			Arrangement arrangement = GetArrangement(reservation.ArrangementID);

			//Utvrdjujemo da li je istekao
			if (DateTime.Now > arrangement.EndDateOfArrangement && reservation.ReservationStatus != ReservationStatus.Expired)
			{
				//Ako jeste promeni status rezervacije i uvecan broj soba smestajne jedinice na koji se odnosi rezervacija
				ChangeReservationStatus(reservation.ID, ReservationStatus.Expired);
				IncrementAccommodaitonUnitRooms(reservation.AccommodationUnitID);
			}
		}

		//Funkcija koja menja status rezervacije
		private void ChangeReservationStatus(string id, ReservationStatus status)
		{
			Reservation reservation = GetReservation(id);

			//Promena statusa
			reservation.ReservationStatus = status;

			//Cuvanje izmena
			SaveReservations();
		}

		//Funkcija koja povecava kapacitet smestajnih jedinica
		private void IncrementAccommodaitonUnitRooms(int id)
		{
			AccommodationUnit unit = GetAccommodationUnit(id);

			//Povecava se kapacitet
			unit.NumberOfFreeRooms++;

			//Cuvaju se izmene
			SaveAccommodationUnits();
		}

		//Funkcija koja povecava broj otkazanih putovanja korisnika
		private void UpdateCancelReservations(string username)
		{
			User user = GetUser(username);

			//Povecava se kapacitet
			user.NumberOfCanceledTrips++;

			//Cuvaju se izmene
			SaveUsers();
		}

		//Metoda koja vrsi pretragu rezervacija
		private List<Reservation> SearchReservationsPrivate(List<Reservation> userReservations, ReservationSearchModel searchModel)
		{
			//Lista u kojoj cuvamo povratnu vrednost metode
			//Sve rezervacije koji zadovoljavaju pretragu
			List<Reservation> output = new List<Reservation>();	

			foreach (var reservation in userReservations)
			{
				//Provera za status rezervacije
				if (searchModel.ReservationStatus != null)
				{
					if (reservation.ReservationStatus != searchModel.ReservationStatus)
					{
						//Ako ne zadovoljava uslov, preskoci entitet
						continue;
					}
				}

				//Provera za naziv aranzmana
				if (searchModel.ArrangementName != null && searchModel.ArrangementName != "")
				{
					//Trazimo aranzman iz baze na osnovu idija koji se nalazi u rezervaciji
					Arrangement arrangement = GetArrangement(reservation.ArrangementID);

					//Izvlacimo indeks prvog slova unetog imena u pretrazi
					int index = arrangement.Name.ToLower().IndexOf(searchModel.ArrangementName.ToLower());
					if (index < 0)
					{
						//Ako ne zadovoljava uslov, preskoci entitet
						continue;
					}
				}

				output.Add(reservation);
			}

			return output;
		}
		#endregion
	}
}