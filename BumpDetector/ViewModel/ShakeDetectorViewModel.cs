using System;
using Xamarin.Forms;
using BumpDetector.Model;
using PropertyChanged;
using System.Collections.ObjectModel;

namespace BumpDetector.ViewModel
{
	[ImplementPropertyChanged]
	public class ShakeDetectorViewModel : BaseViewModel
	{
		public string ShakeStatus { get; set; }
		public string Time { get; set; }
		public BumpLocation Location { get; set; }
		 
		private string _shakeStatusPhrase = "Shakes detected: ";
		private string _timePhrase = "Timestamp: ";
		private int _shakesDetected = 0;

		public ShakeDetectorViewModel ()
		{
			UpdateShakeStatus();

			MessagingCenter.Subscribe<object> (this, "Bump", HandleBump);
		}

		public void HandleBump(object sender)
		{
			_shakesDetected++;
			UpdateShakeStatus ();
			UpdateTime ();
			RequestCurrentLocation ();
		}

		private void UpdateShakeStatus()
		{
			ShakeStatus = _shakeStatusPhrase + _shakesDetected;
		}

		private void UpdateTime()
		{
			Time = _timePhrase + GetTimeSince1970 ();
		}
		
		private double GetTimeSince1970 ()
		{
			return Math.Round (DateTime.Now.ToUniversalTime ().Subtract (new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
		}

		private void RequestCurrentLocation()
		{
			LocationManager.OnLocationAcquired += LocationReceived;
			LocationManager.RequestCurrentLocation ();
		}

		private void LocationReceived(object sender, BumpLocation location)
		{
			LocationManager.OnLocationAcquired -= LocationReceived;
			Location = location;
		}
	}
}

