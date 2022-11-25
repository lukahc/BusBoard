namespace BusBoard.Api
{
    public class Arrival
    {
        public string ExpectedArrival { get; set; }
        public string LineName { get; set; }
        public string DestinationName { get; set; }
        public string Date
        {
            get
            {
                string year = this.ExpectedArrival.Substring(0, 4);
                string month = this.ExpectedArrival.Substring(5, 2);
                string day = this.ExpectedArrival.Substring(8, 2);
                return (day + "/" + month + "/" + year);
            }
        }
        public string Time
        {
            get
            {
                return this.ExpectedArrival.Substring(11, 5);
            }
        }
    }
}