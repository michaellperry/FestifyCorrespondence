﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Festify.Dependency
{
    public class ChildView<TChild> : IDisposable
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
}
