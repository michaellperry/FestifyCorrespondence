using Android.App;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Festify.Droid
{
    public class DispatchAdapter : IDispatchOnUIThread
    {
        public readonly Activity owner;

        public DispatchAdapter(Activity owner)
        {
            this.owner = owner;
        }

        public void Invoke(Action action)
        {
            ThreadPool.QueueUserWorkItem(delegate(Object obj)
            {
                owner.RunOnUiThread(action);
            });
        }
    }
}