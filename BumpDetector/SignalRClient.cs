namespace BumpDetector
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNet.SignalR.Client;

    using Xamarin.Forms;
    using System.Text;
    using System.IO;
    using System.Diagnostics;

    public class SignalRClient
    {
        private readonly HubConnection connection;

        private readonly IHubProxy chatHubProxy;

        public delegate void BumpDetected(string deviceId, string message);

        public event BumpDetected OnBumpDetected;

        public SignalRClient(string url)
        {
            this.connection = new HubConnection(url);
            this.connection.TraceLevel = TraceLevels.All;
            this.connection.TraceWriter = new DebugTextWriter();
            this.chatHubProxy = this.connection.CreateHubProxy("BumpHub");
            this.chatHubProxy.On<string, string>("BumpDetected", (deviceId, message) => { OnBumpDetected?.Invoke(deviceId, message); });
            this.chatHubProxy.On<string>("Message", message =>
                {
                    Application.Current.MainPage.DisplayAlert("Message", message, "Cancel");
                });
        }

        public bool IsConnected => this.connection.State == ConnectionState.Connected;

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

        private class DebugTextWriter : TextWriter
        {
            private StringBuilder buffer;

            public DebugTextWriter()
            {
                buffer = new StringBuilder();
            }

            public override void Write(char value)
            {
                switch (value)
                {
                    case '\n':
                        return;
                    case '\r':
                        Debug.WriteLine(buffer.ToString());
                        buffer.Clear();
                        return;
                    default:
                        buffer.Append(value);
                        break;
                }
            }

            public override void Write(string value)
            {
                Debug.WriteLine(value);

            }
            #region implemented abstract members of TextWriter
            public override Encoding Encoding
            {
                get { throw new NotImplementedException(); }
            }
            #endregion
        }
    }
}