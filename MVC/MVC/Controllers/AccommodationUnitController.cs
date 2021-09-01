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
		#endregion

		#region Load funkcije
		//Ucitavamo podatke iz baze i upisujemo u listu
		private void LoadAccommodationUnits()
		{
			accommodationUnits = XMLHelper.LoadAccommodationUnits();
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
	}
}