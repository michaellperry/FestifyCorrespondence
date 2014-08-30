using Festify.ViewModels;
using Festify.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls;
using Xamarin.Forms;
using Festify.Model;
using Festify.Synchronization;

namespace Festify
{
	public class App
	{
        public static Page GetMainPage(IDispatchOnUIThread dispatchAdapter)
        {
            UpdateScheduler.Initialize(a => dispatchAdapter.Invoke(a));

            var synchronizationService = new SynchronizationService();
            synchronizationService.Initialize();

            var viewModel = new MainViewModel(
                synchronizationService.Device.Conference,
                synchronizationService.Individual);
            var mainPage = new MainPage(viewModel);

            return new NavigationPage(mainPage);
        }
	}
}
