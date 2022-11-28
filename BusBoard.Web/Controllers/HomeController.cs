using System.Text.RegularExpressions;
using System;
using System.Web.Mvc;
using BusBoard.Web.Models;
using BusBoard.Web.ViewModels;
using System.Drawing.Drawing2D;
using BusBoard.Api;
using System.Collections.Generic;

namespace BusBoard.Web.Controllers
{
    public class HomeController : Controller
    {
        private static readonly PostcodeApi postcodeApi = new PostcodeApi();
        private static readonly TflApi tflApi = new TflApi();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult BusInfo(PostcodeSelection selection)
        {
            string postcode = Regex.Replace(selection.Postcode.ToUpper(), @"\s+", string.Empty);
            Coordinates coordinates = postcodeApi.GetCoordinates(postcode);
            List<BusStop> busStops = tflApi.GetBusStops(coordinates);
            Dictionary<BusStop, List<Arrival>> stopsWithArrivals = new Dictionary<BusStop, List<Arrival>>();
            foreach (BusStop busStop in busStops)
            {
                stopsWithArrivals.Add(busStop, tflApi.GetArrivals(busStop.NaptanId));
            }
            var info = new BusInfo(postcode, stopsWithArrivals);
            return View(info);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Information about this site";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact us!";

            return View();
        }
    }
}