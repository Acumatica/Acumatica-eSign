namespace AcumaticaESign
{
    public class AdobeSignClient
    {
        public DocumentsService DocumentsService { get; }
        public AuthenticationService Authentication { get; }

        public AdobeSignClient(Credentials credentials)
        {
            Authentication = new AuthenticationService(new RequestDependencies(credentials));
            DocumentsService = new DocumentsService(new RequestDependencies(credentials));
        }
    }
}
