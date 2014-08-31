using Festify.Dependency;
using Festify.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Festify.Views
{
    public class DayView : StackLayout, IDisposable
    {
        private readonly DayHeader _header;

        private ChildManager _childManager = new ChildManager();

        public DayView(DayHeader header)
        {
            _header = header;

            Children.Add(new Label
            {
                Text = _header.Header
            });

            var list = new StackLayout();
            _childManager.Add(list.Repeat(() => _header.Times, t => new TimeView(t)));
            Children.Add(list);
        }

        public void Dispose()
        {
            _childManager.DisposeAll();
        }
    }
}
