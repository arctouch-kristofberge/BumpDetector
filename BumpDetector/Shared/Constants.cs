using System;
using Xamarin.Forms;

namespace BumpDetector.Shared
{
	public static class Constants
	{
		public const string BUMP_MESSAGE = "BUMP";

		public const int MINIMUM_DURATION_OF_MOTION = 250;
		public static readonly double MOVEMENT_THRESHHOLD = Device.OnPlatform (150d, 200d, 200d);
		public static readonly double STATIONARY_THRESHHOLD = Device.OnPlatform (30d, 30d, 30d);

	}
}

