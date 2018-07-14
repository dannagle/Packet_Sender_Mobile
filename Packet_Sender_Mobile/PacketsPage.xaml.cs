using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;


using Sockets;
using Sockets.Plugin; //nuget https://github.com/rdavisau/sockets-for-pcl

using System.Diagnostics;
using SQLite;

namespace Packet_Sender_Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PacketsPage : ContentPage
    {

        private ObservableCollection<Packet> _thepackets;
        private TcpSocketClient tcp;
        private UdpSocketClient udp;
        private TcpSocketListener tcpServer;
        private UdpSocketReceiver udpServer;

        public int tcpPort;
        public int udpPort;

        public Packet thepacket;

		private SQLiteAsyncConnection _connection;

		bool _isDataLoaded;

		public PacketsPage()
        {
            InitializeComponent();

            tcpPort = SettingsPage.TCPPort;
            udpPort = SettingsPage.UDPPort;

            tcpServer = new TcpSocketListener();
            udpServer = new UdpSocketReceiver();

            tcpServer.ConnectionReceived += TcpConnection;
            udpServer.MessageReceived += UdpConnection;

            Task.Run(async () =>
            {
                await tcpServer.StartListeningAsync(tcpPort);
                await udpServer.StartListeningAsync(udpPort);
                SettingsPage.TCPPort = tcpServer.LocalPort;

                MessagingCenter.Send(this, Events.BOUND_PORTS_CHANGED, 0);

            });

            
            //udpServer.StartListeningAsync(udpPort);

            /*
            += async (sender, args) =>
        {
            var client = args.SocketClient;

            var bytesRead = -1;
            var buf = new byte[1];

            while (bytesRead != 0)
            {
                bytesRead = await args.SocketClient.ReadStream.ReadAsync(buf, 0, 1);
                //if (bytesRead > 0)
                //    Debug.Write(buf[0]);
            }
        };
        */

            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

            /*
			var demopackets = Packet.GetDemoPackets();
            for (int i = 0; i < demopackets.Count(); i++) {
                _thepackets.Add(demopackets[i]);
            }
			packetListView.ItemsSource = _thepackets;
			*/


			tcp = new TcpSocketClient();
            udp = new UdpSocketClient();  
            thepacket = new Packet();

            MessagingCenter.Subscribe<LoginPage, List<Packet>>(this, Events.NEW_PACKET_LIST, OnNewPacketListAsync);
            MessagingCenter.Subscribe<ImportCloud, List<Packet>>(this, Events.NEW_PACKET_LIST, OnNewPacketListAsyncIC);
            MessagingCenter.Subscribe<PacketEditPage, Packet>(this, Events.PACKET_MODIFIED, OnPacketModified);

        }



        void UdpConnection(object sender, Sockets.Plugin.Abstractions.UdpSocketMessageReceivedEventArgs args)
        {


            Packet pkt = new Packet();
            pkt.method = "udp";
            pkt.fromip = args.RemoteAddress;
            pkt.fromport = Convert.ToInt32(args.RemotePort);
            pkt.toip = "You";
            pkt.toport = udpPort;

            var buf = args.ByteData;

            if (buf.Length > 0)
            {
                pkt.hex = Packet.byteArrayToHex(buf);
            }
            

            MessagingCenter.Send(this, Events.NEW_TRAFFIC_PACKET, pkt);
        }




        async void TcpConnection(object sender, Sockets.Plugin.Abstractions.TcpSocketListenerConnectEventArgs args)
        {

            var client = args.SocketClient;

            Packet pkt = new Packet();
            pkt.method = "tcp";
            pkt.fromip = client.RemoteAddress;
            pkt.fromport = client.RemotePort;
            pkt.toip = "You";
            pkt.toport = tcpServer.LocalPort;


            var bytesRead = -1;
            var buf = new byte[1024];
            bytesRead = await args.SocketClient.ReadStream.ReadAsync(buf, 0, 1024);

            if (bytesRead > 0) {
                byte[] saveArray = new byte[bytesRead];
                Array.Copy(buf, saveArray, saveArray.Length);
                pkt.hex = Packet.byteArrayToHex(saveArray);
            }


            //TODO:make persistent?
            await client.DisconnectAsync();
            
            MessagingCenter.Send(this, Events.NEW_TRAFFIC_PACKET, pkt);
        }

        protected override async void OnAppearing()
		{
            Debug.WriteLine("OnAppearing: PacketsPage.xaml.cs");

            // In a multi-page app, everytime we come back to this page, OnAppearing
			// method is called, but we want to load the data only the first time
			// this page is loaded. In other words, when we go to ContactDetailPage
			// and come back, we don't want to reload the data. The data is already
			// there. We can control this using a switch: isDataLoaded.
			if (_isDataLoaded)
				return;

			_isDataLoaded = true;

			// I've extracted the logic for loading data into LoadData method. 
			// Now the code in OnAppearing method looks a lot cleaner. The 
			// purpose is very explicit. If data is loaded, return, otherwise,
			// load data. Details of loading the data is delegated to LoadData
			// method. 
			await LoadData();

			base.OnAppearing();

            //app needs to turn off tcp server. 
            MessagingCenter.Subscribe<App>(this, Events.APP_RESUME, async app => {
                //Do something
                Debug.WriteLine("APP_RESUME: PacketsPage.xaml.cs");
                await tcpServer.StartListeningAsync(tcpPort);
                await udpServer.StartListeningAsync(udpPort);
            });

            MessagingCenter.Subscribe<App>(this, Events.APP_SLEEP, app => {
                //Do something
                Debug.WriteLine("APP_SLEEP: PacketsPage.xaml.cs");
                tcpServer.StopListeningAsync();
                udpServer.StopListeningAsync();
            });

            MessagingCenter.Subscribe<App>(this, Events.APP_DISAPPEAR, app => {
                //Do something
                Debug.WriteLine("APP_DISAPPEAR: PacketsPage.xaml.cs");
                tcpServer.StopListeningAsync();
                udpServer.StopListeningAsync();
            });

		}

		private async Task LoadData()
		{
			await _connection.CreateTableAsync<Packet>();

			var pkts = await _connection.Table<Packet>().ToListAsync();
            List<Packet> SortedList = pkts.OrderBy(o => o.name).ToList();
            _thepackets = new ObservableCollection<Packet>(SortedList);
			packetListView.ItemsSource = _thepackets;
		}



        private void OnPacketModified(PacketEditPage source, Packet packet)
        {
            Debug.WriteLine("PP:Updating main list with " + packet.name + " headed to " + packet.toip);

            bool found = false;
            for (int i = 0; i < _thepackets.Count(); i++)
            {
                if (_thepackets[i].name == packet.name) {
                    Packet freshpacket = new Packet(); //forces view refresh. 
                    freshpacket.Clone(packet);
                    _thepackets[i] = freshpacket;
                    found = true;
                    break;
                }
                
            }

            if (!found) {
                _thepackets.Add(packet);
            }
            

            //packetListView.ItemsSource = newList;



        }

        private void OnNewPacketListAsyncIC(ImportCloud source, List<Packet> newList)
        {
            OnNewPacketListAsync(null, newList);
        }


            private void OnNewPacketListAsync(LoginPage source, List<Packet> newList)
        {
            Debug.WriteLine("PP:List now has " + newList.Count());
            Debug.WriteLine("PP:Updating main list");



            _connection.DropTableAsync<Packet>().Wait();
            _connection.CreateTableAsync<Packet>().Wait();
			_thepackets.Clear();

            for (int i = 0; i < newList.Count(); i++)
            {
				_connection.InsertAsync(newList[i]);
				_thepackets.Add(newList[i]);
            }

            //packetListView.ItemsSource = newList;



        }
        private void packetListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Debug.WriteLine("PP:Selected");

            thepacket = packetListView.SelectedItem as Packet;

            if (thepacket.unitTests())
            {
                //DisplayAlert("Alert", "Unit tests passed!", "OK");
            }
            else
            {
                DisplayAlert("Alert", "Unit tests failed", "OK");
            }


        }

        private void sendButton_Clicked(object sender, EventArgs e)
        {
            var sendpacket = packetListView.SelectedItem as Packet;
            if (sendpacket == null)
            {
                Debug.WriteLine("PP:sendButton_Clicked with null");

            } else {

                doSend(sendpacket);
            }

        }

        public async void doSend(Packet sendpacket)
        {
            Debug.WriteLine($"PP:doSend {sendpacket.method} {sendpacket.toip} {sendpacket.toport} {sendpacket.ascii}");
//            byte[] bytesToSend = System.Text.Encoding.UTF8.GetBytes(sendpacket.ascii);
            byte[] bytesToSend = sendpacket.getByteArray();

            try
            {
                sendpacket.error = "";
                await tcp.DisconnectAsync();

                if (sendpacket.isTCP())
                {
                    await tcp.ConnectAsync(sendpacket.toip, sendpacket.toport);
                    await tcp.WriteStream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
                    await tcp.DisconnectAsync();

                    /*
                    // wait a little before reading
                    var bytesRecv = new byte[20];
                    //tcp.ReadStream.ReadTimeout = 200;
                    //not async, so it can time out. 
                    await Task.Delay(TimeSpan.FromMilliseconds(00));
                    if (tcp.ReadStream.Length > 0)
                    {
                        int bytesRead = await tcp.ReadStream.ReadAsync(bytesRecv, 0, 20);
                        if (bytesRead > 0)
                        {
                            Packet receivepkt = new Packet();
                            receivepkt.toip = sendpacket.fromip;
                            receivepkt.fromip = sendpacket.toip;
                            receivepkt.fromport = sendpacket.toport;
                            Array.Resize(ref bytesRecv, bytesRead);
                            receivepkt.hex = Packet.byteArrayToHex(bytesRecv);
                            MessagingCenter.Send(this, Events.NEW_TRAFFIC_PACKET, receivepkt);
                        }

                    }
                    */


                }
                if (sendpacket.isUDP())
                {
                    await udpServer.SendToAsync(bytesToSend, sendpacket.toip, sendpacket.toport);
                }

            }
            catch (Exception eSend)
            {
                sendpacket.error = "Error: "+eSend.Message;
                Debug.WriteLine("PP:Exception : " + eSend.Message);
            }


            Debug.WriteLine("PP:Before Message");
            MessagingCenter.Send(this, Events.NEW_TRAFFIC_PACKET, sendpacket);
            Debug.WriteLine("PP:After Message");



            Debug.WriteLine("PP:Finished");




        }

        private void deleteButton_Clicked(object sender, EventArgs e)
        {
            Debug.WriteLine("PP:Need to delete " + thepacket.ascii);

			if(_thepackets.IndexOf(thepacket) > -1) {
				Debug.WriteLine("PP:Deleted packet");
				_thepackets.Remove(thepacket);
                _connection.DeleteAsync(thepacket);
            } else {
				Debug.WriteLine("PP:Did not delete packet");
			}




		}

        private async void modifyButton_Clicked(object sender, EventArgs e)
        {
			var sendpacket = packetListView.SelectedItem as Packet;
			if (sendpacket == null)
			{
				Debug.WriteLine("PP:modifyButton_Clicked with null");
                return;

			}

			Debug.WriteLine("PP:modifyButton_Clicked " + sendpacket.name +" " + sendpacket.method);
            //Navigation.PushAsync((new PacketEditPage()));
            await Navigation.PushModalAsync(new PacketEditPage(sendpacket));

        }

        private async void newButton_Clicked(object sender, EventArgs e)
        {
            thepacket = new Packet();
            Debug.WriteLine("PP:newButton_Clicked");
            await Navigation.PushModalAsync(new PacketEditPage(thepacket));
        }
    }


}