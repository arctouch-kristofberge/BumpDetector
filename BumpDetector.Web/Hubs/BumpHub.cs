namespace BumpDetector.Web.Hubs
{
    using System;
    using System.Collections.Concurrent;
    using System.Device.Location;
    using System.Linq;

    using Microsoft.AspNet.SignalR;

    public class BumpHub : Hub
    {
        public void NewBump(int id, double latitude, double longitude, double altitude, double timestamp)
        {
            try
            {
                string connectionId = Context.ConnectionId;
                var utcNow = DateTime.UtcNow;
                long serverTimestamp = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, utcNow.Minute, 0).Ticks;

                BumpInfo currentBumpInfo = new BumpInfo
                                    {
                                        Id = id,
                                        Latitude = latitude,
                                        Longitude = longitude,
                                        Altitude = altitude,
                                        Timestamp = timestamp,
                                        ConnectionId = connectionId
                                    };

                BumpStorage.AllBumps.TryAdd(serverTimestamp, currentBumpInfo);

                GeoCoordinate thisGeoCoordinate = new GeoCoordinate(latitude, longitude);

                foreach (BumpInfo bumpInfo in BumpStorage.AllBumps.Where(b => b.Key == serverTimestamp &&
                                                                                (b.Value != null && b.Value.Id != id)).Select(c => c.Value))
                {
                    double acceptableRangeInMeters = 5d;

                    if (thisGeoCoordinate.GetDistanceTo(new GeoCoordinate(bumpInfo.Latitude, bumpInfo.Longitude)) < acceptableRangeInMeters)
                    {
                        // TODO take altitude into account
                        // TODO take timestamp interval into account

                        Clients.Caller.BumpDetected(
                            $"Matched with Id: {bumpInfo.Id}",
                            $"Lat: {latitude} - Long: {longitude} - Alt: {altitude} - Timestamp: {timestamp}");

                        Clients.Client(bumpInfo.ConnectionId).BumpDetected(
                            $"Matched with Id: {id}",
                            $"Lat: {bumpInfo.Latitude} - Long: {bumpInfo.Longitude} - Alt: {bumpInfo.Altitude} - Timestamp: {bumpInfo.Timestamp}");

                        var a = currentBumpInfo.Clone();
                        var b = bumpInfo.Clone();
                        BumpStorage.AllBumps.TryRemove(serverTimestamp, out a);
                        BumpStorage.AllBumps.TryRemove(serverTimestamp, out b);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Clients.All.BumpDetected("Error", e.Message);
            }
        }
    }

    public static class BumpStorage
    {
        public static ConcurrentDictionary<long, BumpInfo> AllBumps { get; } = new ConcurrentDictionary<long, BumpInfo>();
    }

    public static class DoubleExtensions
    {
        public static double Round(this double value)
        {
            return Math.Round(value, 3);
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

        public BumpInfo Clone()
        {
            return new BumpInfo
                       {
                           Id = Id,
                           Latitude = Latitude,
                           Longitude = Longitude,
                           Altitude = Altitude,
                           Timestamp = Timestamp,
                           ConnectionId = ConnectionId
                       };
        }
    }
}