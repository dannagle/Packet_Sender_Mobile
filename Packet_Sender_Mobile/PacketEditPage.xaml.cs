using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Packet_Sender_Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PacketEditPage : ContentPage
    {

        private SQLiteAsyncConnection _connection;
        private Packet bindPacket;
        private Packet originalPacket;

        public PacketEditPage(Packet packet)
        {

			if (packet == null)
			{
				packet = new Packet();
			}

            bindPacket = new Packet();
            originalPacket = new Packet();

            originalPacket.Clone(packet);
            bindPacket.Clone(packet);

            BindingContext = bindPacket;
            InitializeComponent();

        }

        async void OnSave(object sender, System.EventArgs e)
        {
            var packet = BindingContext as Packet;

            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

            if(packet.ascii != asciiEdit.Text) 
            {
				asciiEdit.Text = packet.ascii;
                packet = BindingContext as Packet;
            }

			if (packet.hex != hexEdit.Text)
			{
				hexEdit.Text = packet.hex;
				packet = BindingContext as Packet;
			}


			if (String.IsNullOrWhiteSpace(packet.name))
            {
                await DisplayAlert("Error", "Please enter a name.", "OK");
                return;
            }

            if (String.IsNullOrWhiteSpace(packet.toip))
            {
                await DisplayAlert("Error", "Please enter a target ip.", "OK");
                return;
            }


            Debug.WriteLine("Saving ID: " + packet.name + " headed to " + packet.toip);
            await _connection.InsertOrReplaceAsync(packet);
            MessagingCenter.Send(this, Events.PACKET_MODIFIED, packet);
            


            await Navigation.PopModalAsync();
        }


        void methodUnfocused(object sender, EventArgs e)
        {
            var packet = BindingContext as Packet;
            if (packetMethod.Text.Length > 0) {
                packet.method = packetMethod.Text;
                packetMethod.Text = packet.method;
            }
        }



        

        void asciiEditUnfocused(object sender, EventArgs e)
        {
            var packet = BindingContext as Packet;
            packet.ascii = asciiEdit.Text;

            asciiEdit.Text = packet.ascii;
            hexEdit.Text = packet.hex;

        }

        void hexEditUnfocused(object sender, EventArgs e)
        {
            var packet = BindingContext as Packet;
            packet.hex = hexEdit.Text;

            asciiEdit.Text = packet.ascii;
            hexEdit.Text = packet.hex;

        }

        async void editCancel_Clicked(object sender, EventArgs e)
        {
            originalPacket.Clone(bindPacket);
            await Navigation.PopModalAsync();

        }
    }
}