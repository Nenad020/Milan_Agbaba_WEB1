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
		#endregion

		#region Save funkcije
		//Listu rezervacija upisujemo u bazu
		private void SaveReservations()
		{
			XMLHelper.SaveReservations(reservations);
		}
		#endregion

	}
}