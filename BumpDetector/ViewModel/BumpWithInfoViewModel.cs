using System;
using BumpDetector.Model;
using BumpDetector.Shared;
using PropertyChanged;
using BumpDetector.CustomExceptions;
using Xamarin.Forms;

namespace BumpDetector.ViewModel
{
	[ImplementPropertyChanged]
	public class BumpWithInfoViewModel : BaseViewModel
	{
		public string BumpsStatus { get; set; }
        public string SkippedBumps { get; set; }
		public string Time { get; set; }
		public BumpLocation Location { get; set; }
		 
		private const string shakeStatusPhrase = "Shakes detected: ";
		private const string timePhrase = "Timestamp: ";
		private int bumpsDetected = 0;
        private int bumpsSkipped = 0;
        private double newBumpTimestamp;
        private double previousBumpTimestamp;

		public BumpWithInfoViewModel ()
		{
			UpdateBumpStatus();
            UpdateSkipped();

            BumpListener.OnBump -= HandleBump;
            BumpListener.OnBump += HandleBump;
            BumpListener.StartListeningForBumps();
		}

        public void HandleBump(object sender, BumpEventArgs e)
		{
            newBumpTimestamp = DateTime.Now.ToMiliSecondsSince1970();
            if(PreviousBumpHappenedLongEnoughAgo())
            {
                previousBumpTimestamp = newBumpTimestamp;
                bumpsDetected++;
                UpdateBumpStatus();
                UpdateTime();
                RequestCurrentLocation();
            }
            else
            {
                bumpsSkipped++;
                UpdateSkipped();
            }
		}

        bool PreviousBumpHappenedLongEnoughAgo()
        {
            return newBumpTimestamp - previousBumpTimestamp > Constants.TIME_BETWEEN_BUMPS;
        }

		private void UpdateBumpStatus()
		{
			BumpsStatus = shakeStatusPhrase + bumpsDetected;
		}

        void UpdateSkipped()
        {
            SkippedBumps = "Bumps skipped: " + bumpsSkipped;
        }

		private void UpdateTime()
		{
            Time = timePhrase + newBumpTimestamp;
		}

		private void RequestCurrentLocation()
		{
			try 
            {
                var locmgr = DependencyService.Get<ILocationManager>();
                locmgr.OnLocationAcquired += ShowLocation;
                locmgr.OnTimeOut += ShowTimeOutMessage;
                locmgr.RequestCurrentLocation (Constants.LOCATION_PROVIDER_TIMEOUT);
            } 
            catch (LocationServiceNotAvailablleException) 
            {
                BumpsStatus += " (location serive not running)";
            }
		}

        void ShowTimeOutMessage (object sender, EventArgs e)
        {
            BumpsStatus += " (location not found)";
        }

		private void ShowLocation(object sender, BumpLocation location)
		{
            ((ILocationManager)sender).OnLocationAcquired -= ShowLocation;
            if(location!=null)
            {
                this.Location = location;
            }
            else
            {
                BumpsStatus += " (location not found)";
            }
		}
	}
}

