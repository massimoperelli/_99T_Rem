using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

// PCLStorage
using PCLStorage;
// PCLStorage

namespace _99T_Rem
{
	public partial class Settings : ContentPage
	{
		public Settings()
		{
			InitializeComponent ();

            buttonSet.Text = App.Lingua[17];
            buttonCancel.Text = App.Lingua[18];

            entryIpOrName.Text = App.IpAddress;

            buttonSet.Clicked += async (sender, e) =>
            {
                App.IpAddress = entryIpOrName.Text;

                // PCLStorage
                IFolder rootFolder = FileSystem.Current.LocalStorage;
                IFolder folder = await rootFolder.CreateFolderAsync("_99T_Rem", CreationCollisionOption.OpenIfExists);
                IFile file = await folder.CreateFileAsync("settings.txt",CreationCollisionOption.ReplaceExisting);
                await file.WriteAllTextAsync(App.IpAddress);
                // PCLStorage

                Navigation.PopModalAsync();
            };

            buttonCancel.Clicked += async (sender, e) =>
            {
                Navigation.PopModalAsync();
            };
        }
    }
}