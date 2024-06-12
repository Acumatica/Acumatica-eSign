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
            return new RequestBuilder(Dependencies, Method.Post, "/transientDocuments")
                .AddAuthorizationHeader()
                .AddFileParameter(fileName, file)
                .Execute<TransientDocumentEntity>();
        }

        public AgreementCreationEntity CreateAgreement(AgreementCreationInfoModel model)
        {
            return new RequestBuilder(Dependencies, Method.Post, "/agreements")
                .AddAuthorizationHeader()
                .AddBodyParameter("options", model.options)
                .AddBodyParameter("documentCreationInfo", model.documentCreationInfo)
                .Execute<AgreementCreationEntity>();
        }

        public object CancelAgreement(string agreementId, string comment)
        {
            return new RequestBuilder(Dependencies, Method.Put, "/agreements/{agreementId}/status")
                .AddAuthorizationHeader()
                .AddUrlSegment("agreementId", agreementId)
                .AddBodyParameter("comment", comment)
                .AddBodyParameter("value", "CANCEL")
                .AddBodyParameter("notifySigner", true)
                .Execute();
        }

        public object DeleteAgreement(string agreementId)
        {
            return new RequestBuilder(Dependencies, Method.Delete, "/agreements/{agreementId}")
                .AddAuthorizationHeader()
                .AddUrlSegment("agreementId", agreementId)
                .Execute();
        }

        public ReminderCreationResultEntity SendReminder(ReminderCreationInfoModel model)
        {
            return new RequestBuilder(Dependencies, Method.Post, "/reminders")
                .AddAuthorizationHeader()
                .AddBodyParameter("agreementId", model.agreementId)
                .AddBodyParameter("comment", model.comment)
                .Execute<ReminderCreationResultEntity>();
        }

        public AgreementInfoEntity GetAgreementStatus(string agreementId)
        {
            return new RequestBuilder(Dependencies, Method.Get, "/agreements/{agreementId}")
                .AddAuthorizationHeader()
                .AddUrlSegment("agreementId", agreementId)
                .Execute<AgreementInfoEntity>();
        }

        public byte[] GetAgreement(string agreementId)
        {
            RestRequest request = new RequestBuilder(Dependencies, Method.Get, "/agreements/{agreementId}/combinedDocument")
               .AddAuthorizationHeader()
               .AddDownloadParameter()
               .AddUrlSegment("agreementId", agreementId).Request.BuildRestRequest();
            return Dependencies.RestClient.DownloadData(request);
        }

        public byte[] GetAgreementFormData(string agreementId)
        {
            RestRequest request = new RequestBuilder(Dependencies, Method.Get, "/agreements/{agreementId}/formData")
               .AddAuthorizationHeader()
               .AddDownloadParameter()
               .AddUrlSegment("agreementId", agreementId).Request.BuildRestRequest();
            return Dependencies.RestClient.DownloadData(request);
        }

        public ViewUrlEntity GetAgreementAssets(string agreementId)
        {
            return new RequestBuilder(Dependencies, Method.Post, "/views/agreementAssets")
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
