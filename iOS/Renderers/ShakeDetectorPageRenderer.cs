using BumpDetector.View;

using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using BumpDetector.Shared;

[assembly: ExportRenderer(typeof(BumpWithInfoPage), typeof(BumpDetector.iOS.ShakeDetectorPageRenderer))]

namespace BumpDetector.iOS
{
    public class ShakeDetectorPageRenderer : PageRenderer
    {
        public string Test { get; set; } = "blabla";

        public override bool CanBecomeFirstResponder => true;

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            BecomeFirstResponder();
        }

        public override void MotionEnded(UIEventSubtype motion, UIEvent evt)
        {
            if (motion == UIEventSubtype.MotionShake)
            {
                MessagingCenter.Send(this as object, Constants.BUMP_MESSAGE);
            }
        }
    }
}

