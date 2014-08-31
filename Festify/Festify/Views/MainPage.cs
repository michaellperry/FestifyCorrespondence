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
        
        public MainPage(MainViewModel viewModel)
        {
            _viewModel = viewModel;

            var exception = new Label();
            exception.BindText(() => _viewModel.Exception);

            var title = new Label();
            title.BindText(() => _viewModel.Title);

            var days = new StackLayout();
            days.Repeat(() => _viewModel.Days, d => new DayView(d));
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
    }
}
