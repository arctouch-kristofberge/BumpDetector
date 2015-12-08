// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShakeDetectorViewModel.cs" company="ArcTouch, Inc.">
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
//   Defines the ShakeDetectorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BumpDetector.ViewModel
{
    using System;
    using System.Collections.ObjectModel;

    using BumpDetector.Model;
    using BumpDetector.Shared;

    using PropertyChanged;

    [ImplementPropertyChanged]
    public class ShakeDetectorViewModel : BaseViewModel
    {
        private string shakeStatusPhrase = "Shakes detected: ";

        private string timePhrase = "Timestamp: ";

        private int bumpsDetected = 0;

        public ShakeDetectorViewModel()
        {
            UpdateShakeStatus();
            BumpListener.ClearAllListeners();
            BumpListener.OnBump += HandleBump;
            BumpListener.StartListeningForBumps();
            this.Items = new ObservableCollection<string> { "Test: 12345" };

            App.SignalRClient.OnBumpDetected += (username, message) =>
                {
                    this.Items.Add($"{username} : {message}");
                };
        }

        public string ShakeStatus { get; set; }

        public string Time { get; set; }

        public BumpLocation Location { get; set; }

        public ObservableCollection<string> Items { get; set; }

        public void HandleBump(object sender, MyArgs e)
        {
            BumpListener.ClearAllListeners();
            this.bumpsDetected++;
            UpdateShakeStatus();
            UpdateTime();
            RequestCurrentLocation();
            BumpListener.OnBump += HandleBump;
            if (App.SignalRClient.IsConnectedOrConnecting && Location != null)
            {
                App.SignalRClient.SendMessage(new Random().Next(), Location.Latitude, Location.Longtitude, Location.Altitude, DateTime.Now.ToMiliSecondsSince1970());
            }
        }

        private void UpdateShakeStatus()
        {
            ShakeStatus = this.shakeStatusPhrase + this.bumpsDetected;
        }

        private void UpdateTime()
        {
            Time = this.timePhrase + DateTime.Now.ToMiliSecondsSince1970();
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

