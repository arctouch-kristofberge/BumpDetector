using System;
using Ninject.Modules;
using BumpDetector.Listener;
using BumpDetector.iOS.Implementations;

namespace BumpDetector.iOS
{
    public class NinjectModuleiOS : NinjectModule
    {
        #region implemented abstract members of NinjectModule
        public override void Load()
        {
            this.Bind<ILocationManager>().To<LocationManagerIos>();
            this.Bind<IBetterBumpListener>().To<BetterBumpListener>();
        }
        #endregion
        
    }
}

