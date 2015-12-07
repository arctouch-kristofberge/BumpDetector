using System;
using System.Collections.Generic;
using BumpDetector.ViewModel;
using DeviceMotion.Plugin;
using DeviceMotion.Plugin.Abstractions;
using Xamarin.Forms;

namespace BumpDetector.View
{
	public partial class BumpDetectorPage : ContentPage
	{
		public BumpDetectorPage ()
		{
			InitializeComponent ();

            BindingContext = new BumpDetectorViewModel(SpeedList);
		}

		private void ButtonClicked(object sender, EventArgs e)
		{
			((BumpDetectorViewModel)BindingContext).StartBumping ();
		}
	}
}

