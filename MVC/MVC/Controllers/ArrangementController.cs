using MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class ArrangementController : Controller
    {
		private List<Arrangement> arrangements = new List<Arrangement>();

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

		//Ucitavamo podatke iz baze i upisujemo u listu
		private void LoadArrangements()
		{
			arrangements = XMLHelper.LoadArrangements();
		}
	}
}