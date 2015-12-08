using System;

using PropertyChanged;
using Xamarin.Forms;

namespace BumpDetector.ViewModel
{
	[ImplementPropertyChanged]
	public class BumpDetailsViewModel : BaseViewModel
	{
		public string BumpsStatus { get; set; }

		private int bumpsDetected = 0;
        private StackLayout SpeedList;

        public BumpDetailsViewModel(StackLayout speedList)
        {
            UpdateBumpsStatus ();
            this.SpeedList = speedList;

        }

		public void StartBumping()
		{
            BumpListener.OnBump -= BumpDetected;
            BumpListener.OnBump += BumpDetected;
            BumpListener.OnHighSpeedDetected -= HighSpeedDetected;
            BumpListener.OnHighSpeedDetected += HighSpeedDetected;
            BumpListener.OnSlowDownAfterHighSpeed -= SlowDownAfterHighSpeed;
            BumpListener.OnSlowDownAfterHighSpeed += SlowDownAfterHighSpeed;
            BumpListener.OnFastSpeedEnded -= FastSpeedEnded;
            BumpListener.OnFastSpeedEnded += FastSpeedEnded;
			BumpListener.StartListeningForBumps ();
		}

        void FastSpeedEnded (object sender, MyArgs e)
        {
            SpeedList.Children.Add(new Label(){ Text = "High speed end " + e.Value });
        }

        void SlowDownAfterHighSpeed (object sender, MyArgs e)
        {
            SpeedList.Children.Add(new Label(){ Text = "Slow speed " + e.Value });
        }

		private void UpdateBumpsStatus()
		{
			BumpsStatus = "Bumps detected: " + bumpsDetected;
		}

        void HighSpeedDetected (object sender, MyArgs e)
		{
            SpeedList.Children.Add(new Label(){ Text = "High speed " + e.Value });
		}

        private void BumpDetected (object sender, MyArgs e)
		{
			bumpsDetected++;
            SpeedList.Children.Add(new Label(){ Text = "BUMP " + e.Value });
			UpdateBumpsStatus ();
		}
	}
}
