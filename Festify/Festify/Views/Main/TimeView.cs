using Festify.Dependency;
using Festify.ViewModels.Main;
using System;
using Xamarin.Forms;

namespace Festify.Views.Main
{
    public class TimeView : Grid, IDisposable
    {
        private readonly TimeHeader _viewModel;
        private readonly NavigationManager _navigation;

        private ChildManager _childManager = new ChildManager();

        public TimeView(TimeHeader viewModel, NavigationManager navigation)
        {
            _viewModel = viewModel;
            _navigation = navigation;

            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = new GridLength(100, GridUnitType.Absolute) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
            };

            var label = new Label();
            _childManager.Add(label.BindText(() => _viewModel.Label));
            Children.Add(label, 0, 0);

            var details = new ListView();
            details.ItemsSource = _viewModel.Sessions;
            details.ItemTemplate = new DataTemplate(() =>
            {
                var cell = new ImageCell();
                cell.SetBinding<SessionHeader>(ImageCell.TextProperty, s => s.Title);
                cell.SetBinding<SessionHeader>(ImageCell.DetailProperty, s => s.Speaker);
                cell.SetBinding<SessionHeader>(ImageCell.ImageSourceProperty, s => s.Image);
                cell.SetBinding<SessionHeader>(ImageCell.CommandProperty, s => s.Select);
                return cell;
            });

            //var details = new StackLayout();
            //_childManager.Add(details.Repeat(() => _viewModel.Sessions, s => new SessionView(s, _navigation)));
            Children.Add(details, 1, 0);

            details.ItemSelected += details_ItemSelected;
        }

        private void details_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            SessionHeader selected = (SessionHeader)e.SelectedItem;
            _navigation.NavigateToTimeSlot(selected.SessionPlace.Place.PlaceTime, selected.Individual);
        }

        public void Dispose()
        {
            _childManager.DisposeAll();
        }
    }
}
