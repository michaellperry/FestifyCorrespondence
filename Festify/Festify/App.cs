using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls;
using Xamarin.Forms;

namespace Festify
{
	public class App
	{
        public static Page GetMainPage(IDispatchOnUIThread dispatchAdapter)
		{
            UpdateScheduler.Initialize(a => dispatchAdapter.Invoke(a));

            return new ContentPage
			{
				Content = new Label {
					Text = "Hello, Forms !",
					VerticalOptions = LayoutOptions.CenterAndExpand,
					HorizontalOptions = LayoutOptions.CenterAndExpand,
				},
			};
		}
	}
}
