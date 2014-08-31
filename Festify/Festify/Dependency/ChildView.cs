using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Festify.Dependency
{
    public class ChildView<TItem, TChild> : IDisposable
        where TChild : View
    {
        private readonly TItem _item;
        private readonly Layout<TChild> _container;
        private readonly Func<TItem, TChild> _createChild;

        private TChild _child = null;
        private bool _inContainer = false;
        
        public ChildView(TItem item, Layout<TChild> container, Func<TItem, TChild> createChild)
        {
            _item = item;
            _container = container;
            _createChild = createChild;
        }

        public void InsertAt(int index)
        {
            if (!_inContainer)
            {
                _child = _createChild(_item);
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
            {
                _container.Children.Remove(_child);
                var disposable = _child as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            var that = obj as ChildView<TItem, TChild>;
            if (that == null)
                return false;

            return Object.Equals(this._item, that._item);
        }

        public override int GetHashCode()
        {
            return _item.GetHashCode();
        }
    }
}
