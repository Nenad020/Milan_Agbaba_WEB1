using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC.Models
{
	//Ovi modeli su napravljeni zbog lakseg generisanja formi za pretragu
	//Kao sto imamo formu za registraciju tako i ovde imamo formu
	public class ArrangementSearchModel
	{
		public ArrangementSearchModel()
		{
		}

		public ArrangementSearchModel(DateTime startTimeMin, DateTime startTimeMax, DateTime endTimeMin, DateTime endTimeMax, ArrangementType arrangementType, TransportationType transportationType, string location)
		{
			StartTimeMin = startTimeMin;
			StartTimeMax = startTimeMax;
			EndTimeMin = endTimeMin;
			EndTimeMax = endTimeMax;
			ArrangementType = arrangementType;
			TransportationType = transportationType;
			Location = location;
		}

		//Minimalno pocetno vreme
		public DateTime? StartTimeMin { get; set; } = null;

		//Maksimalno pocetno vreme
		public DateTime? StartTimeMax { get; set; } = null;

		//Minimalno krajnje vreme
		public DateTime? EndTimeMin { get; set; } = null;

		//Maksimalno krajnje vreme
		public DateTime? EndTimeMax { get; set; } = null;

		//Tip aranzmana
		public ArrangementType? ArrangementType { get; set; } = null;

		//Tip transporta
		public TransportationType? TransportationType { get; set; } = null;

		//Naziv destinacije
		public string Location { get; set; }
	}
}