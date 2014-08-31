using Festify.Dependency;
using Festify.ViewModels.Main;
using System;
using Xamarin.Forms;

namespace Festify.Views.Main
{
    public class DayView : StackLayout, IDisposable
    {
        private readonly DayHeader _header;
        private readonly NavigationManager _navigation;

        private ChildManager _childManager = new ChildManager();

        public DayView(DayHeader header, NavigationManager navigation)
        {
            _header = header;
            _navigation = navigation;

            Children.Add(new Label
            {
                Text = _header.Header
            });

            var list = new StackLayout();
            _childManager.Add(list.Repeat(() => _header.Times, t => new TimeView(t, _navigation)));
            Children.Add(list);
        }

        public void Dispose()
        {
            _childManager.DisposeAll();
        }
    }
}
