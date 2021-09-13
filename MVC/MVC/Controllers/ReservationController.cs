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

			//Rezervacije ubaci u sesiju
			System.Web.HttpContext.Current.Application["reservations"] = output;

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
		#endregion
	}
}