using System;
using BumpDetector.Listener;

namespace BumpDetector.iOS.Implementations
{
    public class BetterBumpListener : IBetterBumpListener
    {
        #region IBetterBumpListener implementation
        public event EventHandler<BumpEventArgs> OnBump;
        public void StartListening()
        {
            
        }
        #endregion
    }
}

