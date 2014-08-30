using Festify.ViewModels;
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
                Text = "Times",
                HorizontalOptions = LayoutOptions.CenterAndExpand,
            };
            var list = new ListView();
            list.SetBinding<MainViewModel>(
                ListView.ItemsSourceProperty, vm => vm.Times);
            list.ItemTemplate = new DataTemplate(() =>
            {
                var cell = new TextCell();
                cell.SetBinding<TimeHeader>(TextCell.TextProperty, h => h.Label);
                return cell;
            });
            //list.SetBinding<RoomSelectorScreen>(
            //    ListView.SelectedItemProperty, s => s.SelectedRoom);

            stack.Children.Add(label);
            stack.Children.Add(list);
            Content = stack;
        }

        void button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SessionPage());
        }
    }
}
