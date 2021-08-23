using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC.Models
{
	public enum Role
	{
		Admin = 1,
		Manager = 2,
		Tourist = 3
	}

	public enum Gender
	{
		Male = 1,
		Female = 2
	}

	public enum ArrangementType
	{
		NightWithBreakfast = 1,
		HalfBoard = 2,
		FullBoard = 3,
		AllInclusive = 4,
		ApartmentRent = 5
	}

	public enum TransportationType
	{
		Bus = 1,
		Airplane = 2,
		BusAndAirplane = 3,
		ByYourself = 4,
		Other = 5
	}

	public enum AccommodationType
	{
		Hotel = 1,
		Motel = 2,
		Villa = 3
	}

	public enum ReservationStatus
	{
		Active = 1,
		Canceled = 2,
		Expired = 3
	}
}