using System;
using CoreLocation;
using BumpDetector.Model;

namespace BumpDetector.iOS.Extensions
{
	public static class LocationExtensions
	{
		public static BumpLocation ToMyLocation(this CLLocationsUpdatedEventArgs e)
		{
			double lat = e.Locations [e.Locations.Length - 1].Coordinate.Latitude;
			double lng = e.Locations [e.Locations.Length - 1].Coordinate.Longitude;
			double alt = e.Locations [e.Locations.Length - 1].Altitude;
			var location = new BumpLocation () {
				Latitude = lat,
				Longtitude = lng,
				Altitude = alt
			};
			return location;
		}
	}
}

