using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC.Models
{
	//Ovi modeli su napravljeni zbog lakseg generisanja formi za pretragu
	//Kao sto imamo formu za registraciju tako i ovde imamo formu
	public class AccommodationUnitSearchModel
	{
		public AccommodationUnitSearchModel()
		{
		}

		public AccommodationUnitSearchModel(int? guestsMin, int? guestsMax, bool? pets, int? price)
		{
			GuestsMin = guestsMin;
			GuestsMax = guestsMax;
			Pets = pets;
			Price = price;
		}

		//Minimalan broj gostiju
		public int? GuestsMin { get; set; } = null;

		//Maksimalan broj gostiju
		public int? GuestsMax { get; set; } = null;

		//Da li su dozvoljeni ljubimci
		public bool? Pets { get; set; } = null;

		//Cena smestaja (maksmimalna cena)
		public int? Price { get; set; } = null;
	}
}