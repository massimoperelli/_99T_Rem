using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

// LINGUA
using System.IO;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Sockets.Plugin;
// LINGUA

// PCLStorage
using PCLStorage;
// PCLStorage

// App Center
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
// App Center

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace _99T_Rem
{
	public partial class App : Application
	{
        // RELEASE
        // CAMBIARLO ANCHE NELLE PROPRIETA' E NEL PROGETTO LICENZE
        public static string Release = "_99T_Rem 1.0";
        // RELEASE

        // LINGUA
        public static string[] Lingua = new String[1000];
        // LINGUA

        // DEBUG
        public static bool Debug = true;
        // DEBUG

        // MACCHINA
        public static String IpAddress = "192.168.000.001";
        public static Int32 Port = 9094;
        public static TcpSocketClient socket = new TcpSocketClient();
        public static Task<int> RisultatoConnect;
        public static Task<int> RisultatoDisconnect;
        public static bool Connesso = false;
        // MACCHINA

        // IMPEDITO IL BACK IN ANDROID
        public bool DoBack
        {
            get
            {
                NavigationPage mainPage = MainPage as NavigationPage;
                if (mainPage != null)
                {
                    if (mainPage.CurrentPage.Title.CompareTo(App.Lingua[26]) == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return mainPage.Navigation.NavigationStack.Count > 1;
                    }
                }
                return true;
            }
        }
        // IMPEDITO IL BACK IN ANDROID

        public App ()
		{
            try
            {
                // LINGUA
                String CodiceLingua = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                if (CodiceLingua.CompareTo("it") == 0)
                {
                    CaricaLingua("Italian.txt");
                }
                else if (CodiceLingua.CompareTo("fr") == 0)
                {
                    CaricaLingua("Francais.txt");
                }
                else
                {
                    CaricaLingua("Italian.txt");
                }
                // LINGUA
            }
            catch (Exception ex)
            {
            }

            InitializeComponent();

            Configurazione();

            MainPage = new MainPage();
		}

		protected override void OnStart ()
		{
            // Handle when your app starts
            // App Center
            AppCenter.Start("ios=94e0b02e-c2cc-4fda-b449-fb88841a5aa5;" + "uwp={Your UWP App secret here};" + "android={Your Android App secret here}", typeof(Analytics), typeof(Crashes));
            // App Center
            Connetti();
        }

        protected override void OnSleep ()
		{
            // Handle when your app sleeps
            Disconnetti();
        }

		protected override void OnResume ()
		{
            // Handle when your app resumes
            Connetti();
        }

        public static async void Configurazione()
        {
            try
            {
                // PCLStorage
                IFolder rootFolder = FileSystem.Current.LocalStorage;
                IFolder folder = await rootFolder.CreateFolderAsync("_99T_Rem", CreationCollisionOption.OpenIfExists);
                ExistenceCheckResult exist = await folder.CheckExistsAsync("settings.txt");
                if (exist == ExistenceCheckResult.FileExists)
                {
                    IFile file = await folder.GetFileAsync("settings.txt");
                    string config = await file.ReadAllTextAsync();
                    IpAddress = config;
                }
                else
                {
                    IFile file = await folder.CreateFileAsync("settings.txt", CreationCollisionOption.ReplaceExisting);
                    await file.WriteAllTextAsync(App.IpAddress);
                }
                // PCLStorage
            }
            catch (Exception ex)
            {
            }
        }

        public static async void Connetti()
        {
            try
            {
                RisultatoConnect = Connect();
                var intRisultatoConnect = await RisultatoConnect;
                if (intRisultatoConnect == 0)
                {
                    Connesso = true;
                }
            }
            catch (Exception ex)
            {
                Connesso = false;
            }
        }

        public static async Task<int> Connect()
        {
            try
            {
                await socket.ConnectAsync(IpAddress, Port);
                return 0;
            }
            catch (Exception ex)
            {
                string temp = ex.ToString();
                return -1;
            }
        }

        public static async void Disconnetti()
        {
            try
            {
                RisultatoDisconnect = Disconnect();
                var intRisultatoDisconnect = await RisultatoDisconnect;
                if (intRisultatoDisconnect == 0)
                {
                    Connesso = false;
                }
            }
            catch (Exception ex)
            {
                Connesso = false;
            }
        }

        public static async Task<int> Disconnect()
        {
            try
            {
                await socket.DisconnectAsync();
                return 0;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        // LINGUA
        async void CaricaLingua(string Lingua)
        {
            try
            {
                var assembly = typeof(App).GetTypeInfo().Assembly;
                // PERCHE' QUESTA PARTE DI CODICE FUNZIONI RINOMINARE IL PROGETTO .ANDROID IN .DROID E RELATIVI ASSEMBLY

                Stream stream = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + Lingua);
                string text = "";
                using (var reader = new System.IO.StreamReader(stream))
                {
                    text = reader.ReadToEnd();
                }
                App.Lingua = text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < App.Lingua.Length; i++)
                {
                    App.Lingua[i] = App.Lingua[i].Split(';')[1];
                }
            }
            catch (Exception ex)
            {
                await _99T_Rem.App.Current.MainPage.DisplayAlert("Error", ex.ToString(), "OK");
            }
        }
        // LINGUA
    }
}
