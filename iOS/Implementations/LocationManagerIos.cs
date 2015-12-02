using System;
using BumpDetector.CustomExceptions;
using BumpDetector.iOS.Extensions;
using BumpDetector.Model;
using CoreLocation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(BumpDetector.iOS.LocationManagerIos))]
namespace BumpDetector.iOS
{
	public class LocationManagerIos : ILocationManager
	{
		protected CLLocationManager manager;
		public LocationManagerIos ()
		{
			this.manager = new CLLocationManager();
			manager.AllowsBackgroundLocationUpdates = true;

			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0))
				manager.RequestAlwaysAuthorization ();
		}

		#region ILocationManager implementation
		public event EventHandler<BumpLocation> OnLocationAcquired;
		
		public void RequestCurrentLocation ()
		{
			if (CLLocationManager.LocationServicesEnabled) {
				manager.DesiredAccuracy = 1;
				manager.LocationsUpdated += LocationsUpdated;

				manager.RequestLocation ();
			} 
			else 
			{
				throw new LocationServiceNotRunningException ();
			}
		}
		#endregion


		
		private void LocationsUpdated(object sender, CLLocationsUpdatedEventArgs e)
		{
			Unsubscribe ();
			
			if(OnLocationAcquired!=null)
				OnLocationAcquired(this, e.ToMyLocation());
		}
		
		private void Unsubscribe ()
		{
			manager.LocationsUpdated -= LocationsUpdated;
			manager.StopUpdatingLocation ();
		}
	}
}

