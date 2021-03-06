using MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class CommentController : Controller
    {
		private List<Arrangement> arrangements = new List<Arrangement>();
		private List<Comment> comments = new List<Comment>();
		private List<Reservation> reservations = new List<Reservation>();
		private List<AccommodationUnit> accommodationUnits = new List<AccommodationUnit>();

		#region Akcije
		//Akcija koja otvara prozor za ispis svih komentara za izabrani aranzman
		public ActionResult ArrangmentComments(int id)
		{
			//Ocitavamo sve aranzmane i komentare iz baze
			LoadArrangements();
			LoadComments();

			//Trazimo aranzman na osnovu idija
			Arrangement arrangement = GetArrangement(id);

			List<Comment> output = new List<Comment>();

			//Prolazimo kroz listu svih komentara
			foreach (var comment in comments)
			{
				//Proveravamo da li dati komentar odgovara za izabrani aranzman
				if (comment.ArrangementID == id)
				{
					//Ako odgovara dodaj ga u listu
					output.Add(comment);
				}
			}

			//Ubacujemo aranzman i komentare u sesiju
			System.Web.HttpContext.Current.Application["arrangement"] = arrangement;
			System.Web.HttpContext.Current.Application["comments"] = output;

			return View("CommentList");
		}

		//Akcija koja otvara prozor za kreiranje komentara
		public ActionResult OpenCreatePage()
		{
			//Ucitavamo sve aranzmane, rezeravacije i smestajne jedinice
			LoadArrangements();
			LoadReservations();
			LoadAccommodationUnits();

			//Ucitavamo korisnika iz sesije
			User user = (User)System.Web.HttpContext.Current.Application["user"];

			//Prolazimo kroz svaku rezervaciju
			foreach (var reservation in reservations)
			{
				//Pozivamo funkciju za azuriranje statusa rezervacije
				UpdateReservationStatus(reservation);

				//Proveravamo da li je ulogovani turista ima rezeravciju aranzmana za koji hoce ostaviti komentar
				//I proveravamo da li je rezeravacija istekla
				if (reservation.TouristUsername == user.Username && reservation.ReservationStatus == ReservationStatus.Expired)
				{
					//Ako jeste otvori prozor za kreiranje komentara
					return View("Create");
				}
			}

			//Ako nije izbaci poruku greske
			ViewBag.Message = "First you need to have reservations, then you can leave comment!";

			return View("CommentList");
		}

		//Akcija koja vrsi dodavanje novog komentara
		public ActionResult Create(Comment comment)
		{
			//Vrsi se validacija unetih vrednosti
			bool validate = comment.Validate();
			if (validate == false)
			{
				ViewBag.Message = "Input fields can't be empty!";
				return View("Create");
			}

			//Ocitavamo sve komentare iz baze
			LoadComments();

			//Ucitavamo korisnika i aranzman iz sesije
			User user = (User)System.Web.HttpContext.Current.Application["user"];
			Arrangement arrangement = (Arrangement)System.Web.HttpContext.Current.Application["arrangement"];

			//Uzimamo id od arazmana i dodeljujemo ga komentaru
			comment.ArrangementID = arrangement.ID;

			//Uzimamo korisnicko ime turiste i doduljemo ga komentaru
			comment.TouristUsername = user.Username;

			//Generisemo novi id za komentar
			comment.GenerateID();

			//Dodajemo komentar u listu
			comments.Add(comment);

			//Sacuvamo u bazu
			SaveComments();

			//Pozovemo view za Logovanje korisnika
			return View("CommentList");
		}

		//Akcija koja otvara prozor za ispis svih komentara u okviru menadzerovih aranzmana
		public ActionResult OpenManagerCommentsListPage()
		{
			//Ocitavamo sve komentare i aranzmane
			LoadComments();
			LoadArrangements();

			//Vadimo korisnika iz sesija i pravimo praznu listu komentara
			User user = (User)System.Web.HttpContext.Current.Application["user"];
			List<Comment> output = new List<Comment>();

			//Prolazimo kroz sve komentare
			foreach (var comment in comments)
			{
				//Proveravamo da li u menadzerovoj listi kreiranih aranzmana sadrzi id aranzmana datog komentara
				if (user.CreatedArrangemetsID.Contains(comment.ArrangementID))
				{
					//Ako sadrzi dodaj u ispisnu listu
					output.Add(comment);
				}
			}

			//Listu komentara i aranzmana ubacujemo u sesiju
			System.Web.HttpContext.Current.Application["comments"] = comments;
			System.Web.HttpContext.Current.Application["arrangements"] = arrangements;

			return View("ManagerCommentList");
		}

		//Akcija koja sluzi za odobravanje komentara
		public ActionResult Approve(int id)
		{
			//Ucitavamo sve komentare
			LoadComments();

			//Trazimo komentar iz baze na osnovu idija
			Comment comment = GetComment(id);

			//Polje za odobravanje postavimo na tacno
			comment.IsApproved = true;

			//Sacuvamo izmene
			SaveComments();

			return RedirectToAction("OpenManagerCommentsListPage");
		}

		//Akcija koja sluzi za odbijanje komentara
		public ActionResult Ban(int id)
		{
			//Ucitavamo sve komentare
			LoadComments();

			//Trazimo komentar iz baze na osnovu idija
			Comment comment = GetComment(id);

			//Polje za odobravanje postavimo na netacno
			comment.IsApproved = false;

			//Sacuvamo izmene
			SaveComments();

			return RedirectToAction("OpenManagerCommentsListPage");
		}
		#endregion

		#region Load funkcije
		//Ucitavamo podatke iz baze i upisujemo u listu
		private void LoadArrangements()
		{
			arrangements = XMLHelper.LoadArrangements();
		}

		//Ucitavamo podatke iz baze i upisujemo u listu
		private void LoadComments()
		{
			comments = XMLHelper.LoadComments();
		}

		//Ucitavamo podatke iz baze i upisujemo u listu
		private void LoadReservations()
		{
			reservations = XMLHelper.LoadReservations();
		}

		//Ucitavamo podatke iz baze i upisujemo u listu
		private void LoadAccommodationUnits()
		{
			accommodationUnits = XMLHelper.LoadAccommodationUnits();
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

		//Listu komentara upisujemo u bazu
		private void SaveComments()
		{
			XMLHelper.SaveComments(comments);
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

		//Trazi se komentar iz liste na osnovu idija
		private Comment GetStartLocaiton(int id)
		{
			//Prolazimo kroz sve komentare
			foreach (var comment in comments)
			{
				//Ako je id komentara isti kao onaj u parametru
				if (comment.ID == id)
				{
					//Nasli smo ga
					return comment;
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

		//Trazi se komentar iz liste na osnovu idija
		private Comment GetComment(int id)
		{
			//Prolazimo kroz sve komentare
			foreach (var comment in comments)
			{
				//Ako je id komentara isti kao onaj u parametru
				if (comment.ID == id)
				{
					//Nasli smo ga
					return comment;
				}
			}

			//Ako ga nismo nasli, baci null
			return null;
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