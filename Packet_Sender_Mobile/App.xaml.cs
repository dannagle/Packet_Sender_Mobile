using System.Diagnostics;
using Xamarin.Forms;

namespace Packet_Sender_Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainTabbedPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            Debug.WriteLine("OnStart: App.xaml.cs");
            MessagingCenter.Send(this, Events.APP_START);
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            Debug.WriteLine("OnSleep: App.xaml.cs");
            MessagingCenter.Send(this, Events.APP_SLEEP);
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            Debug.WriteLine("OnResume: App.xaml.cs");
            MessagingCenter.Send(this, Events.APP_RESUME);
        }
    }
}
