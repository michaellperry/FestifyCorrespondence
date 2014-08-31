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

        private ChildManager _childManager = new ChildManager();
        
        public MainPage(MainViewModel viewModel)
        {
            _viewModel = viewModel;

            var exception = new Label();
            _childManager.Add(exception.BindText(() => _viewModel.Exception));

            var title = new Label();
            _childManager.Add(title.BindText(() => _viewModel.Title));

            var days = new StackLayout();
            _childManager.Add(days.Repeat(() => _viewModel.Days, d => new DayView(d)));
            days.VerticalOptions = LayoutOptions.FillAndExpand;

            var content = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            content.Children.Add(exception);
            content.Children.Add(title);
            content.Children.Add(days);

            Content = content;
        }

        void button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SessionPage());
        }

        protected override void OnDisappearing()
        {
            _childManager.DisposeAll();
        }
    }
}
