using System;
using System.Timers;

using Xamarin.Forms;

using Android.App;
using Android.Content;
using Android.Locations;

using BumpDetector.CustomExceptions;
using BumpDetector.Model;

[assembly: Dependency(typeof(BumpDetector.Droid.LocationManagerAndroid))]

namespace BumpDetector.Droid
{
    public class LocationManagerAndroid : Activity, ILocationManager, ILocationListener
    {
        private LocationManager manager;

        private Timer timeOutTimer;

        #region ILocationManager implementation

        public event EventHandler<BumpLocation> OnLocationAcquired;

        public event EventHandler<EventArgs> OnTimeOut;

        public void RequestCurrentLocation(double timeOut)
        {
            this.manager = (LocationManager)Android.App.Application.Context.GetSystemService(Context.LocationService);

            StartListeningForLocationUpdates();
            StartTimeOutTimer(timeOut);
        }

        private void StartListeningForLocationUpdates()
        {
            if (this.manager.IsProviderEnabled(LocationManager.NetworkProvider))
            {
                this.manager.RequestSingleUpdate(LocationManager.NetworkProvider, this, null);
            }
            else if (this.manager.IsProviderEnabled(LocationManager.GpsProvider))
            {
                this.manager.RequestSingleUpdate(LocationManager.GpsProvider, this, null);
            }
            else
            {
                throw new LocationServiceNotAvailablleException();
            }
        }

        private void StartTimeOutTimer(double timeOut)
        {
            this.timeOutTimer = new Timer(timeOut);
            this.timeOutTimer.Elapsed += TimeOutReached;
            this.timeOutTimer.Start();
        }

        #endregion

        private void TimeOutReached(object sender, ElapsedEventArgs e)
        {
            StopUpdatesAndTimer();
            OnTimeOut?.Invoke(this, new EventArgs());
        }

        private void StopUpdatesAndTimer()
        {
            this.timeOutTimer.Stop();
            this.manager.RemoveUpdates(this);
        }

        #region ILocationListener Implementation

        public void OnLocationChanged(Location location)
        {
            StopUpdatesAndTimer();
            OnLocationAcquired?.Invoke(this, CreateBumpLocation(location));
        }

        private static BumpLocation CreateBumpLocation(Location location)
        {
            return location != null
                       ? new BumpLocation {
                                 Latitude = location.Latitude,
                                 Longtitude = location.Longitude,
                                 Altitude = location.Altitude
                             }
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
