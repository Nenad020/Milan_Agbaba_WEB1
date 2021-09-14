using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC.Models
{
	//Ovi modeli su napravljeni zbog lakseg generisanja formi za pretragu
	//Kao sto imamo formu za registraciju tako i ovde imamo formu
	public class UserSearchModel
	{
		public UserSearchModel()
		{
		}

		public UserSearchModel(Role? role, string name, string lastname)
		{
			Role = role;
			Name = name;
			Lastname = lastname;
		}

		//Uloga korisnika
		public Role? Role { get; set; } = null;

		//Ime korisnik
		public string Name { get; set; }

		//Prezime korisnik
		public string Lastname { get; set; }
	}
}