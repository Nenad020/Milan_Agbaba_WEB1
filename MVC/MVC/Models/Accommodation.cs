using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC.Models
{
	public class Accommodation
	{
		//ID
		public int ID { get; set; }

		//Tip smestaja, po default je vila
		public AccommodationType AccommodationType { get; set; } = AccommodationType.Villa;

		//Naziv
		public string Name { get; set; }

		//Broj zvezdica
		public int Stars { get; set; }

		//Da li postoji bazen, po default je ne
		public bool Pool { get; set; } = false;

		//Da li postoji spa, po default je ne
		public bool Spa { get; set; } = false;

		//Da li prilagodjeno za invalide, po default je ne
		public bool Disability { get; set; } = false;

		//Da li postoji wifi, po default je ne
		public bool Wifi { get; set; } = false;

		//Lista idijeva tj lista smestajnih jedinica koji pripadaju ovom smestaju
		public List<int> AccommodationUnitID { get; set; } = new List<int>();

		//Da li je objekat obrisan, ako jeste ne prikazujemo ga na sajtu
		public bool IsRemoved { get; set; }

		public Accommodation()
		{
		}

		public Accommodation(int iD, AccommodationType accommodationType, string name, int stars, bool pool, bool spa, bool disability, bool wifi, 
			List<int> accommodationUnitID)
		{
			ID = iD;
			AccommodationType = accommodationType;
			Name = name;
			Stars = stars;
			Pool = pool;
			Spa = spa;
			Disability = disability;
			Wifi = wifi;
			AccommodationUnitID = accommodationUnitID;
			IsRemoved = false;
		}

		//Pogledaj kako je napisano u User.cs
		public bool Validate()
		{
			if (Name == null || Name == "" || Stars <= 0)
			{
				return false;
			}

			return true;
		}

		//Pozivamo metodu za generisanje random ID-ija
		public void GenerateID()
		{
			ID = GeneratorHelper.GenerisiRandomID();
		}
	}
}