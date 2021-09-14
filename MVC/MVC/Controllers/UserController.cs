using MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class UserController : Controller
    {
		private List<User> users = new List<User>();

		#region Akcije

		//Akcija koja otvara prozor sa listom svih korisnika sistema
		public ActionResult OpenUsersListPage()
		{
			//Citamo sve korisnike iz baze
			LoadUsers();

			//Korisnike ubacujemo u sesiju
			System.Web.HttpContext.Current.Application["users"] = users;

			return View("UsersList");
		}

		//Otvara se prozor za registraciju novog menadzera
		public ActionResult OpenAddManagerPage()
		{
			return View("AddManager");
		}

		//Akcija koja sluzi za banovanje korisnika
		public ActionResult Ban(string username)
		{
			//Citamo sve korisnike iz baze
			LoadUsers();

			//Izvlacimo trazenog korisnika
			User user = GetUser(username);

			//Banujemo ga, tj polje ban postavljamo na true
			user.IsBaned = true;

			//Sacuvamo izmene
			SaveUsers();

			return RedirectToAction("OpenUsersListPage");
		}

		//Vrsi se registracija menadzera
		public ActionResult AddManager(User user)
		{
			//Vrsi se validacija unetih vrednosti
			bool validate = user.Validate();
			if (validate == false)
			{
				ViewBag.Message = "Input fields can't be empty!";
				return View("AddManager");
			}

			//Ocitavamo sve korisnike iz baze
			LoadUsers();

			//Proveravamo da li je uneto korisnicko ime vec zauzeto
			bool exists = CheckIfUserExists(user.Username);
			if (exists == true)
			{
				ViewBag.Message = "User with username: " + user.Username + " already exists!";
				return View("AddManager");
			}

			//Podesavamo ulogu korisniku da bude menadzer
			user.Role = Role.Manager;

			//Dodajemo korisnika u listu
			users.Add(user);

			//Sacuvamo u bazu
			SaveUsers();

			return RedirectToAction("OpenUsersListPage");
		}

		//Akcija za pretrazivanje korisnika
		public ActionResult SearchUsers(UserSearchModel searchModel)
		{
			//Ocitavamo sve korisnike iz baze
			LoadUsers();

			//Pozivamo funkciju za pretragu korisnika
			List<User> output = SearchUsersPrivate(searchModel);

			//Korisnike ubacujemo u sesiju
			System.Web.HttpContext.Current.Application["users"] = output;

			return View("UsersList");
		}
		#endregion

		#region Load funkcije
		//Ucitavamo podatke iz baze i upisujemo u listu
		private void LoadUsers()
		{
			users = XMLHelper.LoadUsers();
		}
		#endregion

		#region Get funkcije
		//Trazi se korisnik iz liste na osnovu idija
		private User GetUser(string username)
		{
			//Prolazimo kroz sve korisnike
			foreach (var user in users)
			{
				//Ako je id korisnik isti kao onaj u parametru
				if (user.Username == username)
				{
					//Nasli smo ga
					return user;
				}
			}

			//Ako ga nismo nasli, baci null
			return null;
		}
		#endregion

		#region Save funkcije

		//Listu korisnika upisujemo u bazu
		private void SaveUsers()
		{
			XMLHelper.SaveUsers(users);
		}
		#endregion

		#region Pomocne funkcije
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

		//Funkcija koja sluzi za pretragu korisnika
		private List<User> SearchUsersPrivate(UserSearchModel searchModel)
		{
			List<User> output = new List<User>();

			foreach (var user in users)
			{
				//Provera za ime korisnika
				if (searchModel.Name != null && searchModel.Name != "")
				{
					//Izvlacimo indeks prvog slova unetog imena u pretrazi
					int index = user.Name.ToLower().IndexOf(searchModel.Name.ToLower());
					if (index < 0)
					{
						//Ako ne zadovoljava uslov, preskoci entitet
						continue;
					}
				}

				//Provera za prezime korisnika
				if (searchModel.Lastname != null && searchModel.Lastname != "")
				{
					//Izvlacimo indeks prvog slova unetog imena u pretrazi
					int index = user.Lastname.ToLower().IndexOf(searchModel.Lastname.ToLower());
					if (index < 0)
					{
						//Ako ne zadovoljava uslov, preskoci entitet
						continue;
					}
				}

				//Provera za ulogu korisnika
				if (searchModel.Role != null)
				{
					if (user.Role != searchModel.Role)
					{
						//Ako ne zadovoljava uslov, preskoci entitet
						continue;
					}
				}

				output.Add(user);
			}
			
			return output;
		}
		#endregion
	}
}