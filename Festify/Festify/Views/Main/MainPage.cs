using Festify.Dependency;
using Festify.Synchronization;
using Festify.ViewModels.Main;
using Xamarin.Forms;

namespace Festify.Views.Main
{
    public class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;
        private readonly SynchronizationService _synchronizationService;

        private ChildManager _childManager = new ChildManager();
        private readonly NavigationManager _navigation;
        
        public MainPage(MainViewModel viewModel, SynchronizationService synchronizationService)
        {
            _synchronizationService = synchronizationService;
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
                return cell;
            });
            times.ItemSelected += TimeSelected;

            var content = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            content.Children.Add(exception);
            content.Children.Add(title);
            content.Children.Add(times);

            Content = content;
        }

        private void TimeSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null && !_synchronizationService.Individual.IsNull)
                _navigation.NavigateToTimeSlot(((TimeHeader)e.SelectedItem).Time, _synchronizationService.Individual);

            ((ListView)sender).SelectedItem = null;
        }
    }
}
