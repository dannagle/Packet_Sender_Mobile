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
using Newtonsoft.Json.Linq;

namespace Packet_Sender_Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountCloud : ContentPage
    {

        private List<PacketJSON> _packetsjson;
        private List<Packet> _packetsimport;
        private List<PacketSetJSON> _packetSets;

        public string jsontesting;

        public AccountCloud()
        {
            InitializeComponent();
            confirmLabel.IsVisible = false;
            passwordEntryConfirm.IsVisible = false;

            _packetsjson = new List<PacketJSON>();
            _packetsimport = new List<Packet>();

            //SettingsPage.UserName = "";
            //SettingsPage.UserPass = "";

            usernameEntry.Text = SettingsPage.UserName;
            passwordEntry.Text = SettingsPage.UserPass;

            jsontesting = "";


        }

        private bool loginMode()
        {
            return !passwordEntryConfirm.IsVisible;
        }

        private async void LoginButton_ClickedAsync(object sender, EventArgs e)
        {

            string username = usernameEntry.Text.Trim().ToLower();
            string password = passwordEntry.Text;
            string passwordconfirm = passwordEntryConfirm.Text;
            if (String.IsNullOrWhiteSpace(username))
            {
                await DisplayAlert("Error", "Username cannot be blank", "OK");
                return;
            }
            if (String.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Password cannot be blank", "OK");
                return;
            }



            if (loginMode())
            {
                var http = new HttpClient();

                string json = "";
                try
                {
                    var parameters = new Dictionary<string, string> { { "un", username }, { "pw", password } };
                    var encodedContent = new FormUrlEncodedContent(parameters);

                    //Xamarin cannot read cloudflare cert. This works with Let's Encrypt cert.
                    var url = "https://cloud.packetsender.com/";
                    var response = await http.PostAsync(url, encodedContent).ConfigureAwait(false);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        json = await response.Content.ReadAsStringAsync();

                        try
                        {
                            _packetSets = JsonConvert.DeserializeObject<List<PacketSetJSON>>(json);

                            if (_packetSets.Count > 0)
                            {
                                SettingsPage.UserName = username;
                                SettingsPage.UserPass = password;
                                _packetsjson = JsonConvert.DeserializeObject<List<PacketJSON>>(_packetSets[0].packetjson);
                                while((_packetSets.Count > 1) && (_packetsjson.Count == 0)) {
                                    _packetSets.RemoveAt(0);                                    
                                    _packetsjson = JsonConvert.DeserializeObject<List<PacketJSON>>(_packetSets[0].packetjson);
                                }
                                Debug.WriteLine("AC:First set is " + _packetSets[0].name);
                                Debug.WriteLine("AC:This packet set contains : " + _packetsjson.Count);
                                if (_packetsjson.Count > 0)
                                {
                                    Debug.WriteLine("AC:First packet is " + _packetsjson[0].name);

                                    //DisplayAlert("Success", "Found " + _packetSets.Count + " sets.", "OK").Wait();



                                    Device.BeginInvokeOnMainThread(async () => {
                                        MessagingCenter.Send(this, Events.FOUND_PACKETSET_LIST, _packetSets);
                                        await DisplayAlert("Success", "Found " + _packetSets.Count + " sets.", "OK");
                                        var masterPage = this.Parent as TabbedPage;
                                        masterPage.CurrentPage = masterPage.Children[1]; //change to middle tab
                                        Debug.WriteLine("AC:Finished");
                                    });

                                    //
                                    //
                                    Debug.WriteLine("AC:Finished");

                                    return;
                                }
                            }
                            else
                            {
                                await DisplayAlert("Error", "There were no saved packets.", "OK");
                                return;
                            }
                        }
                        catch (Exception eJson)
                        {


                            await DisplayAlert("Error", "Could not log in.", "OK");


                            Debug.WriteLine("AC:Exception : " + eJson.Message);
                            Debug.WriteLine("AC:Exception : " + eJson.InnerException.Message);
                            return;
                        }

                    }

                    await DisplayAlert("Error", "Did not find in packet sets.", "OK");
                    return;


                }
                catch (HttpRequestException eHttp)
                {
                    await DisplayAlert("Error", "Could not connect to cloud server.", "OK");
                    Debug.WriteLine("AC:Exception : " + eHttp.Message);
                    Debug.WriteLine("AC:Exception : " + eHttp.InnerException.Message);
                    return;
                }


            }
            else
            {
                if (password != passwordconfirm)
                {
                    await DisplayAlert("Error", "Passwords do not match", "OK");
                    return;
                }

            }

        }


        private async void cancelButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();

        }

        private void createAccountButton_Clicked(object sender, EventArgs e)
        {
            confirmLabel.IsVisible = !confirmLabel.IsVisible;
            passwordEntryConfirm.IsVisible = !passwordEntryConfirm.IsVisible;

            if (loginMode())
            {
                createAccountButton.Text = "Login";
                LoginButton.Text = "Create Account";
            }
            else
            {
                createAccountButton.Text = "Login Instead";
                LoginButton.Text = "Sign-up";
            }

        }
    }
}