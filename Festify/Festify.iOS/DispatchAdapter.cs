using MonoTouch.Foundation;
using System;

namespace Festify.iOS
{
    public class DispatchAdapter : IDispatchOnUIThread
    {
        public readonly NSObject owner;

        public DispatchAdapter(NSObject owner)
        {
            this.owner = owner;
        }

        public void Invoke(Action action)
        {
            owner.BeginInvokeOnMainThread(new NSAction(action));
        }
    }
}