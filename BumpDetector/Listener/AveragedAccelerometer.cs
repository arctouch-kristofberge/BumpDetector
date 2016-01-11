using System;
using DeviceMotion.Plugin;
using DeviceMotion.Plugin.Abstractions;
using System.Collections.Generic;

namespace BumpDetector.Listener
{
    public class AveragedAccelerometer
    {
        private const int AVERAGE_OVER = 5;

        public event EventHandler<AcceleratorMotionEventArgs> OnValueChanged;

        private List<MotionVector> previousMotions;

        public void StartListening()
        {
            if (!CrossDeviceMotion.Current.IsActive(MotionSensorType.Accelerometer))
            {
                CrossDeviceMotion.Current.Start(MotionSensorType.Accelerometer, MotionSensorDelay.Ui);
                CrossDeviceMotion.Current.SensorValueChanged += SensorValueChanged;
                this.previousMotions = new List<MotionVector>(AVERAGE_OVER);
            }
        }

        private void SensorValueChanged(object sender, SensorValueChangedEventArgs e)
        {
            previousMotions.AddRespectingCapacity(e.Value as MotionVector);

            if(EnoughValuesSaved())
            {
                LaunchEvent();
            }
        }

        private bool EnoughValuesSaved()
        {
            return previousMotions.Count == AVERAGE_OVER;
        }

        private void LaunchEvent()
        {
            OnValueChanged?.Invoke(this, new AcceleratorMotionEventArgs{ Value = previousMotions.CalculateAverage() });
        }
    }
}