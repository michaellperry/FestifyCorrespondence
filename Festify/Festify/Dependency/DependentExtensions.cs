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
            return new DependentListSubscription<T>(
                dependent,
                () => whenChanged(dependent));
        }

        public static DependentSubscription Repeat<TItem, TView>(
            this Layout<TView> container,
            Func<IEnumerable<TItem>> items,
            Func<TItem, TView> view)
            where TView : View
        {
            var depTimes = new DependentList<ChildView<TView>>(() =>
                items().Select(c => new ChildView<TView>(container, view(c))));
            var subscription = depTimes.Subscribe(delegate(IEnumerable<ChildView<TView>> views)
            {
                int i = 0;
                foreach (var v in views)
                    v.InsertAt(i++);
            });
            return subscription;
        }

        public static DependentSubscription BindText(
            this Label control,
            Func<string> text)
        {
            var dependent = new Dependent<string>(text);
            return dependent.Subscribe(t => control.Text = t);
        }

        public static DependentSubscription BindSource(
            this Image control,
            Func<string> source)
        {
            var dependent = new Dependent<string>(source);
            return dependent.Subscribe(s => control.Source = s);
        }
    }
}
