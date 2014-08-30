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

            Children.Add(StackLayoutOf(() =>
                _header.Times.Select(t => new TimeView(t))));
        }

        private StackLayout StackLayoutOf(Func<IEnumerable<View>> children)
        {
            var list = new StackLayout();
            list.Repeat(children);
            return list;
        }
    }
}
