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
using BumpDetector.Shared;
using BumpDetector.Droid.Shared;

[assembly: Dependency(typeof(BumpDetector.Droid.LocationManagerAndroid))]
namespace BumpDetector.Droid
{
    public class LocationManagerAndroid : Activity, ILocationManager, ILocationListener
	{
        protected LocationManager manager;
        protected Timer timeOutTimer;

		#region ILocationManager implementation

		public event EventHandler<BumpLocation> OnLocationAcquired;
        public event EventHandler<EventArgs> OnTimeOut;

        public void RequestCurrentLocation (double timeOut)
		{
            manager = (LocationManager) Android.App.Application.Context.GetSystemService (Context.LocationService);

            StartListeningForLocationUpdates();
            StartTimeOutTimer(timeOut);
		}
        
        void StartListeningForLocationUpdates()
        {
            if(manager.IsProviderEnabled(LocationManager.NetworkProvider))
            {
                manager.RequestSingleUpdate(LocationManager.NetworkProvider, this, null);
            }
            else if(manager.IsProviderEnabled(LocationManager.GpsProvider))
            {
                manager.RequestSingleUpdate(LocationManager.GpsProvider, this, null);
            }
            else
            {
                throw new LocationServiceNotAvailablleException();
            }
        }
        
        void StartTimeOutTimer(double timeOut)
        {
            timeOutTimer = new Timer(timeOut);
            timeOutTimer.Elapsed += TimeOutReached;
            timeOutTimer.Start();
        }

        #endregion

        void TimeOutReached (object sender, ElapsedEventArgs e)
        {
            StopUpdatesAndTimer();
            if(OnTimeOut!=null)
            {
                OnTimeOut(this, new EventArgs());
            }
        }

        void StopUpdatesAndTimer()
        {
            timeOutTimer.Stop();
            manager.RemoveUpdates(this);
        }

        #region ILocationListener Implementation

        public void OnLocationChanged(Location location)
        {
            StopUpdatesAndTimer();
            if(OnLocationAcquired != null)
            {
                OnLocationAcquired(this, CreateBumpLocation(location));
            }
        }

        static BumpLocation CreateBumpLocation(Location location)
        {
            return location!=null ? new BumpLocation() {
                Latitude = location.Latitude,
                Longtitude = location.Longitude,
                Altitude = location.Altitude }
                : null;
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

        #endregion
	}
}

