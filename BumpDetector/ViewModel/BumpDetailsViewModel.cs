using System;

using PropertyChanged;
using Xamarin.Forms;
using BumpDetector.Shared;

namespace BumpDetector.ViewModel
{
	[ImplementPropertyChanged]
	public class BumpDetailsViewModel : BaseViewModel
	{
		public string BumpsStatus { get; set; }

		private int bumpsDetected = 0;
        private StackLayout SpeedList;


        private double newBumpTimestamp;
        private double previousBumpTimestamp;

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
			BumpListener.StartListeningForBumps ();
		}

        void SlowDownAfterHighSpeed (object sender, BumpEventArgs e)
        {
            SpeedList.Children.Add(new Label(){ Text = "Slow down " + e.Value });
        }

		private void UpdateBumpsStatus()
		{
			BumpsStatus = "Bumps detected: " + bumpsDetected;
		}

        void HighSpeedDetected (object sender, BumpEventArgs e)
		{
            SpeedList.Children.Add(new Label(){ Text = "High speed " + e.Value });
		}

        private void BumpDetected (object sender, BumpEventArgs e)
		{
            newBumpTimestamp = DateTime.Now.ToMiliSecondsSince1970();
            if(PreviousBumpHappenedLongEnoughAgo())
            {
                previousBumpTimestamp = newBumpTimestamp;
                bumpsDetected++;
                SpeedList.Children.Add(new Label(){ Text = "BUMP " + e.Value });
                UpdateBumpsStatus ();
            }
		}

        bool PreviousBumpHappenedLongEnoughAgo()
        {
            return newBumpTimestamp - previousBumpTimestamp > Constants.TIME_BETWEEN_BUMPS;
        }
	}
}
