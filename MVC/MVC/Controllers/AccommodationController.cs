using MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class AccommodationController : Controller
    {
		private List<Arrangement> arrangements = new List<Arrangement>();
		private List<Accommodation> accommodations = new List<Accommodation>();
		private List<AccommodationUnit> accommodationUnits = new List<AccommodationUnit>();
		private List<Reservation> reservations = new List<Reservation>();

		#region Akcije
		//Akcija koja otvara prozor sa detaljima smestaja i njegovih smestajnih jedinica
		public ActionResult OpenAccommodationListPage(int id)
		{
			//Ocitavamo sve aranzmane, smestaje i smestajne jedinice iz baze
			LoadArrangements();
			LoadAccommodations();
			LoadAccommodationUnits();

			//Trazimo odgovarajuci aranzman, smestaj i smestajnu jedinicu
			Arrangement arrangement = GetArrangement(id);
			Accommodation accommodation = GetAccommodation(arrangement.AccommodationID);
			List<AccommodationUnit> units = new List<AccommodationUnit>();

			//U listu smestajnih jedinica ubacujemo samo jedinice koje se nalaze u smestaju
			foreach (var unit in accommodation.AccommodationUnitID)
			{
				units.Add(GetAccommodationUnit(unit));
			}

			//Ubacujemo smestaj i smestajne jedinice u sesiju
			System.Web.HttpContext.Current.Application["accommodation"] = accommodation;
			System.Web.HttpContext.Current.Application["accommodationUnits"] = units;

			return View("Details");
		}

		//Akcija koja otvara listu svih smestaja
		public ActionResult OpenManagerAccommodationsListPage()
		{
			//Ocitavamo sve smestaje iz baze
			LoadAccommodations();

			//Ubacujemo smestaje u sesiju
			System.Web.HttpContext.Current.Application["accommodations"] = accommodations;

			return View("ManagerAccommodationsList");
		}

		//Akcija koja otvara prozor za kreiranje smestaja
		public ActionResult OpenCreatePage()
		{
			//Ocitavamo sve smestajne jedinice iz baze
			LoadAccommodationUnits();

			//Ubacujemo smestajne jedinice u sesiju
			System.Web.HttpContext.Current.Application["accommodationUnits"] = accommodationUnits;

			return View("Create");
		}

		//Akcija vrsi kreiranje novog smestaja
		public ActionResult Create(Accommodation accommodation)
		{
			//Vrsi se validacija unetih vrednosti
			bool validate = accommodation.Validate();
			if (validate == false)
			{
				ViewBag.Message = "Input fields can't be empty!";
				return View("Create");
			}

			//Ocitavamo sve smestaje iz baze
			LoadAccommodations();

			//Generisemo novi id
			accommodation.GenerateID();

			//Dodamo ga u listu
			accommodations.Add(accommodation);

			//Sacuvamo izmene
			SaveAccommodations();
			
			return RedirectToAction("OpenManagerAccommodationsListPage");
		}

		//Akcija koja brise smestaj
		public ActionResult Delete(int id)
		{
			//Ucitavamo iz baze sve smestaje i rezervacije
			LoadAccommodations();
			LoadReservations();

			//Trazimo smestaj iz baze na osnovu idija
			Accommodation accommodation = GetAccommodation(id);

			//Prolazimo kroz sve rezervacije
			foreach (var reservation in reservations)
			{
				//Proveravamo da li neka smestajna jedinica unutar smestaja je zauzeta tj da li postoji rezervacija na njoj
				if (accommodation.AccommodationUnitID.Contains(reservation.AccommodationUnitID) && reservation.ReservationStatus == ReservationStatus.Active)
				{
					//Ako postoji ispisi gresku
					ViewBag.Message = $"You can't removed {accommodation.Name}, because there are some reservations!";
					return RedirectToAction("OpenManagerAccommodationsListPage");
				}
			}

			//Logicki obrisi smestaj
			accommodation.IsRemoved = true;

			//Sacuvaj izmene
			SaveAccommodations();

			return RedirectToAction("OpenManagerAccommodationsListPage");
		}

		//Akcija koja otvara prozor za modifikaciju izabranog smestaja
		public ActionResult OpenEditPage(int id)
		{
			//Ocitavamo sve smestaje i smestajne jedinice iz baze
			LoadAccommodations();
			LoadAccommodationUnits();

			//Trazimo izabrani smestaj na osnovu idija
			Accommodation accommodation = GetAccommodation(id);

			//Ubacujemo smestaj i smestajne jedinice u sesiju
			System.Web.HttpContext.Current.Application["accommodation"] = accommodation;
			System.Web.HttpContext.Current.Application["accommodationUnits"] = accommodationUnits;

			return View("Edit");
		}

		//Akcija koja vrsi azuriranje smestaja
		public ActionResult Edit(int ID, Accommodation accommodation)
		{
			//Vrsi se validacija unetih vrednosti
			bool validate = accommodation.Validate();
			if (validate == false)
			{
				ViewBag.Message = "Input fields can't be empty!";
				return View("Edit");
			}

			//Ocitavamo sve smestaje iz baze
			LoadAccommodations();

			//Proveravamo da li je smestaj postoji u bazi
			Accommodation exists = GetAccommodation(ID);
			if (exists != null)
			{
				//Azuriramo podatke
				exists.AccommodationType = accommodation.AccommodationType;
				exists.Disability = accommodation.Disability;
				exists.Name = accommodation.Name;
				exists.Pool = accommodation.Pool;
				exists.Spa = accommodation.Spa;
				exists.Stars = accommodation.Stars;
				exists.Wifi = accommodation.Wifi;
				exists.AccommodationUnitID = accommodation.AccommodationUnitID;
			}

			//Sacuvamo izmene
			SaveAccommodations();

			//I otvaramo akciju za ispis menadzerovih smestaja
			return RedirectToAction("OpenManagerAccommodationsListPage");
		}

		//Akcija sluzi za pretragu smestaja
		public ActionResult SearchAccommodations(AccommodationSearchModel searchModel)
		{
			//Ocitavamo sve smestaje iz baze
			LoadAccommodations();

			//Pozivamo funkciju za pretragu
			List<Accommodation> output = SearchAccommodationsPrivate(searchModel);

			//Ubacujemo smestaje u sesiju
			System.Web.HttpContext.Current.Application["accommodations"] = output;

			return View("ManagerAccommodationsList");
		}
		#endregion

		#region Load funkcije
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
		//Listu smestaja upisujemo u bazu
		private void SaveAccommodations()
		{
			XMLHelper.SaveAccommodations(accommodations);
		}
		#endregion

		#region Pomocne funkcije

		//Metoda koja vrsi pretragu smestaja
		private List<Accommodation> SearchAccommodationsPrivate(AccommodationSearchModel searchModel)
		{
			//Lista u kojoj cuvamo povratnu vrednost metode
			//Svi smestaji koji zadovoljavaju pretragu
			List<Accommodation> output = new List<Accommodation>();

			//Prolazimo kroz svaki smestaj
			foreach (var accommodation in accommodations)
			{
				//Provera za tip smestaja
				if (searchModel.AccommodationType != null)
				{
					if (accommodation.AccommodationType != searchModel.AccommodationType)
					{
						//Ako ne zadovoljava uslov, preskoci entitet
						continue;
					}
				}

				//Provera za ime smestaja
				if (searchModel.Name != null && searchModel.Name != "")
				{
					//Izvlacimo indeks prvog slova unetog imena u pretrazi
					int index = accommodation.Name.ToLower().IndexOf(searchModel.Name.ToLower());
					if (index < 0)
					{
						//Ako ne zadovoljava uslov, preskoci entitet
						continue;
					}
				}

				//Provera za bazen
				if (searchModel.Pool != null)
				{
					if (accommodation.Pool != searchModel.Pool)
					{
						//Ako ne zadovoljava uslov, preskoci entitet
						continue;
					}
				}

				//Provera za spa
				if (searchModel.Spa != null)
				{
					if (accommodation.Spa != searchModel.Spa)
					{
						//Ako ne zadovoljava uslov, preskoci entitet
						continue;
					}
				}

				//Provera za invalide
				if (searchModel.Disability != null)
				{
					if (accommodation.Disability != searchModel.Disability)
					{
						//Ako ne zadovoljava uslov, preskoci entitet
						continue;
					}
				}

				//Provera za wifi
				if (searchModel.Wifi != null)
				{
					if (accommodation.Wifi != searchModel.Wifi)
					{
						//Ako ne zadovoljava uslov, preskoci entitet
						continue;
					}
				}

				//I na kraju dolazi da je entitet zadovoljivo sve uslove i dodajemo ga u listu
				output.Add(accommodation);
			}

			return output;
		}

		#endregion
	}
}