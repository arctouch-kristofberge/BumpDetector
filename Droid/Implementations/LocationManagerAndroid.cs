using System;
using Xamarin.Forms;
using Android.App;
using Android.Locations;
using BumpDetector.Model;
using Android.Content;
using System.Collections.Generic;
using System.Linq;
using BumpDetector.CustomExceptions;

[assembly: Dependency(typeof(BumpDetector.Droid.LocationManagerAndroid))]
namespace BumpDetector.Droid
{
    public class LocationManagerAndroid : Activity, ILocationManager, ILocationListener
	{
        protected LocationManager manager;
        protected LocationProvider provider;

		#region ILocationManager implementation

		public event EventHandler<BumpLocation> OnLocationAcquired;

		public void RequestCurrentLocation ()
		{
            manager = (LocationManager) Android.App.Application.Context.GetSystemService (Context.LocationService);

            var criteriaForLocationService = new Criteria { Accuracy = Accuracy.Fine };

            IList<string> acceptableLocationProviders = manager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
                manager.RequestLocationUpdates (acceptableLocationProviders.First (), 0, 0, this);
            else
                throw new LocationServiceNotRunningException ();
		}

		#endregion

        public void OnLocationChanged(Location location)
        {
            manager.RemoveUpdates (this);
            if(OnLocationAcquired != null)
                OnLocationAcquired(this, new BumpLocation () { Latitude = location.Latitude, Longtitude = location.Longitude , Altitude = location.Altitude });
        }

        public void OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, Availability status, Android.OS.Bundle extras)
        {
            throw new NotImplementedException();
        }
	}
}

