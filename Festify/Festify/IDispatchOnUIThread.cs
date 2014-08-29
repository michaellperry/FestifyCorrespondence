using System;

namespace Festify
{
    public interface IDispatchOnUIThread
    {
        void Invoke(Action action);
    }
}
