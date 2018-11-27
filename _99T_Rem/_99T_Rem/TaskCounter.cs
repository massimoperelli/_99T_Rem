using Xamarin.Forms;
using System.Threading.Tasks;
using System.Threading;
using System;
using Plugin.LocalNotifications;
using Plugin.Vibrate;

namespace _99T_Rem
{
    public class TaskCounter
	{
        // NONOSTANTE AVER LAVORATO IN BACKGROUND SIA ANDROID SIA IOS CREANO PROBLEMI
        // IN PARTICOLARE IN ANDROID SE IL TELEFONO è ALIMENTATO A BATTERIA ED E' TRASCORSO QUALCHE 
        // MINUTO DI INATTIVITA' (DOZE), LA COSA MIGLIORA IMPOSTANDO DI NON GESTIRE LA BATTERIA PER L'APP 
        // SPECIFICA NELLE IMPOSTAZIONI/APP, MA NON E' RISOLUTIVA
        // IN PARTICOLARE IN IOS SE L'APP VIENE MESSA IN BACKGROUND E TRASCORRONO OLTRE 3 MINUTI L'APP VIENE CHIUSA
		public async Task RunCounter(CancellationToken token)
		{
            await Task.Run (async () => {

				for (long i = 0; i < long.MaxValue; i++) {
					token.ThrowIfCancellationRequested ();

					await Task.Delay(1000);
                    DateTime Adesso = DateTime.Now;

                    MainPage.StringaDebug = Math.Round(Math.Abs((Adesso - MainPage.Avviso).TotalSeconds),0).ToString();

                    if (Math.Abs((Adesso - MainPage.Avviso).TotalSeconds) < 5)
                    {
                        //CrossLocalNotifications.Current.Show(App.Release, App.Lingua[9], 0);
                        //var v1 = CrossVibrate.Current;
                        //v1.Vibration(TimeSpan.FromSeconds(0.5));
                    };
                }
			}, token);
		}
	}
}