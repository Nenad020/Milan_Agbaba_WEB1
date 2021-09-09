using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC.Models
{
	//Ovi modeli su napravljeni zbog lakseg generisanja formi za pretragu
	//Kao sto imamo formu za registraciju tako i ovde imamo formu
	public class AccommodationSearchModel
	{
		public AccommodationSearchModel()
		{
		}

		public AccommodationSearchModel(AccommodationType? accommodationType, string name, bool? pool, bool? spa, bool? disability, bool? wifi)
		{
			AccommodationType = accommodationType;
			Name = name;
			Pool = pool;
			Spa = spa;
			Disability = disability;
			Wifi = wifi;
		}

		//Tip smestaja
		public AccommodationType? AccommodationType { get; set; } = null;

		//Naziv
		public string Name { get; set; } = null;

		//Ima li bazen
		public bool? Pool { get; set; } = null;

		//Da li spa
		public bool? Spa { get; set; } = null;

		//Da li je okrenuto invalidima
		public bool? Disability { get; set; } = null;

		//Da li wifi
		public bool? Wifi { get; set; } = null;
	}
}