namespace AcumaticaESign
{
    public class InteractiveOptionsModel
    {
        public bool authoringRequested { get; set; } = true;
        public bool autoLoginUser { get; set; } = true;
        public string locale { get; set; }
        public bool noChrome { get; set; } = false;
        public bool sendThroughWeb { get; set; } = false;
        public SendThroughWebOptions sendThroughWebOptions { get; set; }
    }
}
