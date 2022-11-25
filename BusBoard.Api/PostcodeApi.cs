using RestSharp;
namespace BusBoard.Api
{
    public class PostcodeApi
    {
        private readonly RestClient client = new RestClient("https://api.postcodes.io/");

        public Coordinates GetCoordinates(string postcode)
        {
            var postcodeRequest = new RestRequest("postcodes/{postcode}", Method.Get)
                .AddUrlSegment("postcode", postcode);

            postcodeRequest.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            var coordinates = client.Execute<Postcode>(postcodeRequest).Data.Result;

            return coordinates;
        }
        private class Postcode
        {
            public Coordinates Result { get; set; }
        }
    }
}