using System.Collections.Generic;

namespace AcumaticaESign
{
    public static class Constants
    {
        public static class Url
        {
            public const string DemoDocuSignDocumentDetailsUrl = "https://appdemo.docusign.com/documents/details/";
            public const string DocuSignDocumentDetailsUrl = "https://app.docusign.com/documents/details/";
        }

        public static class ErrorCode
        {
            public const string DocuSignEnvelopeLock = "EDIT_LOCK_NOT_LOCK_OWNER";
            public const string DocuSignAccountNotExists = "ACCOUNT_DOES_NOT_EXIST_IN_SYSTEM";
            public const string DocuSignEnvelopeNotExists = "DOCUMENT_DOES_NOT_EXIST";
            public const string DocuSignEnvelopeNotInFolder = "ENVELOPE_NOT_IN_FOLDER";
            public const string DocuSignEnvelopeVoidInvalidState = "ENVELOPE_CANNOT_VOID_INVALID_STATE";
        }

        public static readonly List<string> AcumaticaESignFileExtensions = new List<string>
        {
            ".pdf",
            ".doc",
            ".docx",
            ".xls",
            ".xlsx",
            ".ppt",
            ".pptx",
            ".wp",
            ".txt",
            ".rtf",
            ".tif",
            ".jpg",
            ".jpeg",
            ".gif",
            ".bmp",
            ".png",
            ".htm",
            ".html"
        };
    }
}
