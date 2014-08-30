using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Festify.Views
{
    public class SessionPage : ContentPage
    {
        public SessionPage()
        {
            Content = new Label
            {
                Text = "Session",
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
        }
    }
}
