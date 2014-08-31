using Festify.Synchronization;
using Festify.ViewModels.Main;
using Festify.Views.Main;
using UpdateControls;
using Xamarin.Forms;

namespace Festify
{
	public class App
	{
        public static Page GetMainPage(IDispatchOnUIThread dispatchAdapter)
        {
            UpdateScheduler.Initialize(a => dispatchAdapter.Invoke(a));

            var synchronizationService = new SynchronizationService();
            synchronizationService.Initialize();

            var viewModel = new MainViewModel(synchronizationService);
            var mainPage = new MainPage(viewModel);

            return new NavigationPage(mainPage);
        }
	}
}
