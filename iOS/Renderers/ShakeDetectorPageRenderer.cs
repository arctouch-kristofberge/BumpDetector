using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using BumpDetector;
using UIKit;
using BumpDetector.ViewModel;
using BumpDetector.Model;
using BumpDetector.View;

[assembly: ExportRenderer(typeof(ShakeDetectorPage), typeof(BumpDetector.iOS.ShakeDetectorPageRenderer))]
namespace BumpDetector.iOS
{
	public class ShakeDetectorPageRenderer : PageRenderer
	{
		private Action<BumpData> _callBack;

		protected override void OnElementChanged (VisualElementChangedEventArgs e)
		{
			base.OnElementChanged (e);
			_callBack = ((ShakeDetectorViewModel)((ShakeDetectorPage)e.NewElement).BindingContext).HandleBump;
		}

		public override bool CanBecomeFirstResponder {
			get
			{
				return true;
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			BecomeFirstResponder ();
		}

		public override void MotionEnded (UIKit.UIEventSubtype motion, UIKit.UIEvent evt)
		{
			if (motion == UIEventSubtype.MotionShake)
			{
				MessagingCenter.Send (this as object, "Bump");
			}
		}

		private BumpData CreateBumpEventArguments(UIEvent evt)
		{
			return new BumpData () { Timestamp = evt.Timestamp };
		}
	}
}

