namespace BumpDetector.Web.Hubs
{
    using System;
    using System.Collections.Concurrent;
    using System.Device.Location;
    using System.Linq;

    using Microsoft.AspNet.SignalR;

    public class BumpHub : Hub
    {
        const double RANGE_IN_METERS = 20d;
        private const int TIME_RANGE_IN_MINS = 1;

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
                                        ConnectionId = connectionId,
                                        ArrivedAt = utcNow
                                    };

                BumpStorage.AllBumps.TryAdd(serverTimestamp, currentBumpInfo);

                GeoCoordinate thisGeoCoordinate = new GeoCoordinate(latitude, longitude);

                Clients.All.Log($"# Device id: {id} - arrived at {utcNow.ToString("R")}");
                Clients.All.Log($"Latitude: {latitude}");
                Clients.All.Log($"Longitude: {longitude}");
                Clients.All.Log($"Altitude: {altitude}");
                Clients.All.Log($"Timestamp on device: {timestamp}");
                Clients.All.Log($"ConnectionId on server: {connectionId}");

                foreach (BumpInfo bumpInfo in BumpStorage.AllBumps.Where(b => b.Key == serverTimestamp &&
                                                                                (b.Value != null && b.Value.Id != id)).Select(c => c.Value))
                {
                    GeoCoordinate otherGeoCoordinate = new GeoCoordinate(bumpInfo.Latitude, bumpInfo.Longitude);

                    double distance = thisGeoCoordinate.GetDistanceTo(otherGeoCoordinate);
                    bool areDevicesNear = distance < RANGE_IN_METERS;
                    var absTimeDiff = Math.Abs((TimeSpan.FromTicks(currentBumpInfo.ArrivedAt.Ticks) - TimeSpan.FromTicks(bumpInfo.ArrivedAt.Ticks)).Ticks);
                    bool haveBumpedAtSameTime = TimeSpan.FromTicks(absTimeDiff).TotalMinutes < TIME_RANGE_IN_MINS;

                    if (areDevicesNear && haveBumpedAtSameTime)
                    {
                        // TODO take altitude into account

                        Clients.Caller.BumpDetected(
                            $"Matched with Id: {bumpInfo.Id}",
                            $"Lat: {latitude} - Long: {longitude} - Alt: {altitude} - Timestamp: {timestamp}");

                        Clients.Client(bumpInfo.ConnectionId).BumpDetected(
                            $"Matched with Id: {id}",
                            $"Lat: {bumpInfo.Latitude} - Long: {bumpInfo.Longitude} - Alt: {bumpInfo.Altitude} - Timestamp: {bumpInfo.Timestamp}");

                        Clients.All.Log($"# MATCHED device {id} with {bumpInfo.Id}");

                        TryRemoveUsedBumps(serverTimestamp, currentBumpInfo, bumpInfo);
                        return;
                    }
                    else if(haveBumpedAtSameTime && !areDevicesNear)
                    {
                        Clients.All.Log($"# Did not match device {id} with {bumpInfo.Id} because they are {distance.ToString("0.##m")} distant.");
                    }
                }
            }
            catch (Exception e)
            {
                Clients.All.BumpDetected("Error", e.Message);
            }
        }

        private static void TryRemoveUsedBumps(long key, params BumpInfo[] bumpInfos)
        {
            foreach (BumpInfo bumpInfo in bumpInfos)
            {
                BumpInfo bumpInfoClone = bumpInfo.Clone();
                BumpStorage.AllBumps.TryRemove(key, out bumpInfoClone);
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

        public DateTime ArrivedAt { get; set; }

        public BumpInfo Clone()
        {
            return new BumpInfo
                       {
                           Id = Id,
                           Latitude = Latitude,
                           Longitude = Longitude,
                           Altitude = Altitude,
                           Timestamp = Timestamp,
                           ConnectionId = ConnectionId,
                           ArrivedAt = ArrivedAt
                       };
        }
    }
}