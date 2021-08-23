using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC.Models
{
	public class StartLocation
	{
		//ID
		public int ID { get; set; }

		//Ulica
		public string Street { get; set; }

		//Broj
		public int Number { get; set; }

		//Grad
		public string City { get; set; }

		//Postanski broj
		public int PostalCode { get; set; }

		//Geografska duzina
		public double Longitude { get; set; }

		//Geografska sirina
		public double Latitude { get; set; }

		//Da li je objekat obrisan, ako jeste ne prikazujemo ga na sajtu
		public bool IsRemoved { get; set; } = false;

		public StartLocation()
		{
		}

		public StartLocation(int iD, string street, int number, string city, int postalCode, double latitude, double longitude)
		{
			ID = iD;
			Street = street;
			Number = number;
			City = city;
			PostalCode = postalCode;
			Latitude = latitude;
			Longitude = longitude;
			IsRemoved = false;
		}

		//Pogledaj kako je napisano u User.cs
		public bool Validate()
		{
			if (Street == null || Street == "" || Number <= 0 || City == null || City == "" || PostalCode <= 0 || Latitude <= 0 || Longitude <= 0)
			{
				return false;
			}

			return true;
		}
	}
}