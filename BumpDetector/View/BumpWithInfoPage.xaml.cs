using System;
using System.Collections.Generic;

using Xamarin.Forms;
using BumpDetector.ViewModel;
using Xamarin.Forms.Xaml;

namespace BumpDetector.View
{
	public partial class BumpWithInfoPage : ContentPage
	{
		public BumpWithInfoPage ()
		{
			InitializeComponent ();

			BindingContext = new BumpWithInfoViewModel ();
		}
	}
}
