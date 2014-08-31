using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Collections;

namespace Festify.Dependency
{
    public class DependentListSubscription<T> : DependentSubscription
    {
        private readonly DependentList<T> _dependentList;

        public DependentListSubscription(DependentList<T> dependentList, Action update)
            : base(dependentList.DependentSentry, update)
        {
            _dependentList = dependentList;
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();

            foreach (var child in _dependentList.OfType<IDisposable>())
                child.Dispose();
        }
    }
}
