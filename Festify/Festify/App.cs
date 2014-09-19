using Festify.Synchronization;
using Festify.ViewModels.Main;
using Festify.Views.Main;
using UpdateControls;
using UpdateControls.Correspondence.Strategy;
using Xamarin.Forms;

namespace Festify
{
	public class App
	{
        public static Page GetMainPage(IDispatchOnUIThread dispatchAdapter, IStorageStrategy storage)
        {
            UpdateScheduler.Initialize(a => dispatchAdapter.Invoke(a));

            var synchronizationService = new SynchronizationService(storage);
            synchronizationService.Initialize();

            var viewModel = new MainViewModel(synchronizationService);
            var mainPage = new MainPage(viewModel, synchronizationService);

            return new NavigationPage(mainPage);
        }
	}
}
