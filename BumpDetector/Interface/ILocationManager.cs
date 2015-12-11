using System;
using BumpDetector.Model;


namespace BumpDetector
{
	public interface ILocationManager
	{
        void RequestCurrentLocation (double timeOut);
		event EventHandler<BumpLocation> OnLocationAcquired;
        event EventHandler<EventArgs> OnTimeOut;
	}
}