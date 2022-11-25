using BusBoard.Api;
using RestSharp;
using System.Collections.Generic;
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
            return arrivals;
        }
    }
}