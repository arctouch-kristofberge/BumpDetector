using System;
using System.Collections.Generic;
using BumpDetector.ViewModel;
using DeviceMotion.Plugin;
using DeviceMotion.Plugin.Abstractions;
using Xamarin.Forms;

namespace BumpDetector.View
{
	public partial class BumpDetailsPage : ContentPage
	{
		public BumpDetailsPage ()
		{
			InitializeComponent ();

            BindingContext = new BumpDetailsViewModel(SpeedList);
		}

		private void ButtonClicked(object sender, EventArgs e)
		{
			((BumpDetailsViewModel)BindingContext).StartBumping ();
		}
	}
}

