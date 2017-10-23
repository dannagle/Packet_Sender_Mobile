using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Plugin.Settings.Abstractions;
using Plugin.Settings;
using System.Diagnostics;

namespace Packet_Sender_Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {

        private static ISettings AppSettings => CrossSettings.Current;

        public SettingsPage()
        {
            InitializeComponent();
            tcpport.Text = "" + TCPPort;
            udpport.Text = "" + UDPPort;
            logTrafficToggle.On = LogTraffic;

            MessagingCenter.Subscribe<PacketsPage, int>(this, Events.BOUND_PORTS_CHANGED, OnPortsChanged);
        }


        private void OnPortsChanged(PacketsPage source, int unused)
        {
            tcpport.Text = "" + TCPPort;
            udpport.Text = "" + UDPPort;
        }
        

        public static bool LogTraffic
        {
            get => AppSettings.GetValueOrDefault(nameof(LogTraffic), true);
            set => AppSettings.AddOrUpdateValue(nameof(LogTraffic), value);

        }


        public static string UserEmail
        {
            get => AppSettings.GetValueOrDefault(nameof(UserEmail), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(UserEmail), value);
        }
       
        public static string UserName
        {
            get => AppSettings.GetValueOrDefault(nameof(UserName), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(UserName), value);
        }


        public static string UserPass
        {
            get => AppSettings.GetValueOrDefault(nameof(UserPass), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(UserPass), value);
        }

        public static int UDPPort
        {
            get
            {
                int theport= AppSettings.GetValueOrDefault(nameof(UDPPort), 5005);

                if (theport > 0)
                {
                    return theport;
                }
                else {
                    return 5005;
                }

            }
            set => AppSettings.AddOrUpdateValue(nameof(UDPPort), value);
        }

        public static int TCPPort
        {
            get
            {
                int theport = AppSettings.GetValueOrDefault(nameof(TCPPort), 5005);
                if (theport > 0)
                {
                    return theport;
                }
                else
                {
                    return 5005;
                }

            }
            set => AppSettings.AddOrUpdateValue(nameof(TCPPort), value);
        }

        private void OnImportClicked(object sender, EventArgs e)
        {
            var cloudPage = new CloudUI();      
            Navigation.PushModalAsync(cloudPage);

        }

        private void SettingsSave_Clicked(object sender, EventArgs e)
        {
            TCPPort = Convert.ToInt32(tcpport.Text);
            UDPPort = Convert.ToInt32(tcpport.Text);
            LogTraffic = logTrafficToggle.On;

            MessagingCenter.Send(this, Events.TRAFFIC_TOGGLED, logTrafficToggle.On);
        }
    }
}