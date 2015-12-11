using System;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using DeviceMotion.Plugin.Abstractions;
using BumpDetector.Shared;

namespace BumpDetector.Tests
{
	public class BumpListenerTests
	{
		#region Setup
		MotionVector motion;
		TestableBumpListener listener;

		[SetUp]
		public void Setup()
		{
			listener = new TestableBumpListener();
		}
		#endregion

		[Test]
		public void HighSpeedDetected()
		{
			//Arrange

			//Act

			//Assert
		}

		#region Helpers
		private MotionVector GetHighSpeedMotion()
		{
			double xyzValues = Constants.HIGH_SPEED_THRESHHOLD / 3 + 1;
			return new MotionVector() { X = xyzValues, Y = xyzValues, Z = xyzValues };
		}
		#endregion
	}
}

