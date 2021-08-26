using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace MVC.Models
{
	public class User
	{
		//Korisnicko ime
		public string Username { get; set; }

		//Sifra
		public string Password { get; set; }

		//Ime
		public string Name { get; set; }

		//Prezime
		public string Lastname { get; set; }

		//Pol, po default je musko
		public Gender Gender { get; set; } = Gender.Male;

		//Email
		public string Email { get; set; }

		//Datum rodjenja
		public DateTime DateOfBirth { get; set; }

		//Uloga, po default je turista
		public Role Role { get; set; } = Role.Tourist;

		//Lista idijeva aranzmana, ovo prestavlja listu kreiranih aranzmana
		//Kad menadzer kreira aranzman u ovu listu se doda id tog aranzmana
		public List<int> CreatedArrangemetsID { get; set; } = new List<int>();

		//Broj otkazanih putovanja, svaki put kad turista otkaze putovanje ovo polje se poveca za 1
		//Kad stigne na broj 2 ili vise, admin moze da ga banuje
		public int NumberOfCanceledTrips { get; set; } = 0;

		//Znak da li je korisnik banovan, ako jeste ne moze se vise prijaviti na sistem
		public bool IsBaned { get; set; } = false;

		public User()
		{
		}

		public User(string username, string password, string name, string lastname, Gender gender, string email, DateTime dateOfBirth, Role role)
		{
			Username = username;
			Password = password;
			Name = name;
			Lastname = lastname;
			Gender = gender;
			Email = email;
			DateOfBirth = dateOfBirth;
			Role = role;
			CreatedArrangemetsID = new List<int>();
			NumberOfCanceledTrips = 0;
			IsBaned = false;
		}

		//Ovo se poziva tokom dodavanje novog korisnika na sistema
		//Vrsi se validacija tj ne dozvoljavamo unos praznih polja
		public bool Validate()
		{
			if (Username == null || Username == "" || Password == null || Password == "" || Name == null || Name == "" || Lastname == null || Lastname == "" ||
				Email == null || Email == "" || DateOfBirth == null)
			{
				return false;
			}

			return true;
		}

		//Vrsi se validacija ali samo za logovanje
		public bool ValidateLogIn()
		{
			if (Username == null || Username == "" || Password == null || Password == "")
			{
				return false;
			}

			return true;
		}
	}
}