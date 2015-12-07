using System;
using Xamarin.Forms;
using BumpDetector.Model;
using PropertyChanged;
using System.Collections.ObjectModel;
using BumpDetector.Shared;

namespace BumpDetector.ViewModel
{
	[ImplementPropertyChanged]
	public class ShakeDetectorViewModel : BaseViewModel
	{
		public string ShakeStatus { get; set; }
		public string Time { get; set; }
		public BumpLocation Location { get; set; }
		 
		private string shakeStatusPhrase = "Shakes detected: ";
		private string timePhrase = "Timestamp: ";
		private int bumpsDetected = 0;

		public ShakeDetectorViewModel ()
		{
			UpdateShakeStatus();

            BumpListener.ClearAllListeners();
            BumpListener.OnBump += HandleBump;
            BumpListener.StartListeningForBumps();
		}

        public void HandleBump(object sender, MyArgs e)
		{
            BumpListener.ClearAllListeners();
            bumpsDetected++;
			UpdateShakeStatus ();
			UpdateTime ();
			RequestCurrentLocation ();
            BumpListener.OnBump += HandleBump;
		}

		private void UpdateShakeStatus()
		{
			ShakeStatus = shakeStatusPhrase + bumpsDetected;
		}

		private void UpdateTime()
		{
            Time = timePhrase + DateTime.Now.ToMiliSecondsSince1970();
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

