using MVC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MVC
{
	public class XMLHelper
	{
		//Vraca putanju do xml fajlova
		//TODO: PROMENI PUTANJU KAD POKRECES PROJEKAT SA DRUGOG RACUNARA
		private static string GetPath()
		{
			return @"D:\Za Faks\Projekti za Faks\Web1\Milan_Agbaba_WEB1\MVC";
		}

		//Otvara xml fajl gde se nalaze korisnici i vraca listu
		public static List<User> LoadUsers()
		{
			List<User> users;
			string path = GetPath() + "\\users.xml";

			XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
			TextReader reader = new StreamReader(path);
			users = (List<User>)serializer.Deserialize(reader);
			reader.Close();

			return users;
		}

		//Upisuje listu korisnika u xml fajl
		public static void SaveUsers(List<User> users)
		{
			string path = GetPath() + "\\users.xml";

			XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
			StreamWriter writer = new StreamWriter(path);
			serializer.Serialize(writer, users);
			writer.Close();
		}

		//Pogledaj za korisnike
		public static List<StartLocation> LoadStartLocations()
		{
			List<StartLocation> startLocations;
			string path = GetPath() + "\\startlocations.xml";

			XmlSerializer serializer = new XmlSerializer(typeof(List<StartLocation>));
			TextReader reader = new StreamReader(path);
			startLocations = (List<StartLocation>)serializer.Deserialize(reader);
			reader.Close();

			return startLocations;
		}

		//Pogledaj za korisnike
		public static void SaveStartLocations(List<StartLocation> startLocations)
		{
			string path = GetPath() + "\\startlocations.xml";

			XmlSerializer serializer = new XmlSerializer(typeof(List<StartLocation>));
			StreamWriter writer = new StreamWriter(path);
			serializer.Serialize(writer, startLocations);
			writer.Close();
		}

		//Pogledaj za korisnike
		public static List<Reservation> LoadReservations()
		{
			List<Reservation> reservations;
			string path = GetPath() + "\\reservations.xml";

			XmlSerializer serializer = new XmlSerializer(typeof(List<Reservation>));
			TextReader reader = new StreamReader(path);
			reservations = (List<Reservation>)serializer.Deserialize(reader);
			reader.Close();

			return reservations;
		}

		//Pogledaj za korisnike
		public static void SaveReservations(List<Reservation> reservations)
		{
			string path = GetPath() + "\\reservations.xml";

			XmlSerializer serializer = new XmlSerializer(typeof(List<Reservation>));
			StreamWriter writer = new StreamWriter(path);
			serializer.Serialize(writer, reservations);
			writer.Close();
		}

		//Pogledaj za korisnike
		public static List<Comment> LoadComments()
		{
			List<Comment> comments;
			string path = GetPath() + "\\comments.xml";

			XmlSerializer serializer = new XmlSerializer(typeof(List<Comment>));
			TextReader reader = new StreamReader(path);
			comments = (List<Comment>)serializer.Deserialize(reader);
			reader.Close();

			return comments;
		}

		//Pogledaj za korisnike
		public static void SaveComments(List<Comment> comments)
		{
			string path = GetPath() + "\\comments.xml";

			XmlSerializer serializer = new XmlSerializer(typeof(List<Comment>));
			StreamWriter writer = new StreamWriter(path);
			serializer.Serialize(writer, comments);
			writer.Close();
		}

		//Pogledaj za korisnike
		public static List<Arrangement> LoadArrangements()
		{
			List<Arrangement> arrangements;
			string path = GetPath() + "\\arrangements.xml";

			XmlSerializer serializer = new XmlSerializer(typeof(List<Arrangement>));
			TextReader reader = new StreamReader(path);
			arrangements = (List<Arrangement>)serializer.Deserialize(reader);
			reader.Close();

			return arrangements;
		}

		//Pogledaj za korisnike
		public static void SaveArrangements(List<Arrangement> arrangements)
		{
			string path = GetPath() + "\\arrangements.xml";

			XmlSerializer serializer = new XmlSerializer(typeof(List<Arrangement>));
			StreamWriter writer = new StreamWriter(path);
			serializer.Serialize(writer, arrangements);
			writer.Close();
		}

		//Pogledaj za korisnike
		public static List<AccommodationUnit> LoadAccommodationUnits()
		{
			List<AccommodationUnit> accommodationUnits;
			string path = GetPath() + "\\accommodationUnits.xml";

			XmlSerializer serializer = new XmlSerializer(typeof(List<AccommodationUnit>));
			TextReader reader = new StreamReader(path);
			accommodationUnits = (List<AccommodationUnit>)serializer.Deserialize(reader);
			reader.Close();

			return accommodationUnits;
		}

		//Pogledaj za korisnike
		public static void SaveAccommodationUnits(List<AccommodationUnit> accommodationUnits)
		{
			string path = GetPath() + "\\accommodationUnits.xml";

			XmlSerializer serializer = new XmlSerializer(typeof(List<AccommodationUnit>));
			StreamWriter writer = new StreamWriter(path);
			serializer.Serialize(writer, accommodationUnits);
			writer.Close();
		}

		//Pogledaj za korisnike
		public static List<Accommodation> LoadAccommodations()
		{
			List<Accommodation> accommodations;
			string path = GetPath() + "\\accommodations.xml";

			XmlSerializer serializer = new XmlSerializer(typeof(List<Accommodation>));
			TextReader reader = new StreamReader(path);
			accommodations = (List<Accommodation>)serializer.Deserialize(reader);
			reader.Close();

			return accommodations;
		}

		//Pogledaj za korisnike
		public static void SaveAccommodations(List<Accommodation> accommodations)
		{
			string path = GetPath() + "\\accommodations.xml";

			XmlSerializer serializer = new XmlSerializer(typeof(List<Accommodation>));
			StreamWriter writer = new StreamWriter(path);
			serializer.Serialize(writer, accommodations);
			writer.Close();
		}
	}
}