using System;

using BumpDetector.Model;
using BumpDetector.Shared;

using PropertyChanged;

using BumpDetector.CustomExceptions;

using Xamarin.Forms;

namespace BumpDetector.ViewModel
{
    using System.Collections.ObjectModel;

    [ImplementPropertyChanged]
    public class BumpWithInfoViewModel : BaseViewModel
    {
        private const string SHAKE_STATUS_PHRASE = "Shakes detected: ";

        private const string TIME_PHRASE = "Timestamp: ";

        private int bumpsDetected = 0;

        private int bumpsSkipped = 0;

        private double newBumpTimestamp;

        private double previousBumpTimestamp;

        public BumpWithInfoViewModel()
        {
            UpdateBumpStatus();
            UpdateSkipped();

            BumpListener.OnBump -= HandleBump;
            BumpListener.OnBump += HandleBump;
            BumpListener.StartListeningForBumps();
            App.SignalRClient.OnBumpDetected += (id, message) => { Items.Add($"{id} - {message}"); };
        }

        public string BumpsStatus { get; set; }

        public string SkippedBumps { get; set; }

        public string Time { get; set; }

        public BumpLocation Location { get; set; }

        public int Id { get; } = new Random().Next(100);

        public ObservableCollection<string> Items { get; set; } = new ObservableCollection<string> { "This is a test" };

        public void HandleBump(object sender, BumpEventArgs e)
        {
            this.newBumpTimestamp = DateTime.Now.ToMiliSecondsSince1970();
            if (PreviousBumpHappenedLongEnoughAgo())
            {
                this.previousBumpTimestamp = this.newBumpTimestamp;
                this.bumpsDetected++;
                UpdateBumpStatus();
                UpdateTime();
                RequestCurrentLocation();
                if (App.SignalRClient.IsConnectedOrConnecting && Location != null)
                {
                    App.SignalRClient.SendMessage(
                        Id,
                        Location.Latitude,
                        Location.Longtitude,
                        Location.Altitude,
                        this.newBumpTimestamp);
                }
            }
            else
            {
                this.bumpsSkipped++;
                UpdateSkipped();
            }
        }

        private bool PreviousBumpHappenedLongEnoughAgo()
        {
            return this.newBumpTimestamp - this.previousBumpTimestamp > Constants.TIME_BETWEEN_BUMPS;
        }

        private void UpdateBumpStatus()
        {
            BumpsStatus = SHAKE_STATUS_PHRASE + this.bumpsDetected;
        }

        private void UpdateSkipped()
        {
            SkippedBumps = "Bumps skipped: " + this.bumpsSkipped;
        }

        private void UpdateTime()
        {
            Time = TIME_PHRASE + this.newBumpTimestamp;
        }

        private void RequestCurrentLocation()
        {
            try
            {
                ILocationManager locmgr = DependencyService.Get<ILocationManager>();
                locmgr.OnLocationAcquired += ShowLocation;
                locmgr.OnTimeOut += ShowTimeOutMessage;
                locmgr.RequestCurrentLocation(Constants.LOCATION_PROVIDER_TIMEOUT);
            }
            catch (LocationServiceNotAvailablleException)
            {
                BumpsStatus += " (location service is not running)";
            }
        }

        private void ShowTimeOutMessage(object sender, EventArgs e)
        {
            BumpsStatus += " (location not found)";
        }

        private void ShowLocation(object sender, BumpLocation location)
        {
            ((ILocationManager)sender).OnLocationAcquired -= ShowLocation;
            if (location != null)
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