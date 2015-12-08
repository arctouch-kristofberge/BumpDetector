using System;
using Xamarin.Forms;

namespace BumpDetector.Shared
{
	public static class Constants
	{
		public const string BUMP_MESSAGE = "BUMP";

        public static readonly int MINIMUM_DURATION_OF_MOTION = Device.OnPlatform(50, 200, 200);
		public static readonly double MOVEMENT_THRESHHOLD = Device.OnPlatform (120d, 190d, 200d);
		public static readonly double STATIONARY_THRESHHOLD = Device.OnPlatform (20d, 30d, 30d);
        public const double TIME_BETWEEN_BUMPS = 1000;
	}
}

