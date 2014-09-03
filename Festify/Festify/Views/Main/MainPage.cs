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
            BindingContext = _viewModel;

            var exception = new Label();
            exception.SetBinding<MainViewModel>(Label.TextProperty, vm => vm.Exception);

            var title = new Label();
            title.SetBinding<MainViewModel>(Label.TextProperty, vm => vm.Title);

            var times = new ListView();
            times.SetBinding<MainViewModel>(ListView.ItemsSourceProperty, vm => vm.Times);
            times.ItemTemplate = new DataTemplate(() =>
            {
                var cell = new ImageCell();
                cell.SetBinding<TimeHeader>(ImageCell.TextProperty, s => s.Title);
                cell.SetBinding<TimeHeader>(ImageCell.DetailProperty, s => s.RoomNumber);
                cell.SetBinding<TimeHeader>(ImageCell.ImageSourceProperty, s => s.Image);
                cell.SetBinding<TimeHeader>(ImageCell.CommandProperty, s => s.Select);
                return cell;
            });

            var content = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            content.Children.Add(exception);
            content.Children.Add(title);
            content.Children.Add(times);

            Content = content;
        }
    }
}
