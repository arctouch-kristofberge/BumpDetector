using System;
using DeviceMotion.Plugin;
using DeviceMotion.Plugin.Abstractions;
using BumpDetector.Model;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;
using BumpDetector.Shared;

namespace BumpDetector
{
	public class BumpListener
	{
		protected bool hasUpdatedLastValues = false;
		protected DateTime lastUpdateTime;
		protected DateTime currentTime;
		protected double last_x;
		protected double last_y;
		protected double last_z;
		protected double speed;

        protected int numberOfValuesReceived = 0;
        protected enum MotionType{ FAST, SLOW };
        protected LinkedList<MotionType> PreviousMotions;
        protected const int AMOUNT_OF_PREVIOUS_MOTIONS = 2;

        public event EventHandler<MyArgs> OnBump;
        public event EventHandler<MyArgs> OnHighSpeedDetected;
        public event EventHandler<MyArgs> OnSlowDownAfterHighSpeed;
        public event EventHandler<MyArgs> OnFastSpeedEnded;

		public void StartListeningForBumps()
		{
            if(!CrossDeviceMotion.Current.IsActive(MotionSensorType.Accelerometer))
            {
                CrossDeviceMotion.Current.Start(MotionSensorType.Accelerometer, MotionSensorDelay.Game);
                CrossDeviceMotion.Current.SensorValueChanged += SensorValueChanged;
                PreviousMotions = new LinkedList<MotionType>();
            }
		}

		public void StopListeningForBumps()
		{
			CrossDeviceMotion.Current.Stop (MotionSensorType.Accelerometer);
			CrossDeviceMotion.Current.SensorValueChanged -= SensorValueChanged;
		}

		protected void SensorValueChanged (object sender, SensorValueChangedEventArgs e)
		{
			if(e.SensorType == MotionSensorType.Accelerometer)
			{
				AccelerometerValueChanged (e.Value as MotionVector);
			}
		}

		protected void AccelerometerValueChanged(MotionVector motion)
		{
			currentTime = DateTime.Now;
            numberOfValuesReceived++;
			if(hasUpdatedLastValues)
			{
				AnalyzeMotion (motion);
			}
			else
			{
				UpdateLastValues (motion);
			}
		}

		protected void UpdateLastValues(MotionVector motion)
		{
			last_x = motion.X;
			last_y = motion.Y;
			last_z = motion.Z;
			lastUpdateTime = currentTime;
			hasUpdatedLastValues = true;
		}

		protected void AnalyzeMotion (MotionVector motion)
		{
			if (LastUpdateWasLongEnoughAgo ())
			{
                CalculateSpeed(motion);

				if (speed > Constants.MOVEMENT_THRESHHOLD)
				{
					HighSpeedDetected ();
				}
				else if (WasAtHighSpeed())
				{
					DetectBump ();
				}
				else
				{
					LowSpeedDetected ();
				}
					
				UpdateLastValues (motion);
			}
		}

        protected bool LastUpdateWasLongEnoughAgo ()
        {
            return (currentTime - lastUpdateTime).TotalMilliseconds > Constants.MINIMUM_DURATION_OF_MOTION;
        }
        
        protected void CalculateSpeed(MotionVector motion)
        {
            double time = (currentTime - lastUpdateTime).TotalMilliseconds;
            double distance = motion.X + motion.Y + motion.Z - last_x - last_y - last_z;
            speed = Math.Abs(distance) / time * 10000;
        }

		protected void HighSpeedDetected()
		{
            AddPreviousMotion(MotionType.FAST);
			if(OnHighSpeedDetected != null)
                OnHighSpeedDetected(this, new MyArgs(){Value = speed});
		}

        protected virtual bool WasAtHighSpeed()
        {
            return PreviousMotions.FirstOrDefault () == MotionType.FAST;
        }

		protected void DetectBump()
		{
            if(PreviousMotionsWereAll(MotionType.FAST))
			{
                PreviousMotions.Clear();
				hasUpdatedLastValues = false;
				LaunchBumpEvent ();
			}
            else
            {
                AddPreviousMotion(MotionType.SLOW);
                if(OnSlowDownAfterHighSpeed != null)
                    OnSlowDownAfterHighSpeed(this, new MyArgs(){Value = speed});
            }
		}

        bool PreviousMotionsWereAll(MotionType motion, int numberOfValues = AMOUNT_OF_PREVIOUS_MOTIONS)
        {
            return PreviousMotions.Take(numberOfValues).Count(x => x == motion) == numberOfValues;
        }

		protected void LowSpeedDetected()
		{
            hasUpdatedLastValues = false;

            if(OnFastSpeedEnded!=null)
                OnFastSpeedEnded(this, new MyArgs(){Value = speed});
            AddPreviousMotion(MotionType.SLOW);
		}

		protected void LaunchBumpEvent()
		{
			if (OnBump != null)
                OnBump (this, new MyArgs(){Value = speed});
		}

        protected virtual void AddPreviousMotion(MotionType motion)
        {
            if(PreviousMotions.Count >= AMOUNT_OF_PREVIOUS_MOTIONS)
            {
                PreviousMotions.RemoveLast();
            }
            PreviousMotions.AddFirst(motion);

        }

		protected BumpData CreateBumpData()
		{
			return new BumpData ();
		} 
	}
}

