using System;
using Xamarin.Forms;
using Android.App;
using Android.Locations;
using BumpDetector.Model;
using Android.Content;
using System.Collections.Generic;
using System.Linq;
using BumpDetector.CustomExceptions;
using System.Timers;

[assembly: Dependency(typeof(BumpDetector.Droid.LocationManagerAndroid))]
namespace BumpDetector.Droid
{
    public class LocationManagerAndroid : Activity, ILocationManager, ILocationListener
	{
        protected LocationManager manager;
        protected LocationProvider provider;
        protected IList<string> acceptableLocationProviders;
        protected Criteria criteria;
        protected string chosenProvider;

        protected Timer timeOutTimer;

		#region ILocationManager implementation

		public event EventHandler<BumpLocation> OnLocationAcquired;

		public void RequestCurrentLocation ()
		{
            manager = (LocationManager) Android.App.Application.Context.GetSystemService (Context.LocationService);
            criteria = new Criteria { Accuracy = Accuracy.Fine };
            chosenProvider = manager.GetBestProvider(criteria, true);

            StartListeningForLocationUpdates();
		}

		#endregion

        void StartListeningForLocationUpdates()
        {
            if(!string.IsNullOrEmpty(chosenProvider))
            {
                manager.RequestLocationUpdates(chosenProvider, 0, 0, this);
                StartTimeOutTimer();
            }
            else
            {
                throw new LocationServiceNotAvailablleException();
            }
        }

        void StartTimeOutTimer()
        {
            timeOutTimer = new Timer();
            timeOutTimer.Interval = 2000;
            timeOutTimer.Elapsed += TimeOutReached;
            timeOutTimer.Start();
        }

        void TimeOutReached (object sender, ElapsedEventArgs e)
        {
            timeOutTimer.Stop();
            TryDifferentProvider();
        }

        void TryDifferentProvider()
        {
            
            acceptableLocationProviders = manager.GetProviders(criteria, true);
            acceptableLocationProviders.Remove(chosenProvider);
            chosenProvider = acceptableLocationProviders.FirstOrDefault();

            try
            {
                StartListeningForLocationUpdates();
            }
            catch(LocationServiceNotAvailablleException)
            {
                throw new LocationNotFoundException();
            }
        }

        public void OnLocationChanged(Location location)
        {
            timeOutTimer.Stop();
            manager.RemoveUpdates (this);
            if(OnLocationAcquired != null)
                OnLocationAcquired(this, new BumpLocation () { Latitude = location.Latitude, Longtitude = location.Longitude , Altitude = location.Altitude });
        }

        public void OnProviderDisabled(string provider)
        {
            
        }

        public void OnProviderEnabled(string provider)
        {
            
        }

        public void OnStatusChanged(string provider, Availability status, Android.OS.Bundle extras)
        {
            
        }
	}
}

