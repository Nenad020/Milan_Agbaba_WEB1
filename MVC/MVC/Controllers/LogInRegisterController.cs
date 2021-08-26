using MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class LogInRegisterController : Controller
    {
		//Lista korisnika
		private List<User> users = new List<User>();

        //Otvara se prozor za registraciju korisnika
        public ActionResult OpenRegisterPage()
        {
            return View("Register");
        }

		//Vrsi se registracija korisnika
		public ActionResult Register(User user)
		{
			//Vrsi se validacija unetih vrednosti
			bool validate = user.Validate();
			if (validate == false)
			{
				ViewBag.Message = "Input fields can't be empty!";
				return View();
			}

			//Ocitavamo sve korisnike iz baze
			LoadUsers();

			//Proveravamo da li je uneto korisnicko ime vec zauzeto
			bool exists = CheckIfUserExists(user.Username);
			if (exists == true)
			{
				ViewBag.Message = "User with username: " + user.Username + " already exists!";
				return View();
			}

			//Dodajemo korisnika u listu
			users.Add(user);

			//Sacuvamo u bazu
			SaveUsers();

			//Pozovemo view za Logovanje korisnika
			return View("LogIn");
		}

		//Otvara se prozor za logovanje korisnika
		public ActionResult OpenLogInPage()
		{
			return View("LogIn");
		}

		public ActionResult LogIn(User user)
		{
			//Vrsi se validacija unetih vrednosti
			bool validate = user.ValidateLogIn();
			if (validate == false)
			{
				ViewBag.Message = "Input fields can't be empty!";
				return View();
			}

			//Ocitavamo sve korisnike iz baze
			LoadUsers();

			//Proveravamo da li je korisnik vec postoji u bazi sa datim kredencijalima
			User exists = CheckIfUserExists(user.Username, user.Password);
			if (exists == null)
			{
				ViewBag.Message = "User with username: " + user.Username + " doesn't exists!";
				return View();
			}

			//Ubacujemo korisnika u sesiju
			System.Web.HttpContext.Current.Application["user"] = exists;

			//I otvaramo akciju za ispis aktivnih aranzmana na pocetnoj stranici
			return RedirectToAction("../Home/Index");
		}

		public ActionResult OpenProfilePage()
		{
			return View("Profile");
		}

		//Vrsi se provera da li korisnik postoji u listi korisnika
		private bool CheckIfUserExists(string username)
		{
			//Prolazimo kroz svakog korisnika
			foreach (var user in users)
			{
				//Ako je korisnicko ime jednako onom sto smo prosledili kao parametar
				if (user.Username == username)
				{
					//Znaci da korisnik postoji
					return true;
				}
			}

			//U suprotnom ne postoji
			return false;
		}

		//Vrsi se provera da li korisnik postoji u listi korisnika
		private User CheckIfUserExists(string username, string password)
		{
			//Prolazimo kroz svakog korisnika
			foreach (var user in users)
			{
				//Ako je korisnicko ime jednako onom sto smo prosledili kao parametar
				//I ako je sifra jednaka isto onoj sto prosledili kao parametar
				if (user.Username == username && user.Password == password)
				{
					//Znaci da korisnik postoji
					return user;
				}
			}

			//U suprotnom ne postoji
			return null;
		}

		//Ucitavamo podatke iz baze i upisujemo u listu
		private void LoadUsers()
		{
			users = XMLHelper.LoadUsers();
		}

		//Listu korisnika upisujemo u bazu
		private void SaveUsers()
		{
			XMLHelper.SaveUsers(users);
		}
	}
}