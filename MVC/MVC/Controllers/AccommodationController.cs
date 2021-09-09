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
	}
}