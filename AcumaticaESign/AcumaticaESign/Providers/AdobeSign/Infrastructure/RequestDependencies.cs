using RestSharp;

namespace AcumaticaESign
{
    public class RequestDependencies
    {
        public Credentials Credentials { get; }
        public RestClient RestClient { get; set; }

        public RequestDependencies(Credentials credentials)
        {
            Credentials = credentials;
        }
    }
}
