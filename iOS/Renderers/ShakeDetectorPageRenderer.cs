using System;
using BumpDetector;
using BumpDetector.Model;
using BumpDetector.View;
using BumpDetector.ViewModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using BumpDetector.Shared;

[assembly: ExportRenderer(typeof(ShakeDetectorPage), typeof(BumpDetector.iOS.ShakeDetectorPageRenderer))]
namespace BumpDetector.iOS
{
	public class ShakeDetectorPageRenderer : PageRenderer
	{
		public string Test { get; set; } = "blabla";

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
				MessagingCenter.Send (this as object, Constants.BUMP_MESSAGE);
			}
		}
	}
}

