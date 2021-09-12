using MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class AccommodationUnitController : Controller
    {
		private List<AccommodationUnit> accommodationUnits = new List<AccommodationUnit>();
		private List<Reservation> reservations = new List<Reservation>();

		#region Akcije
		//Akcija sluzi za pretragu smestajnih jedinica
		public ActionResult SearchAccommodationUnits(AccommodationUnitSearchModel searchModel)
		{
			//Ocitavamo sve smestajne jedinice iz baze
			LoadAccommodationUnits();
			
			//Izvlacimo smestaj iz sesije
			Accommodation accommodation = (Accommodation)System.Web.HttpContext.Current.Application["accommodation"];
			List<AccommodationUnit> units = new List<AccommodationUnit>();

			//U listu smestajnih jedinica ubacujemo samo jedinice koje se nalaze u smestaju
			foreach (var unit in accommodation.AccommodationUnitID)
			{
				units.Add(GetAccommodationUnit(unit));
			}

			//Pozivamo funkciju za pretragu
			var output = SearchAccommodationUnitsPrivate(units, searchModel);

			//Ubacujemo smestajne jedinice u sesiju
			System.Web.HttpContext.Current.Application["accommodationUnits"] = output;

			return View("../Accommodation/Details");
		}

		//Akcija koja otvara listu svih smestajnih jedinica
		public ActionResult OpenManagerAccommodationUnitsListPage()
		{
			//Ocitavamo sve smestajne jedinice iz baze
			LoadAccommodationUnits();

			//Ubacujemo smestaje u sesiju
			System.Web.HttpContext.Current.Application["accommodationUnits"] = accommodationUnits;

			return View("ManagerAccommodationUnitsList");
		}

		//Akcija koja otvara prozor za kreiranje smestajne jedinice
		public ActionResult OpenCreatePage()
		{
			return View("Create");
		}

		//Akcija vrsi kreiranje nove smestajne jedinice
		public ActionResult Create(AccommodationUnit accommodationUnit)
		{
			//Vrsi se validacija unetih vrednosti
			bool validate = accommodationUnit.Validate();
			if (validate == false)
			{
				ViewBag.Message = "Input fields can't be empty!";
				return View("Create");
			}

			//Ocitavamo sve smestajne jedinice iz baze
			LoadAccommodationUnits();

			//Generisemo novi id
			accommodationUnit.GenerateID();

			//Postavljamo da trenutni broj soba je isti kao ukupan broj soba
			accommodationUnit.NumberOfFreeRooms = accommodationUnit.NumberOfTotalRooms;

			//Dodamo ga u listu
			accommodationUnits.Add(accommodationUnit);

			//Sacuvamo izmene
			SaveAccommodationUnits();

			return RedirectToAction("OpenManagerAccommodationUnitsListPage");
		}

		//Akcija koja brise smestajnu jediniu
		public ActionResult Delete(int id)
		{
			//Ucitavamo iz baze sve smestajne jedinice i rezervacije
			LoadAccommodationUnits();
			LoadReservations();

			//Trazimo smestajnu jedinicu iz baze na osnovu idija
			AccommodationUnit accommodationUnit = GetAccommodationUnit(id);

			//Proveravamo da li se sve smestajna jedinica obrisati
			bool canDelete = CheckIfReservationsIsActive(id);
			if (canDelete == false)
			{
				ViewBag.Message = $"You can't removed this accommodation unit, because there are some reservations!";
				return View("ManagerAccommodationUnitsList");
			}

			//Logicki obrisi smestajnu jedinicu
			accommodationUnit.IsRemoved = true;

			//Sacuvaj izmene
			SaveAccommodationUnits();

			return RedirectToAction("OpenManagerAccommodationUnitsListPage");
		}

		//Akcija koja otvara prozor za modifikaciju izabrane smestajne jedinice
		public ActionResult OpenEditPage(int id)
		{
			//Ocitavamo svesmestajne jedinice iz baze
			LoadAccommodationUnits();

			//Trazimo izabranu smestajnu jedinicu na osnovu idija
			AccommodationUnit unit = GetAccommodationUnit(id);

			//Ubacujemo smestajnu jedinicu u sesiju
			System.Web.HttpContext.Current.Application["accommodationUnit"] = unit;

			return View("Edit");
		}

		//Akcija koja vrsi azuriranje smestajne jedinice
		public ActionResult Edit(int ID, AccommodationUnit accommodationUnit)
		{
			//Vrsi se validacija unetih vrednosti
			bool validate = accommodationUnit.Validate();
			if (validate == false)
			{
				ViewBag.Message = "Input fields can't be empty!";
				return View("Edit");
			}

			//Ocitavamo sve smestajne jedinice iz baze
			LoadAccommodationUnits();

			//Proveravamo da li je smestajna jedinica postoji u bazi
			AccommodationUnit exists = GetAccommodationUnit(ID);
			if (exists != null)
			{
				//Azuriramo podatke
				exists.NumberOfFreeRooms = accommodationUnit.NumberOfTotalRooms;
				exists.Pets = accommodationUnit.Pets;
				exists.Price = accommodationUnit.Price;
				exists.NumberOfTotalRooms = accommodationUnit.NumberOfTotalRooms;
			}

			//Sacuvamo izmene
			SaveAccommodationUnits();

			//I otvaramo akciju za ispis menadzerovih smestajnih jedinica
			return RedirectToAction("OpenManagerAccommodationUnitsListPage");
		}

		//Akcija koja vrsi pretragu svih smestajnih jedinica
		public ActionResult SearchAccommodationUnitsManager(AccommodationUnitSearchModel searchModel)
		{
			//Ocitavamo sve smestajne jedinice iz baze
			LoadAccommodationUnits();
			
			//Poziva se metoda za pretragu
			List<AccommodationUnit> output = SearchAccommodationUnitsPrivate(accommodationUnits, searchModel);

			//U sesiju ubacujemo pretrazene rezultate
			System.Web.HttpContext.Current.Application["accommodationUnits"] = output;

			return View("ManagerAccommodationUnitsList");
		}
		#endregion

		#region Pomocne funkcije
		private List<AccommodationUnit> SearchAccommodationUnitsPrivate(List<AccommodationUnit> accommodationUnits, AccommodationUnitSearchModel searchModel)
		{
			List<AccommodationUnit> output = new List<AccommodationUnit>();

			foreach (var accommodationUnit in accommodationUnits)
			{
				//Provera za minimalni broj slobodnih soba
				if (searchModel.GuestsMin != null)
				{
					if (accommodationUnit.NumberOfFreeRooms <= searchModel.GuestsMin)
					{
						//Ako ne zadovoljava uslov, preskoci entitet
						continue;
					}
				}

				//Provera za maksimalni broj slobodnih soba
				if (searchModel.GuestsMax != null)
				{
					if (accommodationUnit.NumberOfFreeRooms >= searchModel.GuestsMax)
					{
						//Ako ne zadovoljava uslov, preskoci entitet
						continue;
					}
				}

				//Provera za ljubimce
				if (searchModel.Pets != null)
				{
					if (accommodationUnit.Pets != searchModel.Pets)
					{
						//Ako ne zadovoljava uslov, preskoci entitet
						continue;
					}
				}

				//Provera za cenu
				if (searchModel.Price != null)
				{
					if (searchModel.Price <= accommodationUnit.Price)
					{
						//Ako ne zadovoljava uslov, preskoci entitet
						continue;
					}
				}

				//I na kraju dolazi da je entitet zadovoljivo sve uslove i dodajemo ga u listu
				output.Add(accommodationUnit);
			}

			return output;
		}

		//Funkcija koja proverava da li neka rezervacija sadrzi smestajnu jedinicu koju zelimo obrisati ili modifikovati
		private bool CheckIfReservationsIsActive(int id)
		{
			foreach (var reservation in reservations)
			{
				//Proverava se da li je rezervacija rezervisana za odabrani smestaj i provera se da li je rezervacija aktivna
				if (reservation.AccommodationUnitID == id && reservation.ReservationStatus == ReservationStatus.Active)
				{
					//Ako jeste
					return false;
				}
			}

			//Ako nije
			return true;
		}
		#endregion

		#region Load funkcije
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
		//Listu smestajnih jedinica upisujemo u bazu
		private void SaveAccommodationUnits()
		{
			XMLHelper.SaveAccommodationUnits(accommodationUnits);
		}
		#endregion
	}
}