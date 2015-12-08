using System;
using PropertyChanged;
using Xamarin.Forms;

namespace BumpDetector
{
	[ImplementPropertyChanged]
	public abstract class BaseViewModel
	{
		protected ILocationManager LocationManager
		{
			get 
            { 
                return ((App)Application.Current).LocationManager; 
            }
		}

		protected BumpListener BumpListener
		{
			get
			{
				return ((App)Application.Current).BumpListener;
			}
		}
	}
}

