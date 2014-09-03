using Microsoft.Phone.Controls;
using Xamarin.Forms;

namespace Festify.WinPhone
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            Forms.Init();
            Content = Festify.App.GetMainPage(new DispatchAdapter()).ConvertPageToUIElement(this);
        }
    }
}
