using MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
	public class HomeController : Controller
	{
		private List<Arrangement> arrangements = new List<Arrangement>();

		public ActionResult Index()
		{
			//Ocitavamo sve aranzmane iz baze
			LoadArrangements();

			//Sortiramo aranzmane po rastucem redosledu
			arrangements.Sort((x, y) => x.StartDateOfArrangement.CompareTo(y.EndDateOfArrangement));

			//Ubacujemo aranzmane u sesiju
			System.Web.HttpContext.Current.Application["arrangements"] = arrangements;

			return View("Index");
		}

		//Ucitavamo podatke iz baze i upisujemo u listu
		private void LoadArrangements()
		{
			arrangements = XMLHelper.LoadArrangements();
		}

		private void InitData()
		{
			AccommodationUnit unit1 = new AccommodationUnit(1, 15, 15, true, 200);
			AccommodationUnit unit2 = new AccommodationUnit(2, 5, 5, false, 500);
			AccommodationUnit unit3 = new AccommodationUnit(3, 20, 20, false, 100);

			Accommodation accommodation1 = new Accommodation(1, AccommodationType.Villa, "Top Start Villa", 0, false, false, true, true, new List<int>() { unit1.ID });
			Accommodation accommodation2 = new Accommodation(2, AccommodationType.Hotel, "Luksor", 4, true, false, true, true, new List<int>() { unit2.ID, unit3.ID });

			StartLocation start1 = new StartLocation(1, "Bulevar Evrope", 70, "Novi Sad", 25000, 45.24353, 19.82023);

			DateTime date1 = new DateTime(2021, 9, 20);
			DateTime date2 = new DateTime(2021, 9, 24);
			DateTime time1 = new DateTime(2021, 9, 20, 4, 30, 0);

			Arrangement arrangement1 = new Arrangement(1, "Bec", ArrangementType.NightWithBreakfast, TransportationType.Bus, "Bec", date1,
				date2, 1, time1, 30, "---", "---", "Bec.jpg", 1);

			DateTime date3 = new DateTime(2021, 7, 22);
			DateTime date4 = new DateTime(2021, 7, 30);
			DateTime time2 = new DateTime(2021, 7, 22, 1, 0, 0);

			Arrangement arrangement2 = new Arrangement(2, "Egipat", ArrangementType.AllInclusive, TransportationType.Airplane, "Egipat", date3,
				date4, 1, time2, 10, "---", "---", "Egipat.jpg", 2);

			DateTime date5 = new DateTime(1992, 7, 1);
			DateTime date6 = new DateTime(1995, 9, 12);
			DateTime date7 = new DateTime(1999, 1, 24);

			User user1 = new User("admin", "admin", "admin", "admin", Gender.Male, "admin@gmail.com", date5, Role.Admin);
			User user2 = new User("manager", "manager", "Marija", "Davidovac", Gender.Female, "marijad@gmail.com", date6, Role.Manager);
			user2.CreatedArrangemetsID.Add(arrangement1.ID);
			user2.CreatedArrangemetsID.Add(arrangement2.ID);
			User user3 = new User("Nemac44", "Nemac44", "Nemanja", "Miljkovic", Gender.Male, "nemacnemanja@gmail.com", date7, Role.Tourist);

			Reservation reservation1 = new Reservation("Santorini_1", user3.Username, ReservationStatus.Expired, arrangement2.ID, unit2.ID);

			Comment comment1 = new Comment(1, user3.Username, arrangement2.ID, "Vreme je bilo odlicno svidelo mi se!", 5);

			List<Accommodation> accommodations = new List<Accommodation>() { accommodation1, accommodation2 };
			List<AccommodationUnit> units = new List<AccommodationUnit>() { unit1, unit2, unit3 };
			List<Arrangement> arrangments = new List<Arrangement>() { arrangement1, arrangement2 };
			List<User> users = new List<Models.User>() { user1, user2, user3 };
			List<Reservation> reservations = new List<Reservation>() { reservation1 };
			List<Comment> comments = new List<Comment>() { comment1 };
			List<StartLocation> starts = new List<StartLocation>() { start1 };

			XMLHelper.SaveAccommodations(accommodations);
			XMLHelper.SaveAccommodationUnits(units);
			XMLHelper.SaveArrangements(arrangments);
			XMLHelper.SaveUsers(users);
			XMLHelper.SaveReservations(reservations);
			XMLHelper.SaveComments(comments);
			XMLHelper.SaveStartLocations(starts);
		}
	}
}