using System;
using DeviceMotion.Plugin.Abstractions;

namespace BumpDetector.Listener
{
    public class AcceleratorMotionEventArgs : EventArgs
    {
        public MotionVector Value { get; set; }
    }
}