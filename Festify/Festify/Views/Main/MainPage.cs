using Festify.Dependency;
using Festify.ViewModels.Main;
using System;
using Xamarin.Forms;

namespace Festify.Views.Main
{
    public class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        private ChildManager _childManager = new ChildManager();
        private readonly NavigationManager _navigation;
        
        public MainPage(MainViewModel viewModel)
        {
            _viewModel = viewModel;
            _navigation = new NavigationManager(Navigation);

            var exception = new Label();
            _childManager.Add(exception.BindText(() => _viewModel.Exception));

            var title = new Label();
            _childManager.Add(title.BindText(() => _viewModel.Title));

            var days = new StackLayout();
            _childManager.Add(days.Repeat(() => _viewModel.Days, d => new DayView(d, _navigation)));
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
