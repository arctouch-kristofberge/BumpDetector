using System;

using Xamarin.Forms;
using BumpDetector.View;

namespace BumpDetector
{
	public class App : Application
	{
		public App ()
		{
			LocationManager = DependencyService.Get<ILocationManager> ();
			// The root page of your application
			MainPage = new NavigationPage( new ShakeDetectorPage ());
		}

		public ILocationManager LocationManager { get; set; }

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

