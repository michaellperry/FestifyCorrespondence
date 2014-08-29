using System;
using System.Windows;

namespace Festify.WinPhone
{
    public class DispatchAdapter : IDispatchOnUIThread
    {
        public void Invoke(Action action)
        {
            Deployment.Current.Dispatcher.BeginInvoke(action);
        }
    }
}
