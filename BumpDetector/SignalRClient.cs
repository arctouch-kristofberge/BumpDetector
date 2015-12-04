namespace BumpDetector
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNet.SignalR.Client;

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
            this.chatHubProxy.On<string, string>(nameof(BumpDetected), (deviceId, message) => { OnBumpDetected?.Invoke(deviceId, message); });
        }

        public bool IsConnectedOrConnecting => this.connection.State != ConnectionState.Disconnected;

        public void SendMessage(int id, double latitude, double longtitude, double altitude, double timestamp)
        {
            this.chatHubProxy.Invoke("NewBump", id, latitude, longtitude, altitude, timestamp);
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