namespace BumpDetector.Web.Hubs
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNet.SignalR;

    public class BumpHub : Hub
    {
        private static ConcurrentDictionary<long, BumpInfo> Bumps { get; } = new ConcurrentDictionary<long, BumpInfo>();

        public void NewBump(int id, double latitude, double longitude, double altitude, double timestamp)
        {
            try
            {
                Clients.All.Message("hello from the server");


                string connectionId = Context.ConnectionId;

                double latitudeRounded = Math.Round(latitude, 4);
                double longitudeRounded = Math.Round(longitude, 4);
                double altitudeRounded = Math.Round(altitude, 4);
                double timestampRounded = Math.Round(timestamp, 4);

                long groupId = GetGroupIdBasedOn(latitudeRounded, longitudeRounded, altitudeRounded, timestampRounded);

                Bumps.TryAdd(
                    groupId,
                    new BumpInfo
                    {
                        Id = id,
                        Latitude = latitudeRounded,
                        Longitude = longitudeRounded,
                        Altitude = altitudeRounded,
                        Timestamp = timestampRounded,
                        ConnectionId = connectionId
                    });

                Groups.Add(connectionId, groupId.ToString());
                KeyValuePair<long, BumpInfo> kvp = Bumps.SingleOrDefault(b => b.Key == groupId && b.Value.ConnectionId != connectionId);

                if (kvp.Equals(default(KeyValuePair<long, BumpInfo>)))
                {
                    return;
                }

                Clients.All.BumpDetected(
                    $"Id: {kvp.Value.Id}",
                    $"Lat: {latitude} - Long: {longitude} - Alt: {altitude} - Timestamp: {timestamp}");
            }
            catch (Exception e)
            {
                Clients.All.BumpDetected("Error", e.Message);
            }
        }

        private long GetGroupIdBasedOn(double latitude, double longitude, double altitude, double timestamp)
        {
            double sum = Math.Round((latitude + longitude + altitude + timestamp), 0);
            long value;
            long.TryParse(sum.ToString(), out value);
            return value;
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