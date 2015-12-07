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
			BumpListener = new BumpListener ();
			// The root page of your application
            MainPage = new NavigationPage( new MainPage ());
		}

		public ILocationManager LocationManager { get; set; }
		public BumpListener BumpListener { get; set; }

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

