// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.cs" company="ArcTouch, Inc.">
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
//   Defines the App type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BumpDetector
{
    using System;
    using System.Diagnostics;

    using BumpDetector.View;

    using Xamarin.Forms;

    public class App : Application
    {
//#if DEBUG
//        public static readonly SignalRClient SignalRClient = new SignalRClient("http://10.211.55.5:49842/");
//#else
        public static readonly SignalRClient SignalRClient = new SignalRClient("http://bumpdetector.azurewebsites.net");
//#endif

        public App()
        {
            LocationManager = DependencyService.Get<ILocationManager>();
            BumpListener = new BumpListener();
            MainPage = new NavigationPage(new MainPage());
        }

        public ILocationManager LocationManager { get; set; }

        public BumpListener BumpListener { get; set; }

        protected override void OnStart()
        {
            App.SignalRClient.Start().ContinueWith(
                task =>
                    {
                        if (task.IsFaulted)
                        {
                            MainPage.DisplayAlert(
                                "Error",
                                "An error occurred when trying to connect to SignalR: "
                                + task.Exception.InnerExceptions[0].Message,
                                "OK");
                        }
                    });

            Device.StartTimer(
                TimeSpan.FromSeconds(10),
                () =>
                    {
                        if (!App.SignalRClient.IsConnectedOrConnecting)
                        {
                            App.SignalRClient.Start();
                        }

                        return true;
                    });
            App.SignalRClient.OnBumpDetected += (deviceId, message) => Debug.WriteLine($"{deviceId} - {message}");
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

