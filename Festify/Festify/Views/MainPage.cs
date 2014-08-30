using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Festify.Views
{
    public class MainPage : ContentPage
    {
        public MainPage()
        {
            Content = new Label
            {
                Text = "Main Page",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
            };
        }
    }
}
