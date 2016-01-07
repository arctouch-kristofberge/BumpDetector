using System;
using BumpDetector.Model;

namespace BumpDetector.Listener
{
    public interface IBetterBumpListener
    {
        event EventHandler<BumpEventArgs> OnBump;
        void StartListening();
    }
}

