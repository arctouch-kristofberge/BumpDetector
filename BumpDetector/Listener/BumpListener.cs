using System;

using DeviceMotion.Plugin;
using DeviceMotion.Plugin.Abstractions;

using System.Collections.Generic;
using System.Linq;

using BumpDetector.Shared;

namespace BumpDetector
{
    public class BumpListener
    {
        private bool hasUpdatedLastValues = false;

        private DateTime lastUpdateTime;

        private DateTime currentTime;

        private double lastX;

        private double lastY;

        private double lastZ;

        private double speed;

        private double lastSpeed;

        protected enum MotionType
        {
            DEFAULT,

            FAST,

            SLOW
        };

        private List<MotionType> previousMotions;

        private const int AMOUNT_OF_PREVIOUS_MOTIONS = 2;

        public event EventHandler<BumpEventArgs> OnBump;

        public event EventHandler<BumpEventArgs> OnHighSpeedDetected;

        public event EventHandler<BumpEventArgs> OnSlowDownAfterHighSpeed;

        public event EventHandler<BumpEventArgs> OnMovementDetected;

        public void StartListeningForBumps()
        {
            if (!CrossDeviceMotion.Current.IsActive(MotionSensorType.Accelerometer))
            {
                CrossDeviceMotion.Current.Start(MotionSensorType.Accelerometer, MotionSensorDelay.Game);
                CrossDeviceMotion.Current.SensorValueChanged += SensorValueChanged;
                this.previousMotions = new List<MotionType>(AMOUNT_OF_PREVIOUS_MOTIONS);
            }
        }

        protected void SensorValueChanged(object sender, SensorValueChangedEventArgs e)
        {
            if (e.SensorType == MotionSensorType.Accelerometer)
            {
                AccelerometerValueChanged(e.Value as MotionVector);
            }
        }

        protected void AccelerometerValueChanged(MotionVector motion)
        {
            this.currentTime = DateTime.Now;
            if (this.hasUpdatedLastValues)
            {
                AnalyzeMotion(motion);
            }
            else
            {
                UpdateLastValues(motion);
            }
        }

        protected void UpdateLastValues(MotionVector motion)
        {
            this.lastX = motion.X;
            this.lastY = motion.Y;
            this.lastZ = motion.Z;
            this.lastUpdateTime = this.currentTime;
            this.hasUpdatedLastValues = true;
        }

        protected void AnalyzeMotion(MotionVector motion)
        {
            if (LastUpdateWasLongEnoughAgo())
            {
                OnMovementDetected?.Invoke(this, new BumpEventArgs{Value = motion.Y});
                CalculateSpeed(motion);

                DetectMotions();

                UpdateLastValues(motion);
            }
        }

        protected bool LastUpdateWasLongEnoughAgo()
        {
            return (this.currentTime - this.lastUpdateTime).TotalMilliseconds > Constants.MINIMUM_DURATION_OF_MOTION;
        }

        protected void CalculateSpeed(MotionVector motion)
        {
            double time = (this.currentTime - this.lastUpdateTime).TotalMilliseconds;
            double distance = motion.X + motion.Y + motion.Z - this.lastX - this.lastY - this.lastZ;
            this.lastSpeed = this.speed;
            this.speed = Math.Abs(distance) / time * 10000;
        }

        protected void DetectMotions()
        {
            if (this.speed > Constants.HIGH_SPEED_THRESHHOLD)
            {
                RecordHighSpeed();
            }
            else if (PreviousMotionWasAtHighSpeed())
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
            this.previousMotions.AddRespectingCapacity(MotionType.FAST);
            OnHighSpeedDetected?.Invoke(this, new BumpEventArgs { Value = this.speed });
        }

        protected virtual bool PreviousMotionWasAtHighSpeed()
        {
            return this.previousMotions.FirstOrDefault() == MotionType.FAST;
        }

        protected void CheckIfBumpHappened()
        {
            if (PreviousMotionsWereAll(MotionType.FAST) && DidAbruptStop())
            {
                EndMotion();
                OnBump?.Invoke(this, new BumpEventArgs() { Value = this.speed });
            }
            else
            {
                OnSlowDownAfterHighSpeed?.Invoke(this, new BumpEventArgs() { Value = this.speed });
                this.previousMotions.AddRespectingCapacity(MotionType.SLOW);
            }
        }

        protected void EndMotion()
        {
            this.previousMotions.Clear();
            this.hasUpdatedLastValues = false;
        }

        private bool PreviousMotionsWereAll(MotionType motion, int numberOfValues = AMOUNT_OF_PREVIOUS_MOTIONS)
        {
            return this.previousMotions.Take(numberOfValues).Count(x => x == motion) == numberOfValues;
        }

        private bool DidAbruptStop()
        {
            return this.speed <= this.lastSpeed / 3;
        }
    }
}
