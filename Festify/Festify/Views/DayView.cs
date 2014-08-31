using Festify.Dependency;
using Festify.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Festify.Views
{
    public class DayView : StackLayout
    {
        private readonly DayHeader _header;

        public DayView(DayHeader header)
        {
            _header = header;

            Children.Add(new Label
            {
                Text = _header.Header
            });

            var list = new StackLayout();
            list.Repeat(() => _header.Times, t => new TimeView(t));
            Children.Add(list);
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            var that = obj as DayView;
            if (that == null)
                return false;

            return Object.Equals(this._header, that._header);
        }

        public override int GetHashCode()
        {
            return _header.GetHashCode();
        }
    }
}
