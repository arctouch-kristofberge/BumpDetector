using System;
using Ninject.Modules;
using BumpDetector.Droid.Implementations;
using BumpDetector.Listener;

namespace BumpDetector.Droid
{
    public class NinjectModuleAndroid : NinjectModule
    {
        #region implemented abstract members of NinjectModule
        public override void Load()
        {
            this.Bind<ILocationManager>().To<LocationManagerAndroid>();
            this.Bind<IBetterBumpListener>().To<BetterBumpListener>();
        }
        #endregion
    }
}

