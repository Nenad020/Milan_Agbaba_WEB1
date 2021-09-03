using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC.Models
{
	public class Arrangement
	{
		//ID
		public int ID { get; set; }

		//Naziv aranzmana
		public string Name { get; set; }

		//Tip aranzmana, po default je nocenje sa doruckom
		public ArrangementType ArrangementType { get; set; } = ArrangementType.NightWithBreakfast;

		//Tip prevoza, po default je bus
		public TransportationType TransportationType { get; set; } = TransportationType.Bus;

		//Destinacija
		public string Destination { get; set; }

		//Pocetak aranzmana
		public DateTime StartDateOfArrangement { get; set; }

		//Kraj aranzmana
		public DateTime EndDateOfArrangement { get; set; }

		//ID startne lokacije
		public int StartLocationID { get; set; }

		//Vreme okupljanja grupe
		public DateTime StartTime { get; set; }

		//Broj putnika
		public int NumberOfPassengers { get; set; }

		//Opis
		public string Description { get; set; }

		//Program putovanja
		public string TravelProgram { get; set; }

		//Slika aranzmana
		public string PosterOfArrangement { get; set; }

		//ID smestajne jedinice
		public int AccommodationID { get; set; }

		//Da li je objekat obrisan, ako jeste ne prikazujemo ga na sajtu
		public bool IsRemoved { get; set; } = false;

		public Arrangement()
		{
		}

		public Arrangement(int iD, string name, ArrangementType arrangementType, TransportationType transportationType, string destination,
			DateTime startDateOfArrangement, DateTime endDateOfArrangement, int startLocationID, DateTime startTime, int numberOfPassengers, string description, 
			string travelProgram, string posterOfArrangement, int accommodationID)
		{
			ID = iD;
			Name = name;
			ArrangementType = arrangementType;
			TransportationType = transportationType;
			Destination = destination;
			StartDateOfArrangement = startDateOfArrangement;
			EndDateOfArrangement = endDateOfArrangement;
			StartLocationID = startLocationID;
			StartTime = startTime;
			NumberOfPassengers = numberOfPassengers;
			Description = description;
			TravelProgram = travelProgram;
			PosterOfArrangement = posterOfArrangement;
			AccommodationID = accommodationID;
			IsRemoved = false;
		}

		//Pogledaj kako je napisano u User.cs
		public bool Validate()
		{
			if (Name == null || Name == "" || Destination == null || Destination == "" || StartDateOfArrangement == null || EndDateOfArrangement == null || 
				StartLocationID <= 0 || StartTime == null || NumberOfPassengers <= 0 || Description == null || Description == "" || TravelProgram == null || 
				TravelProgram == "" || AccommodationID <= 0)
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