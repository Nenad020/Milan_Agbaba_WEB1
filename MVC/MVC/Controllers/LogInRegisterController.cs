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
				ViewBag.Message = "User with username: " + "already exists!";
				return View();
			}

			//Dodajemo korisnika u listu
			users.Add(user);

			//Sacuvamo u bazu
			SaveUsers();

			//Pozovemo view za Logovanje korisnika
			return View("LogIn");
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