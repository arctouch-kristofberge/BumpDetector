namespace BumpDetector.Web.Hubs
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNet.SignalR;

    public class BumpHub : Hub
    {
        private static ConcurrentDictionary<int, BumpInfo> Bumps { get; } = new ConcurrentDictionary<int, BumpInfo>();

        public async Task NewBump(int id, double latitude, double longitude, double altitude, double timestamp)
        {
            int groupId = GetGroupIdBasedOn(latitude, longitude, altitude, timestamp);

            string connectionId = Context.ConnectionId;

            Bumps.TryAdd(
                groupId,
                new BumpInfo
                    {
                        Id = id,
                        Latitude = latitude,
                        Longitude = longitude,
                        Altitude = altitude,
                        Timestamp = timestamp,
                        ConnectionId = connectionId
                    });

            await Groups.Add(connectionId, groupId.ToString());

            //await Clients.OthersInGroup(groupId.ToString()).BumpDetected($"Id: {id}", $"Lat: {latitude}\nLong: {longitude}\nAlt: {altitude}\nTimestamp: {timestamp}");

            var otherClient = Bumps.Single(b => b.Key == groupId && b.Value.ConnectionId != connectionId).Value;

            Task callerTask = Clients.Caller.BumpDetected($"Id: {otherClient.Id}", $"Lat: {latitude} - Long: {longitude} - Alt: {altitude} - Timestamp: {timestamp}");
            //Task otherTask = Clients.Client(otherClient.ConnectionId).BumpDetected($"Id: {otherClient.Id}", $"Lat: {latitude}\nLong: {longitude}\nAlt: {altitude}\nTimestamp: {timestamp}");

            //await Task.WhenAll(callerTask, otherTask);

            await callerTask;
        }

        private int GetGroupIdBasedOn(double latitude, double longitude, double altitude, double timestamp)
        {
            return Convert.ToInt32(Math.Round(latitude) + Math.Round(longitude) + Math.Round(altitude) + Math.Round(timestamp));
        }
    }

    public class BumpInfo
    {
        public int Id { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Altitude { get; set; }

        public double Timestamp { get; set; }

        public string ConnectionId { get; set; }
    }
}