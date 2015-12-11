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
		protected double lastX;
		protected double lastY;
		protected double lastZ;
		protected double speed;
        protected double lastSpeed;

        protected enum MotionType{ DEFAULT, FAST, SLOW };
        protected List<MotionType> PreviousMotions;
        protected const int AMOUNT_OF_PREVIOUS_MOTIONS = 2;
        bool movementDirection;
        bool previousMovementDirection;


        public event EventHandler<BumpEventArgs> OnBump;
        public event EventHandler<BumpEventArgs> OnHighSpeedDetected;
        public event EventHandler<BumpEventArgs> OnSlowDownAfterHighSpeed;

		public void StartListeningForBumps()
		{
            if(!CrossDeviceMotion.Current.IsActive(MotionSensorType.Accelerometer))
            {
                CrossDeviceMotion.Current.Start(MotionSensorType.Accelerometer, MotionSensorDelay.Game);
                CrossDeviceMotion.Current.SensorValueChanged += SensorValueChanged;
                PreviousMotions = new List<MotionType>(AMOUNT_OF_PREVIOUS_MOTIONS);
            }
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
			lastX = motion.X;
			lastY = motion.Y;
			lastZ = motion.Z;
			lastUpdateTime = currentTime;
            previousMovementDirection = movementDirection;
            movementDirection = lastX + lastY + lastZ > 0;
			hasUpdatedLastValues = true;
		}

		protected void AnalyzeMotion (MotionVector motion)
		{
			if (LastUpdateWasLongEnoughAgo ())
			{
                CalculateSpeed(motion);

                DetectMotions();
					
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
            double distance = motion.X + motion.Y + motion.Z - lastX - lastY - lastZ;
            lastSpeed = speed;
            speed = Math.Abs(distance) / time * 10000;
        }

        protected void DetectMotions()
        {
            if(speed > Constants.HIGH_SPEED_THRESHHOLD)
            {
                RecordHighSpeed();
            }
            else if(PreviousMotionWasAtHighSpeed())
            {
                CheckIfBumpHappened();
            }
            else
            {
                EndMotion();
            }
        }

		protected void RecordHighSpeed()
		{
            PreviousMotions.AddRespectingCapacity(MotionType.FAST);
			if(OnHighSpeedDetected != null)
                OnHighSpeedDetected(this, new BumpEventArgs(){Value = speed});
		}

        protected virtual bool PreviousMotionWasAtHighSpeed()
        {
            return PreviousMotions.FirstOrDefault () == MotionType.FAST;
        }

		protected void CheckIfBumpHappened()
		{
            if(PreviousMotionsWereAll(MotionType.FAST) && DidAbruptStop())
			{
                EndMotion();
				LaunchBumpEvent ();
			}
            else
            {
                if(OnSlowDownAfterHighSpeed != null)
                        OnSlowDownAfterHighSpeed(this, new BumpEventArgs(){Value = speed});
                PreviousMotions.AddRespectingCapacity(MotionType.SLOW);
            }
        }

        protected void EndMotion()
        {
            PreviousMotions.Clear();
            hasUpdatedLastValues = false;
        }
        
        protected void LaunchBumpEvent()
        {
            if (OnBump != null)
                OnBump (this, new BumpEventArgs(){Value = speed});
        }
        
        bool PreviousMotionsWereAll(MotionType motion, int numberOfValues = AMOUNT_OF_PREVIOUS_MOTIONS)
        {
            return PreviousMotions.Take(numberOfValues).Count(x => x == motion) == numberOfValues;
        }
        
        bool DidAbruptStop()
        {
            return speed <= lastSpeed/3;
        }
        
        bool IsMovingInOpositeDirection()
        {
            return movementDirection != previousMovementDirection;
        }
	}
}

