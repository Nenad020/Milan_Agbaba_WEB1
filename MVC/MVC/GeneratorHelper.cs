using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC
{
	public class GeneratorHelper
	{
		//Generise random broj
		public static int GenerisiRandomID()
		{
			Random rand = new Random();
			return rand.Next(1, 999999);
		}
	}
}