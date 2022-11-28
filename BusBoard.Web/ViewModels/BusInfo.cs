using BusBoard.Api;
using System.Collections.Generic;

namespace BusBoard.Web.ViewModels
{
    public class BusInfo
    {
        public BusInfo(string postCode, Dictionary<BusStop, List<Arrival>> stopsWithArrivals)
        {
            PostCode = postCode;
            StopsWithArrivals = stopsWithArrivals;
        }

        public string PostCode { get; set; }
        public Dictionary<BusStop, List<Arrival>> StopsWithArrivals { get; set; }
    }
}