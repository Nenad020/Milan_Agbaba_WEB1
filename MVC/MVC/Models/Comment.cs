using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC.Models
{
	public class Comment
	{
		//ID
		public int ID { get; set; }

		//Korisnicko ime turiste koji je ostavio komentar
		public string TouristUsername { get; set; }

		//ID aranzmana
		public int ArrangementID { get; set; }

		//Tekst
		public string Text { get; set; }

		//Ocena
		public int Grade { get; set; }

		//Oznaka da li je komentar odobren, ako jeste vidljiv je svima, 
		//ako nije onda je vidljiv samo menadzeru koji je kreiranu aranzman za koji se odnosi komentar
		public bool IsApproved { get; set; } = false;

		public Comment()
		{
		}

		public Comment(int iD, string touristUsername, int arrangementID, string text, int grade)
		{
			ID = iD;
			TouristUsername = touristUsername;
			ArrangementID = arrangementID;
			Text = text;
			Grade = grade;
			IsApproved = false;
		}

		//Pogledaj kako je napisano u User.cs
		public bool Validate()
		{
			if (Text == null || Text == "" || Grade <= 0 || Grade >= 6)
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