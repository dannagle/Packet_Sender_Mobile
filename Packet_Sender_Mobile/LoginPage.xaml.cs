using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;

using Plugin.Settings.Abstractions;
using Plugin.Settings;

namespace Packet_Sender_Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private List<PacketJSON> _packetsjson;
		private List<Packet> _packetsimport;
		
        private static ISettings AppSettings => CrossSettings.Current;


        public LoginPage()
        {
            InitializeComponent();
            _packetsjson = new List<PacketJSON> ();
            _packetsimport= new List<Packet>();

            usernameEntry.Text = SettingsPage.UserEmail;
            passwordEntry.Text = SettingsPage.UserPass;
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {

            var http = new HttpClient();
            var un = usernameEntry.Text;
            var pw = passwordEntry.Text;

            un = "boom";
            pw = "pass";

            string json = "";

            try {

                        //Xamarin cannot read cloudflare cert. This works with Let's Encrypt cert.
                json = await http.GetStringAsync(new Uri("https://cloud.packetsender.com/?un=" + un + "&pw=" + pw));
            } catch (HttpRequestException eHttp ) {
                Debug.WriteLine("Exception : " + eHttp.Message);
                Debug.WriteLine("Exception : " + eHttp.InnerException.Message);
            }

            if (json.Length > 0)
            {
                //JsonConvert.DeserializeObject<List<CustomerJson>>(json);

                try
                {
                    _packetsjson = JsonConvert.DeserializeObject<List<PacketJSON>>(json);
                }
                catch (Exception eJson)
                {
                    Debug.WriteLine("Exception : " + eJson.Message);
                    Debug.WriteLine("Exception : " + eJson.InnerException.Message);
                }

                MessagingCenter.Send(this, Events.NEW_PACKET_LIST, PacketJSON.ToPacketList(_packetsjson));

                SettingsPage.UserEmail = un;
                SettingsPage.UserPass = pw;

				await DisplayAlert("Success", "Found " + _packetsjson.Count() + " packets", "OK");

				await Navigation.PopModalAsync();
            }
            else {
                await DisplayAlert("Error", "Could not connect to packet sender.", "OK");

            }




        }

        private void createAccountButton_Clicked(object sender, EventArgs e)
        {

        }
    }
}