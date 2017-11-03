namespace AcumaticaESign
{
    /// <summary>
    /// Contains error information in <see cref="EnvelopesApi"/> rest api.
    /// </summary>
    public class ApiErrorModel
    {
        public string errorCode { get; set; }

        public string message { get; set; }
    }
}
