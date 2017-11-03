namespace AcumaticaESign
{
    public class DocumentCreationInfoModel
    {
        public FileInfosModel[] fileInfos { get; set; }
        public string name { get; set; }
        public RecipientSetInfosModel[] recipientSetInfos { get; set; }
        public string signatureType { get; set; }
        public string callbackInfo { get; set; }
        public string[] ccs { get; set; }
        public int daysUntilSigningDeadline { get; set; }
        public string message { get; set; }
        public string reminderFrequency { get; set; }
        public string signatureFlow { get; set; }
    }
}
