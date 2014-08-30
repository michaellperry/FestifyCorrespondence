using System;
using Festify.Dependency;
using UpdateControls.Fields;
using UpdateControls.Collections;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;

namespace Festify
{
    public static class DependentExtensions
    {
        private class ChildView<TChild> : IDisposable
            where TChild : View
        {
            private readonly Layout<TChild> _container;
            private readonly TChild _child;

            private bool _inContainer = false;

            public ChildView(Layout<TChild> container, TChild child)
            {
                _child = child;
                _container = container;
            }

            public void InsertAt(int index)
            {
                if (!_inContainer)
                {
                    _container.Children.Insert(index, _child);
                    _inContainer = true;
                }
                else if (_container.Children[index] != _child)
                {
                    _container.Children.Remove(_child);
                    _container.Children.Insert(index, _child);
                }
            }

            public void Dispose()
            {
                if (_inContainer)
                    _container.Children.Remove(_child);
            }

            public override bool Equals(object obj)
            {
                if (this == obj)
                    return true;

                var that = obj as ChildView<TChild>;
                if (that == null)
                    return false;

                return Object.Equals(this._child, that._child);
            }

            public override int GetHashCode()
            {
                return _child.GetHashCode();
            }
        }

        public static DependentSubscription Subscribe<T>(
            this Dependent<T> dependent,
            Action<T> whenChanged)
        {
            return new DependentSubscription(
                dependent,
                () => whenChanged(dependent));
        }

        public static DependentSubscription Subscribe<T>(
            this DependentList<T> dependent,
            Action<IEnumerable<T>> whenChanged)
        {
            return new DependentSubscription(
                dependent.DependentSentry,
                () => whenChanged(dependent));
        }

        public static DependentSubscription Repeat<TView>(
            this Layout<TView> container,
            Func<IEnumerable<TView>> children)
            where TView : View
        {
            var depTimes = new DependentList<ChildView<TView>>(() =>
                children().Select(c => new ChildView<TView>(container, c)));
            var subscription = depTimes.Subscribe(delegate(IEnumerable<ChildView<TView>> views)
            {
                int i = 0;
                foreach (var v in views)
                    v.InsertAt(i++);
            });
            return subscription;
        }
    }
}
