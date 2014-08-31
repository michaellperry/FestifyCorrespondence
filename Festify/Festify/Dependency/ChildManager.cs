using System;
using System.Collections.Generic;
using System.Text;

namespace Festify.Dependency
{
    public class ChildManager
    {
        private List<DependentSubscription> _subscriptions =
            new List<DependentSubscription>();

        public void Add(DependentSubscription subscription)
        {
            _subscriptions.Add(subscription);
        }

        public void DisposeAll()
        {
            foreach (var subscription in _subscriptions)
                subscription.Unsubscribe();
        }
    }
}
