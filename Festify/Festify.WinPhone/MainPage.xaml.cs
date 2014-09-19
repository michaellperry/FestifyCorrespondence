using Microsoft.Phone.Controls;
using UpdateControls.Correspondence.Memory;
using Xamarin.Forms;

namespace Festify.WinPhone
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            Forms.Init();
            Content = Festify.App.GetMainPage(new DispatchAdapter(), new MemoryStorageStrategy()).ConvertPageToUIElement(this);
        }
    }
}
