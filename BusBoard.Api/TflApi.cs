using BusBoard.Api;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace BusBoard.Api
{
    public class TflApi
    {
        private readonly RestClient client = new RestClient("https://api.tfl.gov.uk/StopPoint");
        private readonly string appKey = "5bc76cf7dd6d40418c4fced56a2bc4ed";

        public List<BusStop> GetBusStops(Coordinates coordinates)
        {
            var stopRequest = new RestRequest("/", Method.Get)
                .AddQueryParameter("lon", coordinates.Longitude)
                .AddQueryParameter("lat", coordinates.Latitude)
                .AddQueryParameter("radius", 1000)
                .AddQueryParameter("stopTypes", "NaptanPublicBusCoachTram")
                .AddQueryParameter("app_key", appKey);

            stopRequest.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            var busStops = client.Execute<BusStops>(stopRequest).Data.StopPoints;
            busStops.RemoveAll(x => busStops.IndexOf(x) > 3);

            return busStops;
        }
        private class BusStops
        {
            public List<BusStop> StopPoints { get; set; }
        }
        public List<Arrival> GetArrivals(string id)
        {
            var arrivalRequest = new RestRequest("/{id}/Arrivals", Method.Get)
                .AddUrlSegment("id", id)
                .AddQueryParameter("app_key", appKey);
            arrivalRequest.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            var arrivals = client.Execute<List<Arrival>>(arrivalRequest).Data;
            SortArrivals(arrivals);
            return arrivals;
        }
        private static void SortArrivals(List<Arrival> arrivals)
        {
            bool sorted = false;
            while (sorted == false)
            {
                sorted = true;
                for (int i = 0; i < arrivals.Count - 1; i++)
                {
                    int hourA = Convert.ToInt32(arrivals[i].Time.Substring(0, 2));
                    int minuteA = Convert.ToInt32(arrivals[i].Time.Substring(3, 2));
                    int hourB = Convert.ToInt32(arrivals[i + 1].Time.Substring(0, 2));
                    int minuteB = Convert.ToInt32(arrivals[i + 1].Time.Substring(3, 2));
                    if ((hourA == hourB && minuteA > minuteB) || hourA > hourB)
                    {
                        sorted = false;
                        (arrivals[i + 1], arrivals[i]) = (arrivals[i], arrivals[i + 1]);
                    }
                }
            }
            arrivals.RemoveAll(x => arrivals.IndexOf(x)>4);
        }
    }
}