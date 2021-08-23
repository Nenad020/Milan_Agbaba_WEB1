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
		public static string GetPath()
		{
			return @"D:\Projekti\Web_1\Projekti\Milan_Agbaba_WEB1\MVC\MVC\bin";
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
		public static void SaveReservations(List<Reservation> Reservations)
		{
			string path = GetPath() + "\\reservations.xml";

			XmlSerializer serializer = new XmlSerializer(typeof(List<Reservation>));
			StreamWriter writer = new StreamWriter(path);
			serializer.Serialize(writer, Reservations);
			writer.Close();
		}
	}
}