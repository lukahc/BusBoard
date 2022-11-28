using System;
using System.Collections.Generic;
using System.Net;
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

                foreach (BusStop busStop in busStops)
                {
                    List<Arrival> arrivals = tflApi.GetArrivals(busStop.NaptanId);
                    string stopName = busStop.CommonName;
                    Console.WriteLine("Bus Stop: " + stopName);
                    Console.WriteLine("Next Bus Arrival(s): ");
                    if (arrivals.Count == 0)
                    {
                        Console.WriteLine("None");
                        continue;
                    }
                    foreach (Arrival arrival in arrivals)
                    {
                        Console.WriteLine(arrival.LineName + " to " + arrival.DestinationName + " arriving at " + arrival.Time + " on " + arrival.Date);
                    }
                    Console.WriteLine("");
                }
            }
        }
    }
}
