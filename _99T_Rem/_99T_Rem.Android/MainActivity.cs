using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

// BACK
using Xamarin.Forms;
// BACK

// BIND WIFI RICHIEDE NECESSARIAMENTE API 23
using Android.Net;
using Android.Content;
// BIND WIFI RICHIEDE NECESSARIAMENTE API 23

namespace _99T_Rem.Droid
{
    [Activity(Label = "_99T_Rem", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public App app;


        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            // BIND WIFI RICHIEDE NECESSARIAMENTE API 23
            try
            {
                ConnectivityManager cm = (ConnectivityManager)GetSystemService(ConnectivityService);
                Network[] networks = cm.GetAllNetworks();
                for (int i = 0; i < networks.Length; i++)
                {
                    NetworkInfo wifiInfo = cm.GetNetworkInfo(networks[i]);
                    if (wifiInfo.Type == ConnectivityType.Wifi)
                    {
                        cm.BindProcessToNetwork(networks[i]);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            // BIND WIFI RICHIEDE NECESSARIAMENTE API 23

            app = new App();
            LoadApplication(app);

            // BACKGROUND
            StartService(new Intent(this, typeof(LongRunningTaskService)));
            // BACKGROUND
        }

        // IMPEDITO IL BACK IN ANDROID
        public override void OnBackPressed()
        {
            if (app.DoBack)
            {
                return;
                //base.OnBackPressed();
            }
        }
        // IMPEDITO IL BACK IN ANDROID
    }
}

