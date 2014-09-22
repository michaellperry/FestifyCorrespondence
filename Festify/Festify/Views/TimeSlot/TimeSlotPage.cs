using Festify.Dependency;
using Festify.ViewModels.Detail;
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

            this.SetBinding<TimeSlotViewModel>(Page.TitleProperty, vm => vm.Time);

            var sessions = new ListView();
            sessions.SetBinding<TimeSlotViewModel>(ListView.ItemsSourceProperty, vm => vm.Sessions);
            sessions.ItemTemplate = new DataTemplate(() =>
            {
                ImageCell cell = new ImageCell();
                cell.SetBinding<SessionHeader>(ImageCell.TextProperty, s => s.Name);
                cell.SetBinding<SessionHeader>(ImageCell.TextColorProperty, s => s.TextColor);
                cell.SetBinding<SessionHeader>(ImageCell.DetailProperty, s => s.Speaker);
                cell.SetBinding<SessionHeader>(ImageCell.ImageSourceProperty, s => s.Image);
                return cell;
            });
            sessions.ItemSelected += sessions_ItemSelected;

            Content = sessions;
        }

        void sessions_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
                Navigation.PushAsync(new DetailPage(new DetailViewModel(((SessionHeader)e.SelectedItem).Session, _viewModel.Individual)));

            ((ListView)sender).SelectedItem = null;
        }
    }
}
