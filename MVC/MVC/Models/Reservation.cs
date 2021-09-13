using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC.Models
{
	public class Reservation
	{
		//ID
		public string ID { get; set; }

		//Korisnicko ime turiste koji je izvrsio rezervaciju
		public string TouristUsername { get; set; }

		//Status rezervacije, po defaultu je aktivna
		public ReservationStatus ReservationStatus { get; set; } = ReservationStatus.Active;

		//ID aranzmana
		public int ArrangementID { get; set; }

		//ID smestajne jedinice
		public int AccommodationUnitID { get; set; }

		//Da li je objekat obrisan, ako jeste ne prikazujemo ga na sajtu
		public bool IsRemoved { get; set; } = false;

		public Reservation()
		{
		}

		public Reservation(string iD, string touristUsername, ReservationStatus reservationStatus, int arrangementID, int accommodationUnitID)
		{
			ID = iD;
			TouristUsername = touristUsername;
			ReservationStatus = reservationStatus;
			ArrangementID = arrangementID;
			AccommodationUnitID = accommodationUnitID;
			IsRemoved = false;
		}

		//Pogledaj kako je napisano u User.cs
		public bool Validate()
		{
			if (AccommodationUnitID <= 0)
			{
				return false;
			}

			return true;
		}

		//Pozivamo metodu za generisanje random ID-ija
		public void GenerateID()
		{
			int id = GeneratorHelper.GenerisiRandomID();
			ID = "Reservation_" + id;
		}
	}
}