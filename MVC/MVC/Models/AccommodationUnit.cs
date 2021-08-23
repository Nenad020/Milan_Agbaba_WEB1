using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC.Models
{
	public class AccommodationUnit
	{
		//ID
		public int ID { get; set; }

		//Broj slobodnih soba
		public int NumberOfFreeRooms { get; set; }

		//Broj ukupnih soba
		public int NumberOfTotalRooms { get; set; }

		//Da li su ljubimci dozvoljeni, po default je ne
		public bool Pets { get; set; } = false;

		//Cena smestaja
		public int Price { get; set; }

		//Da li je objekat obrisan, ako jeste ne prikazujemo ga na sajtu
		public bool IsRemoved { get; set; } = false;

		public AccommodationUnit()
		{
		}

		public AccommodationUnit(int iD, int numberOfFreeRooms, int numberOfTotalRooms, bool pets, int price)
		{
			ID = iD;
			NumberOfFreeRooms = numberOfFreeRooms;
			NumberOfTotalRooms = numberOfTotalRooms;
			Pets = pets;
			Price = price;
			IsRemoved = false;
		}

		//Pogledaj kako je napisano u User.cs
		public bool Validate()
		{
			if (NumberOfFreeRooms <= 0 || Price <= 0 || NumberOfTotalRooms <= 0)
			{
				return false;
			}

			return true;
		}
	}
}