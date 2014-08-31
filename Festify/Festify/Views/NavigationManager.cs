using Festify.Model;
using Festify.ViewModels.TimeSlot;
using Festify.Views.TimeSlot;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Festify.Views
{
    public class NavigationManager
    {
        private readonly INavigation _navigation;

        public NavigationManager(INavigation navigation)
        {
            _navigation = navigation;
        }

        public void NavigateToTimeSlot(Time time, Individual individual)
        {
            _navigation.PushAsync(new TimeSlotPage(new TimeSlotViewModel(time, individual)));
        }
    }
}
