using System;
using System.Collections.Generic;

using Xamarin.Forms;

using BumpDetector.ViewModel;

namespace BumpDetector.View
{
    public partial class ShakeDetectorPage : ContentPage
    {
        public ShakeDetectorPage()
        {
            InitializeComponent();

            BindingContext = new ShakeDetectorViewModel();
        }
    }
}