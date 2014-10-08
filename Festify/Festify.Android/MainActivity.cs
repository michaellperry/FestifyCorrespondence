using Android.App;
using Android.Content.PM;
using Android.OS;
using Correspondence.MobileStorage;
using Xamarin.Forms.Platform.Android;

namespace Festify.Droid
{
    [Activity(Label = "Festify", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : AndroidActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);

            SetPage(App.GetMainPage(new DispatchAdapter(this), new MobileStorageStrategy()));
        }
    }
}

