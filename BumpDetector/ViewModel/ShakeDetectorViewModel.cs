using System;

using Xamarin.Forms;

using BumpDetector.Model;

using PropertyChanged;

namespace BumpDetector.ViewModel
{
    using System.Collections.ObjectModel;
    using System.Dynamic;

    [ImplementPropertyChanged]
    public class ShakeDetectorViewModel : BaseViewModel
    {
        private string _shakeStatusPhrase = "Shakes detected: ";

        private string _timePhrase = "Timestamp: ";

        private int shakesDetected = 0;

        public ShakeDetectorViewModel()
        {
            UpdateShakeStatus();
            this.Items = new ObservableCollection<string> { "Test: 12345" };

            MessagingCenter.Subscribe<object>(this, "Bump", HandleBump);
            App.SignalRClient.OnBumpDetected += (username, message) =>
                {
                    this.Items.Add($"{username} : {message}");
                };
        }

        public string ShakeStatus { get; set; }

        public string Time { get; set; }

        public BumpLocation Location { get; set; }

        public ObservableCollection<string> Items { get; set; }

        public void HandleBump(object sender)
        {
            this.shakesDetected++;
            UpdateShakeStatus();
            UpdateTime();
            RequestCurrentLocation();
            if (App.SignalRClient.IsConnectedOrConnecting && Location != null)
            {
                App.SignalRClient.SendMessage(new Random().Next(), Location.Latitude, Location.Longtitude, Location.Altitude, GetTimeSince1970());
            }
        }

        private void UpdateShakeStatus()
        {
            ShakeStatus = this._shakeStatusPhrase + this.shakesDetected;
        }

        private void UpdateTime()
        {
            Time = this._timePhrase + GetTimeSince1970();
        }

        private double GetTimeSince1970()
        {
            return
                Math.Round(
                    DateTime.Now.ToUniversalTime()
                        .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                        .TotalMilliseconds);
        }

        private void RequestCurrentLocation()
        {
            LocationManager.OnLocationAcquired += LocationReceived;
            LocationManager.RequestCurrentLocation();
        }

        private void LocationReceived(object sender, BumpLocation location)
        {
            LocationManager.OnLocationAcquired -= LocationReceived;
            Location = location;
        }
    }
}

