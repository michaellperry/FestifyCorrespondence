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
            var stack = new StackLayout()
            {
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            var label = new Label
            {
                Text = "Main Page",
                HorizontalOptions = LayoutOptions.CenterAndExpand,
            };
            var button = new Button()
            {
                Text = "Session",
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            button.Clicked += button_Clicked;

            stack.Children.Add(label);
            stack.Children.Add(button);
            Content = stack;
        }

        void button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SessionPage());
        }
    }
}
