using Festify.Dependency;
using Festify.ViewModels.TimeSlot;
using Festify.Views.Detail;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Festify.Views.TimeSlot
{
    public class TimeSlotPage : ContentPage
    {
        private readonly TimeSlotViewModel _viewModel;

        private ChildManager _childMananger = new ChildManager();

        public TimeSlotPage(TimeSlotViewModel viewModel)
        {
            _viewModel = viewModel;
            BindingContext = _viewModel;

            var content = new StackLayout();

            Label title = new Label();
            title.SetBinding<TimeSlotViewModel>(Label.TextProperty, vm => vm.Time);
            content.Children.Add(title);

            var sessions = new ListView();
            sessions.SetBinding<TimeSlotViewModel>(ListView.ItemsSourceProperty, vm => vm.Sessions);
            sessions.ItemTemplate = new DataTemplate(() =>
            {
                ImageCell cell = new ImageCell();
                cell.SetBinding<SessionHeader>(ImageCell.TextProperty, s => s.Name);
                cell.SetBinding<SessionHeader>(ImageCell.DetailProperty, s => s.Speaker);
                cell.SetBinding<SessionHeader>(ImageCell.ImageSourceProperty, s => s.Image);
                return cell;
            });
            sessions.ItemSelected += sessions_ItemSelected;
            content.Children.Add(sessions);

            Content = content;
        }

        void sessions_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
                Navigation.PushAsync(new DetailPage(((SessionHeader)e.SelectedItem).Session));

            ((ListView)sender).SelectedItem = null;
        }
    }
}
