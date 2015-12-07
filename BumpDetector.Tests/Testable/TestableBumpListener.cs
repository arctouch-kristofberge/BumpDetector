using System;
using DeviceMotion.Plugin.Abstractions;

namespace BumpDetector.Tests
{
	public class TestableBumpListener : BumpListener
	{
		void CallAccelerometerValueChanged(MotionVector motion)
		{
			AccelerometerValueChanged(motion);
		}
	}
}

