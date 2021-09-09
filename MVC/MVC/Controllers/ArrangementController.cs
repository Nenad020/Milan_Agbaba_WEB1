using MVC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class ArrangementController : Controller
    {
		private List<Arrangement> arrangements = new List<Arrangement>();
		private List<StartLocation> startLocations = new List<StartLocation>();
		private List<Accommodation> accommodations = new List<Accommodation>();
		private List<Reservation> reservations = new List<Reservation>();
		private List<User> users = new List<User>();

		#region Akcije
		//Akcija koja otvara prozor u kome se nalaze svi aranzmani (prosli i buduci)
		public ActionResult OpenArrangementsListPage()
		{
			//Ocitavamo sve aranzmane iz baze
			LoadArrangements();

			//Sortiramo aranzmane po rastucem redosledu
			arrangements.Sort((x, y) => x.StartDateOfArrangement.CompareTo(y.EndDateOfArrangement));

			//Ubacujemo aranzmane u sesiju
			System.Web.HttpContext.Current.Application["arrangements"] = arrangements;

			return View("ArrangementList");
		}

		//Akcija sluzi za pretragu aranzmana
		public ActionResult SearchArrangements(ArrangementSearchModel searchModel)
		{
			//Ocitavamo sve aranzmane iz baze
			LoadArrangements();

			//Pozivamo funkciju za pretragu
			List<Arrangement> output = SearchArrangementsPrivate(arrangements, searchModel);

			//Ubacujemo aranzmane u sesiju
			System.Web.HttpContext.Current.Application["arrangements"] = output;

			return View("ArrangementList");
		}

		//Akcija koja otvara stranicu sa detaljima za aranzman
		public ActionResult Details(int id)
		{
			//Ocitavamo sve aranzmane i pocetne lokacije iz baze
			LoadArrangements();
			LoadStartLocations();

			//Trazimo odgovarajuci aranzman i pocetnu lokaciju
			Arrangement arrangement = GetArrangement(id);
			StartLocation startLocation = GetStartLocation(arrangement.StartLocationID);

			//Ubacujemo aranzman i pocetnu lokaciju u sesiju
			System.Web.HttpContext.Current.Application["arrangement"] = arrangement;
			System.Web.HttpContext.Current.Application["startLocation"] = startLocation;

			return View("Details");
		}

		//Akcija koja otvara stranicu gde se nalaze svi aranzmani koje je ulogovani menadzer kreirao
		public ActionResult OpenManagerArrangementsListPage()
		{
			//Ocitavamo sve aranzmane iz baze
			LoadArrangements();

			//Vadimo korisnika iz sesija i pravimo praznu listu aranzmana
			User user = (User)System.Web.HttpContext.Current.Application["user"];
			List<Arrangement> output = new List<Arrangement>();

			//Prolazimo kroz sve menadzerove aranzmane
			foreach (var arrangementID in user.CreatedArrangemetsID)
			{
				//Na osnovu idija dobijamo ceo objekat aranzmana
				Arrangement item = GetArrangement(arrangementID);

				//I ubacujemo ga u listu
				output.Add(item);
			}

			//Listu aranzmana ubacujemo u sesiju
			System.Web.HttpContext.Current.Application["arrangements"] = output;

			return View("ManagerArrangemetsList");
		}

		//Akcija koja otvara prozor za kreiranje aranzmana
		public ActionResult OpenCreatePage()
		{
			//Ucitavamo sve smestaje i pocetne lokacije
			LoadAccommodations();
			LoadStartLocations();

			//Listu smestaja i pocetnih lokacija ubacujemo u sesiju
			System.Web.HttpContext.Current.Application["accommodations"] = accommodations;
			System.Web.HttpContext.Current.Application["startLocations"] = startLocations;

			return View("Create");
		}

		//Akcija koja kreira novi aranzman
		public ActionResult Create(Arrangement arrangement, HttpPostedFileBase poster)
		{
			//Vrsi se validacija unetih vrednosti
			bool validate = arrangement.Validate();
			if (validate == false)
			{
				ViewBag.Message = "Input fields can't be empty!";
				return View("Create");
			}

			//Ocitavamo sve aranzmane iz baze
			LoadArrangements();

			//Vadimo korisnika iz sesije
			User user = (User)System.Web.HttpContext.Current.Application["user"];

			//Trazimo putanju fajla kojeg smo ucitali kroz prozor i sacuvamo taj fajl na lokaciji
			string path = Path.Combine(Server.MapPath("~/Content/Pictures"), Path.GetFileName(poster.FileName));
			poster.SaveAs(path);

			//Generisemo novi id za aranzman
			arrangement.GenerateID();

			//Za sliku aranzmana stavimo naziv slike
			arrangement.PosterOfArrangement = poster.FileName;

			//Dodamo aranzman u listu
			arrangements.Add(arrangement);

			//Azuriramo menadzerove kreirane aranzmane
			UpdateManagerArrangementList(user.Username, arrangement.ID);

			//Sacuvamo izmene
			SaveArrangements();

			//U sesiju dodamo korisnika
			System.Web.HttpContext.Current.Application["user"] = user;

			return RedirectToAction("OpenManagerArrangementsListPage");
		}

		//Akcija koja otvara prozor za modifikaciju izabranog aranzmana
		public ActionResult OpenEditPage(int id)
		{
			//Ocitavamo sve aranzmane iz baze
			LoadArrangements();

			//Trazimo aranzman iz baze na osnovu idija
			Arrangement arrangement = GetArrangement(id);

			//Proveravamo da li mozemo da izvrsimo izmenu
			bool canModify = CheckIfArrangementIsExpired(arrangement);
			if (canModify == false)
			{
				ViewBag.Message = "Arrangement has expired, you can't update it!";
				return View("ManagerArrangemetsList");
			}

			//Ucitavamo pocetne lokacije i smestaje iz baze
			LoadStartLocations();
			LoadAccommodations();

			//U sesiju dodajemo aranzman, pocetne lokacije i smestaje
			System.Web.HttpContext.Current.Application["arrangement"] = arrangement;
			System.Web.HttpContext.Current.Application["startLocations"] = startLocations;
			System.Web.HttpContext.Current.Application["accommodations"] = accommodations;

			return View("Edit");
		}

		//Akcija koja vrsi azuriranje aranzmana
		public ActionResult Edit(int ID, Arrangement arrangement, HttpPostedFileBase poster)
		{
			//Vrsi se validacija unetih vrednosti
			bool validate = arrangement.Validate();
			if (validate == false)
			{
				ViewBag.Message = "Input fields can't be empty!";
				return View("Edit");
			}

			//Ocitavamo sve aranzmane iz baze
			LoadArrangements();

			//Trazimo putanju fajla kojeg smo ucitali kroz prozor i sacuvamo taj fajl na lokaciji
			string path = Path.Combine(Server.MapPath("~/Content/Pictures"), Path.GetFileName(poster.FileName));
			poster.SaveAs(path);

			//Proveravamo da li je aranzmana postoji u bazi
			Arrangement exists = GetArrangement(ID);
			if (exists != null)
			{
				//Azuriramo podatke
				exists.StartDateOfArrangement = arrangement.StartDateOfArrangement;
				exists.EndDateOfArrangement = arrangement.EndDateOfArrangement;
				exists.ArrangementType = arrangement.ArrangementType;
				exists.NumberOfPassengers = arrangement.NumberOfPassengers;
				exists.Description = arrangement.Description;
				exists.Destination = arrangement.Destination;
				exists.Name = arrangement.Name;
				exists.StartLocationID = arrangement.StartLocationID;
				exists.StartTime = arrangement.StartTime;
				exists.TransportationType = arrangement.TransportationType;
				exists.TravelProgram = arrangement.TravelProgram;
				exists.PosterOfArrangement = poster.FileName;
				exists.AccommodationID = arrangement.AccommodationID;
			}

			//Sacuvamo izmene
			SaveArrangements();

			//I otvaramo akciju za ispis menadzerovih aranzmana
			return RedirectToAction("OpenManagerArrangementsListPage");
		}

		//Akcija koja vrsi brisanje aranzmana
		public ActionResult Delete(int id)
		{
			//Ocitavamo sve aranzmane i rezervacije iz baze
			LoadArrangements();
			LoadReservations();

			//Trazimo aranzman iz baze na osnovu idija
			Arrangement arrangement = GetArrangement(id);

			//Proveravamo da li mozemo da izvrsimo brisanje
			bool canDelete = CheckIfArrangementIsExpired(arrangement);
			if (canDelete == false)
			{
				ViewBag.Message = "Arrangement has expired, you can't delete it!";
				return View("ManagerArrangemetsList");
			}
			
			//Prolazimo kroz svaku rezervaciju
			foreach (var reservation in reservations)
			{
				//Proveravamo da li ID aranzmana odgovara sa ID-ijem u rezervaciji
				//i proveravamo da li je rezervacija trenutno aktivna
				if (reservation.ArrangementID == id && reservation.ReservationStatus == ReservationStatus.Active)
				{
					//Ako jeste ispisujemo poruku greske
					ViewBag.Message = "Arrangement has expired, you can't delete it!";
					return View("ManagerArrangemetsList");
				}
			}

			//Logicki brisemo
			arrangement.IsRemoved = true;

			//Cuvamo podatke
			SaveArrangements();

			return RedirectToAction("OpenManagerArrangementsListPage");
		}

		//Akcija koja vrsi pretragu aranzmana nekog menadzera tj njegovim aranzmana
		public ActionResult SearchManagerArrangements(ArrangementSearchModel searchModel)
		{
			//Ocitavamo sve aranzmane iz baze
			LoadArrangements();

			//Vadimo korisnika iz sesije
			User user = (User)System.Web.HttpContext.Current.Application["user"];

			//Pravimo listu aranzmana
			List<Arrangement> arrangements2 = new List<Arrangement>();

			//Prolazimo kroz listu menadzerovim aranzmana, ali to su samo idijevi
			foreach (var arrangement in user.CreatedArrangemetsID)
			{
				//Izvlacimo aranzman na osnovu idija i ubacujemo ga u listu
				Arrangement item = GetArrangement(arrangement);
				arrangements2.Add(item);
			}

			//Pozivamo funkciju za pretragu
			List<Arrangement> output = SearchArrangementsPrivate(arrangements2, searchModel);

			//Ubacujemo aranzmane u sesiju
			System.Web.HttpContext.Current.Application["arrangements"] = output;

			return View("ManagerArrangemetsList");
		}
		#endregion

		#region Pomocne funkcije
		//Metoda koja vrsi pretragu aranzmana
		private List<Arrangement> SearchArrangementsPrivate(List<Arrangement> arrangements, ArrangementSearchModel searchModel)
		{
			//Lista u kojoj cuvamo povratnu vrednost metode
			//Svi aranzmani koji zadovoljavaju pretragu
			List<Arrangement> output = new List<Arrangement>();

			foreach (var arrangement in arrangements)
			{
				//Provera za minimalni startni datum
				if (searchModel.StartTimeMin != null)
				{
					if (arrangement.StartDateOfArrangement < searchModel.StartTimeMin)
					{
						//Ako ne zadovoljava uslov, preskoci entitet
						continue;
					}
				}

				//Provera za maksimalni startni datum
				if (searchModel.StartTimeMax!= null)
				{
					if (searchModel.StartTimeMax < arrangement.StartDateOfArrangement)
					{
						//Ako ne zadovoljava uslov, preskoci entitet
						continue;
					}
				}

				//Provera za minimalni zavrsni datum
				if (searchModel.EndTimeMin != null)
				{
					if (arrangement.EndDateOfArrangement > searchModel.EndTimeMin)
					{
						//Ako ne zadovoljava uslov, preskoci entitet
						continue;
					}
				}

				//Provera za maksimalni zavrsni datum
				if (searchModel.EndTimeMax != null)
				{
					if (searchModel.EndTimeMax < arrangement.EndDateOfArrangement)
					{
						//Ako ne zadovoljava uslov, preskoci entitet
						continue;
					}
				}

				//Provera za tip transporta
				if (searchModel.TransportationType != null)
				{
					if (arrangement.TransportationType != searchModel.TransportationType)
					{
						//Ako ne zadovoljava uslov, preskoci entitet
						continue;
					}
				}

				//Provera za tip aranzmana
				if (searchModel.ArrangementType != null)
				{
					if (arrangement.ArrangementType != searchModel.ArrangementType)
					{
						//Ako ne zadovoljava uslov, preskoci entitet
						continue;
					}
				}

				//Provera za naziv
				if (searchModel.Location != null && searchModel.Location != "")
				{
					//Izvlacimo indeks prvog slova unetog imena u pretrazi
					int index = arrangement.Name.ToLower().IndexOf(searchModel.Location.ToLower());
					if (index < 0)
					{
						//Ako ne zadovoljava uslov, preskoci entitet
						continue;
					}
				}

				//I na kraju dolazi da je entitet zadovoljivo sve uslove i dodajemo ga u listu
				output.Add(arrangement);
			}

			return output;
		}

		//Funkcija koja u listu kreiranih aranzmana dodajemo novi aranzman
		private void UpdateManagerArrangementList(string username, int arrangementID)
		{
			//Izvlacimo korisnika
			User user = GetUser(username);

			//Listu povecavamo za kreirani aranzman
			user.CreatedArrangemetsID.Add(arrangementID);

			//Sacuvamo izmene
			SaveUsers();
		}

		//Proveravamo da li je aranzman istekao
		private bool CheckIfArrangementIsExpired(Arrangement arrangement)
		{
			//Ako je trenutno vreme vece od pocetnog ili krajnjeg vremena isteka aranzmana
			if (DateTime.Now > arrangement.StartDateOfArrangement || DateTime.Now > arrangement.EndDateOfArrangement)
			{
				//Vrati false
				return false;
			}

			//Suprotno vrati true
			return true;
		}
		#endregion

		#region Load funkcije
		//Ucitavamo podatke iz baze i upisujemo u listu
		private void LoadArrangements()
		{
			arrangements = XMLHelper.LoadArrangements();
		}

		//Ucitavamo podatke iz baze i upisujemo u listu
		private void LoadStartLocations()
		{
			startLocations = XMLHelper.LoadStartLocations();
		}

		//Ucitavamo podatke iz baze i upisujemo u listu
		private void LoadAccommodations()
		{
			accommodations = XMLHelper.LoadAccommodations();
		}

		//Ucitavamo podatke iz baze i upisujemo u listu
		private void LoadUsers()
		{
			users = XMLHelper.LoadUsers();
		}

		//Ucitavamo podatke iz baze i upisujemo u listu
		private void LoadReservations()
		{
			reservations = XMLHelper.LoadReservations();
		}
		#endregion

		#region Get funkcije
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

		//Trazi se pocetna lokacija iz liste na osnovu idija
		private StartLocation GetStartLocation(int id)
		{
			//Prolazimo kroz sve pocetne lokacije
			foreach (var startLocation in startLocations)
			{
				//Ako je id pocetne lokacije isti kao onaj u parametru
				if (startLocation.ID == id)
				{
					//Nasli smo ga
					return startLocation;
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

		//Trazi se korisnik iz liste na osnovu korisnickog imena
		private User GetUser(string username)
		{
			//Prolazimo kroz sve korisnike
			foreach (var user in users)
			{
				//Ako je korisnicko ime korisnika isti kao onaj u parametru
				if (user.Username == username)
				{
					//Nasli smo ga
					return user;
				}
			}

			//Ako ga nismo nasli, baci null
			return null;
		}

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
		#endregion

		#region Save funkcije
		//Listu korisnika upisujemo u bazu
		private void SaveUsers()
		{
			XMLHelper.SaveUsers(users);
		}

		//Listu aranzmana upisujemo u bazu
		private void SaveArrangements()
		{
			XMLHelper.SaveArrangements(arrangements);
		}

		//Listu rezervacija upisujemo u bazu
		private void SaveReservations()
		{
			XMLHelper.SaveReservations(reservations);
		}
		#endregion
	}
}