using System;
using RestSharp;

namespace AcumaticaESign
{
    public class DocumentsService : BaseApiService
    {
        public DocumentsService(RequestDependencies dependencies)
            : base(dependencies)
        {
        }

        public TransientDocumentEntity SendDocument(Byte[] file, string fileName)
        {
            return new RequestBuilder(Dependencies, Method.POST, "/transientDocuments")
                .AddAuthorizationHeader()
                .AddFileParameter(fileName, file)
                .Execute<TransientDocumentEntity>();
        }

        public AgreementCreationEntity CreateAgreement(AgreementCreationInfoModel model)
        {
            return new RequestBuilder(Dependencies, Method.POST, "/agreements")
                .AddAuthorizationHeader()
                .AddBodyParameter("options", model.options)
                .AddBodyParameter("documentCreationInfo", model.documentCreationInfo)
                .Execute<AgreementCreationEntity>();
        }

        public object CancelAgreement(string agreementId, string comment)
        {
            return new RequestBuilder(Dependencies, Method.PUT, "/agreements/{agreementId}/status")
                .AddAuthorizationHeader()
                .AddUrlSegment("agreementId", agreementId)
                .AddBodyParameter("comment", comment)
                .AddBodyParameter("value", "CANCEL")
                .AddBodyParameter("notifySigner", true)
                .Execute();
        }

        public object DeleteAgreement(string agreementId)
        {
            return new RequestBuilder(Dependencies, Method.DELETE, "/agreements/{agreementId}")
                .AddAuthorizationHeader()
                .AddUrlSegment("agreementId", agreementId)
                .Execute();
        }

        public ReminderCreationResultEntity SendReminder(ReminderCreationInfoModel model)
        {
            return new RequestBuilder(Dependencies, Method.POST, "/reminders")
                .AddAuthorizationHeader()
                .AddBodyParameter("agreementId", model.agreementId)
                .AddBodyParameter("comment", model.comment)
                .Execute<ReminderCreationResultEntity>();
        }

        public AgreementInfoEntity GetAgreementStatus(string agreementId)
        {
            return new RequestBuilder(Dependencies, Method.GET, "/agreements/{agreementId}")
                .AddAuthorizationHeader()
                .AddUrlSegment("agreementId", agreementId)
                .Execute<AgreementInfoEntity>();
        }

        public byte[] GetAgreement(string agreementId)
        {
            IRestRequest request = new RequestBuilder(Dependencies, Method.GET, "/agreements/{agreementId}/combinedDocument")
               .AddAuthorizationHeader()
               .AddDownloadParameter()
               .AddUrlSegment("agreementId", agreementId).Request.BuildRestRequest();
            return Dependencies.RestClient.DownloadData(request);
        }

        public byte[] GetAgreementFormData(string agreementId)
        {
            IRestRequest request = new RequestBuilder(Dependencies, Method.GET, "/agreements/{agreementId}/formData")
               .AddAuthorizationHeader()
               .AddDownloadParameter()
               .AddUrlSegment("agreementId", agreementId).Request.BuildRestRequest();
            return Dependencies.RestClient.DownloadData(request);
        }

        public ViewUrlEntity GetAgreementAssets(string agreementId)
        {
            return new RequestBuilder(Dependencies, Method.POST, "/views/agreementAssets")
                .AddAuthorizationHeader()
                .AddDownloadParameter()
                .AddBodyParameter("agreementAssetId", agreementId)
                .AddBodyParameter("autoLogin ", true)
                .Execute<ViewUrlEntity>();
        }

        protected override void InitRestClient()
        {
            var api = string.IsNullOrEmpty(Dependencies.Credentials.ApiAccessPoint)
                ? Dependencies.Credentials.ApiUrl + "/api/rest/v5"
                : Dependencies.Credentials.ApiAccessPoint + "api/rest/v5";
            Dependencies.RestClient = new RestClient(api);
        }
    }
}
