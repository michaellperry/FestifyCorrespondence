using System;
using Festify.Dependency;
using UpdateControls.Fields;

namespace Festify
{
    public static class DependentExtensions
    {
        public static DependentSubscription Subscribe<T>(
            this Dependent<T> dependent,
            Action<T> whenChanged)
        {
            return new DependentSubscription(dependent, () => whenChanged(dependent));
        }
    }
}
