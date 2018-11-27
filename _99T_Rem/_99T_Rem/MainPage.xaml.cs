using Plugin.LocalNotifications;
using Plugin.Vibrate;
using Sockets.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace _99T_Rem
{
    public partial class MainPage : ContentPage
    {
        public static String Domanda = "";
        public static String Risposta = "";
        public static String Status = "";
        public static int RisultatoSendCommand;
        public static DateTime Puntato = new DateTime();
        public static TimeSpan Differenza;
        public static DateTime Avviso;
        public static long MillisecondiStart = 15000;
        public static long MillisecondiErogazione = 0;
        public static long MillisecondiRampe = 7000;
        public static string PaginaCorrente = "";
        public static bool CicloAttivo = false;
        public static bool RitardoImpostato = false;
        public static bool RitardoAttivato = false;
        public static bool StartInibito = false;
        public static string StringaDebug = "";
        public static int ContaErrori = 0;
        public static int fSkip = 0;

        public MainPage()
        {
            InitializeComponent();

            segmentMatrixGaugeRow1.Text = "";
            segmentMatrixGaugeRow2.Text = "";
            labelMessaggio.Text = "";
            pickerRoomNo.SelectedItem = "00";
            entryM3.Text = "1";

            buttonStart.Text = App.Lingua[1];
            labelRoomNo.Text = App.Lingua[2];
            labelM3.Text = App.Lingua[3];
            buttonSetRoomNo.Text = App.Lingua[4];
            buttonSetM3.Text = App.Lingua[4];
            buttonSetDateTime.Text = App.Lingua[4];
            buttonReset.Text = App.Lingua[5];

            pickerRoomNo.IsEnabled = false;
            buttonSetRoomNo.IsEnabled = false;

            entryM3.IsEnabled = false;
            buttonSetM3.IsEnabled = false;

            DP.IsEnabled = false;
            TP.IsEnabled = false;
            buttonSetDateTime.IsEnabled = false;

            buttonStart.IsEnabled = false;
            buttonReset.IsEnabled = false;

            
            buttonStart.Clicked += async (sender, e) =>
            {
                Domanda = "KEY^1";
                if ((PaginaCorrente.CompareTo("Work") == 0) && (CicloAttivo == false) && (RitardoImpostato == false) && (RitardoAttivato == false)) // START NORMALE
                {
                    Avviso = DateTime.Now + TimeSpan.FromMilliseconds(MillisecondiStart + MillisecondiErogazione + MillisecondiRampe);
                    labelMessaggio.Text = App.Lingua[14];
                    Device.StartTimer(TimeSpan.FromSeconds(4), () => {
                        labelMessaggio.Text = "";
                        return false; // runs again, or false to stop
                    });
                }
                else if ((PaginaCorrente.CompareTo("Work") == 0) && (CicloAttivo == false) && (RitardoImpostato == true) && (RitardoAttivato == false)) // START RITARDATO
                {
                    Avviso = Puntato + TimeSpan.FromMilliseconds(MillisecondiStart + MillisecondiErogazione + MillisecondiRampe);
                    labelMessaggio.Text = App.Lingua[15];
                    Device.StartTimer(TimeSpan.FromSeconds(12), () => {
                        labelMessaggio.Text = "";
                        return false; // runs again, or false to stop
                    });
                }
                else
                {
                    CrossLocalNotifications.Current.Cancel(0);
                }
            };

            buttonSetM3.Clicked += async (sender, e) =>
            {
                if ((int.Parse(entryM3.Text.ToString()) >= 0) || (int.Parse(entryM3.Text.ToString()) <= 1000))
                {
                    Domanda = "MC^" + entryM3.Text.ToString();
                }
            };

            buttonSetDateTime.Clicked += async (sender, e) =>
            {
                Puntato = new DateTime(DP.Date.Year, DP.Date.Month, DP.Date.Day, TP.Time.Hours, TP.Time.Minutes, TP.Time.Seconds);
                Differenza = Puntato.Subtract(DateTime.Now);
                if (Differenza.Ticks < 0)
                {
                    labelMessaggio.Text = App.Lingua[7];
                }
                else
                {
                    // ANTIBACO PER LA PRESSIONE TROPPO RAVVICINATA DI SET RITARDO E START
                    StartInibito = true;
                    buttonStart.IsEnabled = false;
                    // ANTIBACO PER LA PRESSIONE TROPPO RAVVICINATA DI SET RITARDO E START
                    Domanda = "SETDELAY^" + DP.Date.ToString("dd/MM/yy") + ";" + TP.Time.ToString() + "^" + DateTime.Now.ToString("dd/MM/yy;HH:mm:ss");
                    labelMessaggio.Text = App.Lingua[8];
                    Device.StartTimer(TimeSpan.FromSeconds(4), () => {
                        labelMessaggio.Text = "";
                        // ANTIBACO PER LA PRESSIONE TROPPO RAVVICINATA DI SET RITARDO E START
                        StartInibito = false;
                        // ANTIBACO PER LA PRESSIONE TROPPO RAVVICINATA DI SET RITARDO E START
                        return false; // runs again, or false to stop
                    });
                }
            };

            buttonReset.Clicked += async (sender, e) =>
            {
                Domanda = "RESET?^?^";

                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    if (Device.OS == TargetPlatform.Android)
                    {
                        DependencyService.Get<IAndroidMethods>().CloseApp();
                    }
                    if (Device.OS == TargetPlatform.iOS)
                    {
                        Thread.CurrentThread.Abort();
                    }
                    return false; // runs again, or false to stop
                });
            };

            buttonSetRoomNo.Clicked += async (sender, e) =>
            {
                Domanda = "ROOM^" + pickerRoomNo.SelectedItem.ToString();
            };

            Device.StartTimer(TimeSpan.FromSeconds(3), () => {
                var milliseconds = TimeSpan.FromMilliseconds(500);
                Device.StartTimer(milliseconds, () => {
                    if (fSkip == 0)
                    {

                        RisultatoSendCommand = SendCommand(Domanda);
                        if (RisultatoSendCommand < 0)
                        {
                        }
                        Domanda = "LCD";
                    }
                    return true;
                });
                return false; // runs again, or false to stop
            });
        }

        public int SendCommand(String Comando)
        {
            labelDebugDx.Text = ContaErrori.ToString();
            fSkip = 1;
            try
            {
                if (App.Connesso == false)
                {
                    labelMessaggio.Text = App.Lingua[12];
                    Device.StartTimer(TimeSpan.FromSeconds(4), () => {
                        labelMessaggio.Text = "";
                        return false; // runs again, or false to stop
                    });
                }

                String invio = Comando + "\r";
                var sendMsgBytes = Encoding.UTF8.GetBytes(invio);
                App.socket.WriteStream.WriteTimeout = 500;
                App.socket.WriteStream.Write(sendMsgBytes, 0, sendMsgBytes.Length);
                App.socket.WriteStream.Flush();
                //System.Diagnostics.Debug.WriteLine(Comando);
                byte[] RecvMsgBytes = new byte[1024];
                var bytes = 0;
                do
                {
                    try
                    {
                        App.socket.ReadStream.ReadTimeout = 500;
                        bytes = App.socket.ReadStream.Read(RecvMsgBytes, 0, RecvMsgBytes.Length);
                        Risposta = Encoding.UTF8.GetString(RecvMsgBytes, 0, bytes);
                        Risposta = Risposta.Replace("\r", "");
                        ContaErrori = 0;
                        //System.Diagnostics.Debug.WriteLine(Risposta);
                        if (Risposta.StartsWith("LCD") == true)
                        {
                            buttonReset.IsEnabled = true;

                            if (Risposta.Split('^')[1].StartsWith("   99T") == true)
                            {
                                PaginaCorrente = "Release";
                                segmentMatrixGaugeRow1.Text = Risposta.Split('^')[1];

                                // SE SONO NELLA PAGINA DEL FIRMWARE MANDO SUBITO UNO START PER PROSEGUIRE
                                invio = "KEY^1" + "\r";
                                sendMsgBytes = Encoding.UTF8.GetBytes(invio);
                                App.socket.WriteStream.Write(sendMsgBytes, 0, sendMsgBytes.Length);
                                App.socket.WriteStream.Flush();
                                fSkip = 0;
                                return 0;
                                // SE SONO NELLA PAGINA DEL FIRMWARE MANDO SUBITO UNO START PER PROSEGUIRE
                            }
                            else if (Risposta.Split('^')[1].StartsWith("ROOM") == true)
                            {
                                PaginaCorrente = "Room";
                                pickerRoomNo.IsEnabled = true;
                                buttonSetRoomNo.IsEnabled = true;

                                entryM3.IsEnabled = false;
                                buttonSetM3.IsEnabled = false;

                                DP.IsEnabled = false;
                                TP.IsEnabled = false;
                                buttonSetDateTime.IsEnabled = false;

                                segmentMatrixGaugeRow1.Text = Risposta.Split('^')[1];
                            }
                            else if ((Risposta.Split('^')[1].StartsWith("LVL:") == true) || (Regex.IsMatch(Risposta.Split('^')[1], @"^\d")))
                            {
                                PaginaCorrente = "Work";
                                pickerRoomNo.IsEnabled = false;
                                buttonSetRoomNo.IsEnabled = false;

                                pickerRoomNo.SelectedIndex = int.Parse(Risposta.Split('^')[3]);

                                entryM3.IsEnabled = true;
                                buttonSetM3.IsEnabled = true;

                                DP.IsEnabled = true;
                                TP.IsEnabled = true;
                                buttonSetDateTime.IsEnabled = true;

                                if (StartInibito == false) buttonStart.IsEnabled = true;

                                if (Risposta.Split('^')[4].CompareTo("1") == 0)
                                {
                                    CicloAttivo = true;
                                    entryM3.IsEnabled = false;
                                    buttonSetM3.IsEnabled = false;

                                    DP.IsEnabled = false;
                                    TP.IsEnabled = false;
                                    buttonSetDateTime.IsEnabled = false;
                                }
                                else
                                {
                                    CicloAttivo = false;
                                }
                                if (Risposta.Split('^')[5].CompareTo("1") == 0)
                                {
                                    RitardoImpostato = true;
                                    entryM3.IsEnabled = false;
                                    buttonSetM3.IsEnabled = false;

                                    DP.IsEnabled = true;
                                    TP.IsEnabled = true;
                                    buttonSetDateTime.IsEnabled = true;
                                }
                                else
                                {
                                    RitardoImpostato = false;
                                }
                                if (Risposta.Split('^')[6].CompareTo("1") == 0)
                                {
                                    RitardoAttivato = true;
                                    entryM3.IsEnabled = false;
                                    buttonSetM3.IsEnabled = false;

                                    DP.IsEnabled = false;
                                    TP.IsEnabled = false;
                                    buttonSetDateTime.IsEnabled = false;
                                }
                                else
                                {
                                    RitardoAttivato = false;
                                }

                                char[] bottiglia = new char[5];
                                if (RecvMsgBytes[8].ToString("X2").CompareTo("85") == 0) bottiglia[0] = '#';
                                else bottiglia[0] = '_';
                                if (RecvMsgBytes[9].ToString("X2").CompareTo("85") == 0) bottiglia[1] = '#';
                                else bottiglia[1] = '_';
                                if (RecvMsgBytes[10].ToString("X2").CompareTo("85") == 0) bottiglia[2] = '#';
                                else bottiglia[2] = '_';
                                if (RecvMsgBytes[11].ToString("X2").CompareTo("85") == 0) bottiglia[3] = '#';
                                else bottiglia[3] = '_';
                                if (RecvMsgBytes[12].ToString("X2").CompareTo("85") == 0) bottiglia[4] = '#';
                                else bottiglia[4] = '_';
                                segmentMatrixGaugeRow1.Text = "LVL:" + bottiglia[0] + bottiglia[1] + bottiglia[2] + bottiglia[3] + bottiglia[4] + Risposta.Split('^')[1].Substring(9, 7);

                                if ((Risposta.Split('^')[2].StartsWith("Mc") == true) && (Risposta.Split('^')[4].CompareTo("0") == 0)) // Ultimo giro prima dello start (quello dopo prenderebbe il tempo di 15 secondi)
                                {
                                    int Minuti = int.Parse(Risposta.Split('^')[2].Substring(9, 2));
                                    int Secondi = int.Parse(Risposta.Split('^')[2].Substring(12, 2));
                                    MillisecondiErogazione = ((Minuti * 60) + (long)Secondi) * 1000;
                                }
                            }
                            else
                            {
                                segmentMatrixGaugeRow1.Text = Risposta.Split('^')[1];
                            }
                            segmentMatrixGaugeRow2.Text = Risposta.Split('^')[2];
                        }
                        fSkip = 0;
                        return 0;
                    }
                    catch (Exception ex)
                    {
                        ContaErrori++;
                        if (ContaErrori == 10)
                        {
                            ContaErrori = 0;
                            App.Disconnetti();
                            Device.StartTimer(TimeSpan.FromSeconds(1), () => {
                                App.Connetti();
                                return false;
                            });
                        }
                    }
                } while (bytes > 0);
            }
            catch (Exception ex)
            {
                ContaErrori++;
                if (ContaErrori == 10)
                {
                    ContaErrori = 0;
                    App.Disconnetti();
                    Device.StartTimer(TimeSpan.FromSeconds(1), () => {
                        App.Connetti();
                        return false;
                    });
                }
                fSkip = 0;
                return -1;
            }
            fSkip = 0;
            return -2;
        }

        void OnImageNameTapped(object sender, EventArgs args)
        {
            try
            {
                Navigation.PushModalAsync(new Settings());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
