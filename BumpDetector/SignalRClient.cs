namespace BumpDetector
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNet.SignalR.Client;

    using Xamarin.Forms;

    public class SignalRClient
    {
        private readonly HubConnection connection;

        private readonly IHubProxy chatHubProxy;

        public delegate void BumpDetected(string deviceId, string message);

        public event BumpDetected OnBumpDetected;

        public SignalRClient(string url)
        {
            this.connection = new HubConnection(url);
            this.chatHubProxy = this.connection.CreateHubProxy("BumpHub");
            this.chatHubProxy.On<string, string>("BumpDetected", (deviceId, message) => { OnBumpDetected?.Invoke(deviceId, message); });
            this.chatHubProxy.On<string>("Message", message =>
                {
                    Application.Current.MainPage.DisplayAlert("Message", message, "Cancel");
                });
        }

        public bool IsConnectedOrConnecting => this.connection.State != ConnectionState.Disconnected;

        public void SendMessage(int id, double latitude, double longitude, double altitude, double timestamp)
        {
            this.chatHubProxy.Invoke("NewBump", id, latitude, longitude, altitude, timestamp);
        }

        public Task Start()
        {
            return this.connection.Start();
        }

        public static async Task<SignalRClient> CreateAndStart(string url)
        {
            var client = new SignalRClient(url);
            await client.Start();
            return client;
        }
    }
}