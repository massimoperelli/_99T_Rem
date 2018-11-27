using System;

using Android.App;
using _99T_Rem.Droid;
using Xamarin.Forms;
using Java.IO;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidMethods))]
namespace _99T_Rem.Droid
{
    public class AndroidMethods : IAndroidMethods
    {
        public void CloseApp()
        {
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }
    }
}

