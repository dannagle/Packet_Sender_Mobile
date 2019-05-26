using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;

using System.Diagnostics;
using Newtonsoft.Json;

namespace Packet_Sender_Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImportCloud : ContentPage
    {


        private ObservableCollection<PacketSetJSON> _thepackets;
        private List<PacketSetJSON> _packetSets;

        public ImportCloud()
        {
            InitializeComponent();

            _thepackets = new ObservableCollection<PacketSetJSON>();
            packetSetJSONListView.ItemsSource = _thepackets;

            //urlEntry.Text = "https://cloud.packetsender.com/dannagle/homeset.";

            MessagingCenter.Subscribe<AccountCloud, List<PacketSetJSON>>(this, Events.FOUND_PACKETSET_LIST, OnNewPacketSetListAsync);

        }
        

        private void OnNewPacketSetListAsync(AccountCloud source, List<PacketSetJSON> newList)
        {
            Debug.WriteLine("IC:List now has " + newList.Count);

            _thepackets.Clear();

            for (int i = 0; i < newList.Count; i++)
            {
                _thepackets.Add(newList[i]);
            }

            Debug.WriteLine("IC:Updated list now has " + _thepackets.Count);

            //packetSetJSONListView.ItemsSource = null;
            //packetSetJSONListView.ItemsSource = _thepackets;

            //packetListView.ItemsSource = newList;

            Debug.WriteLine("IC:Finished");


        }

        private async void urlImportButton_ClickedAsync(object sender, EventArgs e)
        {
            Debug.WriteLine("IC:Import from URL");

            string urlpath = urlEntry.Text + "/json";

            if(!urlpath.StartsWith("http://", StringComparison.Ordinal)) {
                if (!urlpath.StartsWith("https://", StringComparison.Ordinal))
                {
                    await DisplayAlert("Error", "URL must start with http or https.", "OK");
                    return;
                }
            }

            //Check URL is valid. 

            var http = new HttpClient();



            try
            {
                var response = await http.GetAsync(urlpath).ConfigureAwait(false);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    try
                    {
                        _packetSets = JsonConvert.DeserializeObject<List<PacketSetJSON>>(json);

                        if (_packetSets.Count > 0)
                        {
                            _thepackets.Clear();
                            _thepackets.Add(_packetSets[0]);
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                DisplayAlert("Success", "Found a packet set.", "OK");

                            });


                        }
                    }
                    catch (Exception eJson)
                    {
                        Debug.WriteLine("IC:Exception : " + eJson.Message);
                        Debug.WriteLine("IC:Exception : " + eJson.InnerException.Message);

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert("Error", "Could find any packets.", "OK");

                        });
                    }
                }
                else
                {

                    await DisplayAlert("Error", "Connection Error.", "OK");
                }
            } catch (Exception eResponse)
            {

                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Error", "Problem connecting to " + urlpath, "OK");

                });
                Debug.WriteLine("IC:Exception : " + eResponse.InnerException.Message);
            }

        }

            private async void importSetButton_ClickedAsync(object sender, EventArgs e)
        {
            
            var thepacketset = packetSetJSONListView.SelectedItem as PacketSetJSON;

            if(thepacketset != null)
            {

                if (!String.IsNullOrWhiteSpace(thepacketset.name)
                    && !String.IsNullOrWhiteSpace(thepacketset.packetjson)
                    )
                {

                    var _packetsjson = JsonConvert.DeserializeObject<List<PacketJSON>>(thepacketset.packetjson);

                    if (_packetsjson != null)
                    {
                        if (_packetsjson.Count > 0)
                        {
                            MessagingCenter.Send(this, Events.NEW_PACKET_LIST, PacketJSON.ToPacketList(_packetsjson));

                            await DisplayAlert("Success", "Imported " + _packetsjson.Count() + " packets", "OK");

                            await Navigation.PopModalAsync();

                        }
                    }
                    

                }
            }


        }
    }
}