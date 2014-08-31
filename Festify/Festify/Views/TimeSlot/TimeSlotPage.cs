using Festify.Dependency;
using Festify.ViewModels.TimeSlot;
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

            var content = new StackLayout();

            Label title = new Label();
            _childMananger.Add(title.BindText(() => _viewModel.Time));
            content.Children.Add(title);

            StackLayout sessions = new StackLayout();
            _childMananger.Add(sessions.Repeat(
                () => _viewModel.Sessions,
                s => new SessionView(s)));
            content.Children.Add(sessions);

            Content = content;
        }
    }
}
