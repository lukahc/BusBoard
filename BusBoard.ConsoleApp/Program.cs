using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;

namespace BusBoard.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string appKey = "5bc76cf7dd6d40418c4fced56a2bc4ed";
            Console.WriteLine("Please enter postcode: ");
            string postcode = Console.ReadLine();
            var postcodeClient = new RestClient("https://api.postcodes.io/");

            var postcodeRequest = new RestRequest("postcodes/{postcode}", Method.Get);

            postcodeRequest.AddUrlSegment("postcode", postcode);

            postcodeRequest.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            var postcodeResponse = postcodeClient.Execute<Postcode>(postcodeRequest);

            var lon = postcodeResponse.Data.Result.Longitude;
            var lat = postcodeResponse.Data.Result.Latitude;

            var tflClient = new RestClient("https://api.tfl.gov.uk/StopPoint");

            var stopRequest = new RestRequest("/", Method.Get)
                .AddQueryParameter("lon", lon)
                .AddQueryParameter("lat", lat)
                .AddQueryParameter("stopTypes", "NaptanPublicBusCoachTram")
                .AddQueryParameter("app_key", appKey);
            stopRequest.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            var stopResponse = tflClient.Execute<BusStops>(stopRequest);
            var id = stopResponse.Data.StopPoints[0].NaptanId;
            var stopName = stopResponse.Data.StopPoints[0].CommonName;

            var arrivalRequest = new RestRequest("/{id}/Arrivals", Method.Get)
                .AddUrlSegment("id", id)
                .AddQueryParameter("app_key", appKey);
            arrivalRequest.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            var arrivalResponse = tflClient.Execute<List<Arrival>>(arrivalRequest);
            var arrivals = arrivalResponse.Data;

            bool sorted = false;
            while (sorted == false)
            {
                sorted = true;
                for (int i = 0; i < arrivals.Count-1; i++)
                {
                    int hourA = Convert.ToInt32(arrivals[i].Time.Substring(0, 2));
                    int minuteA = Convert.ToInt32(arrivals[i].Time.Substring(3, 2));
                    int hourB = Convert.ToInt32(arrivals[i+1].Time.Substring(0, 2));
                    int minuteB = Convert.ToInt32(arrivals[i+1].Time.Substring(3, 2));
                    if ((hourA == hourB&&minuteA>minuteB)||hourA>hourB)
                    {
                        sorted = false;
                        (arrivals[i+1], arrivals[i]) = (arrivals[i], arrivals[i+1]);
                    }
                }
            }

            Console.WriteLine("Postcode: "+postcode);
            Console.WriteLine("Nearest Bus Stop: "+stopName);
            Console.WriteLine("Next Bus Arrival(s): ");
            for (int i = 0; i < 5&&i < arrivals.Count; i++)
            {
                Console.WriteLine(arrivals[i].LineName+" to " + arrivals[i].DestinationName+" arriving at " + arrivals[i].Time+" on " + arrivals[i].Date);
            }

            Console.ReadLine();

        }

    }
}
