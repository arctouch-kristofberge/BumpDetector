using System;
using BumpDetector.Model;

namespace BumpDetector
{
	public interface ILocationManager
	{
		void RequestCurrentLocation ();
		event EventHandler<BumpLocation> OnLocationAcquired;
	}
}

