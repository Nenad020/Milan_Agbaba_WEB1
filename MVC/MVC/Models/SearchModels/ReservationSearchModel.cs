using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC.Models
{
	//Ovi modeli su napravljeni zbog lakseg generisanja formi za pretragu
	//Kao sto imamo formu za registraciju tako i ovde imamo formu
	public class ReservationSearchModel
	{
		public ReservationSearchModel()
		{
		}

		public ReservationSearchModel(ReservationStatus? reservationStatus, string arrangementName)
		{
			ReservationStatus = reservationStatus;
			ArrangementName = arrangementName;
		}

		//Status rezervacije
		public ReservationStatus? ReservationStatus { get; set; } = null;

		//Naziv aranzmana
		public string ArrangementName { get; set; }
	}
}