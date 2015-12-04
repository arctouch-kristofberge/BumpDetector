using Microsoft.AspNet.SignalR;

namespace BumpDetector.Web.Hubs
{
    public class BumpHub : Hub
    {
        public void NewBump(int id, double latitude, double longitude, double altitude, double timestamp)
        {
            Clients.Caller.BumpDetected($"Id: {id}", $"Lat: {latitude}\nLong: {longitude}\nAlt: {altitude}\nTimestamp: {timestamp}");
        }
    }
}