// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BumpWithInfoViewModel.cs" company="ArcTouch, Inc.">
//   All rights reserved.
//   
//   This file, its contents, concepts, methods, behavior, and operation
//   (collectively the "Software") are protected by trade secret, patent,
//   and copyright laws. The use of the Software is governed by a license
//   agreement. Disclosure of the Software to third parties, in any form,
//   in whole or in part, is expressly prohibited except as authorized by
//   the license agreement.
// </copyright>
// <summary>
//   Defines the BumpWithInfoViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BumpDetector.ViewModel
{
    using System;
    using System.Collections.ObjectModel;

    using BumpDetector.CustomExceptions;
    using BumpDetector.Model;
    using BumpDetector.Shared;

    using PropertyChanged;

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
            App.SignalRClient.OnBumpDetected += (id, message) =>
                {
                    Items.Add($"Matched with ID: {id} - {message}");
                };
        }

        public string BumpsStatus { get; set; }

        public string SkippedBumps { get; set; }

        public string Time { get; set; }

        public BumpLocation Location { get; set; }

        public int Id { get; } = new Random().Next(100);

        public ObservableCollection<string> Items { get; set; } = new ObservableCollection<string> { "This is a test" };

        public void HandleBump(object sender, MyArgs e)
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
                    App.SignalRClient.SendMessage(Id, Location.Latitude, Location.Longtitude, Location.Altitude, this.newBumpTimestamp);
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
                LocationManager.OnLocationAcquired += LocationReceived;
                LocationManager.RequestCurrentLocation();
            }
            catch (LocationServiceNotRunningException)
            {
                BumpsStatus += " (location service is not running)";
            }
        }

        private void LocationReceived(object sender, BumpLocation location)
        {
            LocationManager.OnLocationAcquired -= LocationReceived;
            Location = location;
        }
    }
}