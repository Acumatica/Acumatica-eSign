namespace AcumaticaESign
{
    public class RecipientSetInfosModel
    {
        public RecipientSetMemberInfosModel[] recipientSetMemberInfos { get; set; }
        public string recipientSetRole { get; set; }
        public string privateMessage { get; set; }
        public int signingOrder { get; set; }
    }
}
