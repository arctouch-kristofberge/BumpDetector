namespace BumpDetector
{
    using System;

    using BumpDetector.View;

    using Xamarin.Forms;

    public class App : Application
    {
        public static readonly SignalRClient SignalRClient = new SignalRClient("http://bumpdetector.azurewebsites.net/");

        public App()
        {
            LocationManager = DependencyService.Get<ILocationManager>();
            // The root page of your application
            MainPage = new NavigationPage(new ShakeDetectorPage());
        }

        public ILocationManager LocationManager { get; set; }

        protected override void OnStart()
        {
            App.SignalRClient.Start().ContinueWith(
                task =>
                {
                    if (task.IsFaulted)
                    {
                        MainPage.DisplayAlert(
                            "Error",
                            "An error occurred when trying to connect to SignalR: " + task.Exception.InnerExceptions[0].Message,
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

