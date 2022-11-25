using System;
using System.Collections.Generic;
using System.Net;
using RestSharp;
using BusBoard.Api;
using System.Text.RegularExpressions;

namespace BusBoard.ConsoleApp
{
    class Program
    {
        private static readonly PostcodeApi postcodeApi = new PostcodeApi();
        private static readonly TflApi tflApi = new TflApi();
        static void Main()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            while (true)
            {
                Console.Write("Please enter postcode: ");
                string postcode = Regex.Replace(Console.ReadLine().ToUpper(), @"\s+", string.Empty);

                Coordinates coordinates = postcodeApi.GetCoordinates(postcode);
                if (coordinates == null )
                {
                    Console.WriteLine("Invalid postcode");
                    continue;
                }

                List<BusStop> busStops = tflApi.GetBusStops(coordinates);

                if (busStops.Count == 0)
                {
                    Console.WriteLine("No bus stops nearby");
                    continue;
                }

                Console.WriteLine("\nPostcode: " + postcode);
                Console.WriteLine("Displaying arrivals for nearest bus stops\n");

                for (int i = 0; i < 4 && i < busStops.Count; i++)
                {
                    List<Arrival> arrivals = tflApi.GetArrivals(busStops[i].NaptanId);
                    string stopName = busStops[i].CommonName;
                    SortArrivals(arrivals);
                    Console.WriteLine("Bus Stop: " + stopName);
                    Console.WriteLine("Next Bus Arrival(s): ");
                    if (arrivals.Count == 0)
                    {
                        Console.WriteLine("None");
                        continue;
                    }
                    for (int j = 0; j < 5 && j < arrivals.Count; j++)
                    {
                        Console.WriteLine(arrivals[j].LineName + " to " + arrivals[j].DestinationName + " arriving at " + arrivals[j].Time + " on " + arrivals[j].Date);
                    }
                    Console.WriteLine("");
                }
            }
        }
        public static void SortArrivals(List<Arrival> arrivals)
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
        }
    }
}
