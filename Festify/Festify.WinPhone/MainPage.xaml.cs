using Microsoft.Phone.Controls;
using UpdateControls.Correspondence.Memory;
using Xamarin.Forms;
using UpdateControls.Correspondence.FileStream;

namespace Festify.WinPhone
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            Forms.Init();
            var storage = new FileStreamStorageStrategy();
            Content = Festify.App.GetMainPage(new DispatchAdapter(), storage).ConvertPageToUIElement(this);
            this.SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;
        }
    }
}
