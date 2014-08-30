using Festify.Dependency;
using Festify.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls;
using UpdateControls.Collections;
using UpdateControls.Fields;
using Xamarin.Forms;

namespace Festify.Views
{
    public class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        private List<DependentSubscription> _subscriptions = new List<DependentSubscription>();
        
        public MainPage(MainViewModel viewModel)
        {
            _viewModel = viewModel;

            var stack = StackLayoutOf(() =>
                _viewModel.Days.Select(d => new DayView(d)));
            stack.VerticalOptions = LayoutOptions.FillAndExpand;

            Content = stack;
        }

        private StackLayout StackLayoutOf(Func<IEnumerable<View>> children)
        {
            var list = new StackLayout();
            _subscriptions.Add(list.Repeat(children));
            return list;
        }

        void button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SessionPage());
        }

        protected override void OnDisappearing()
        {
            foreach (var s in _subscriptions)
                s.Unsubscribe();
            _subscriptions.Clear();
        }
    }
}
