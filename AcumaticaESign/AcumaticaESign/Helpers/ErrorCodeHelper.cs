using System.Collections.Generic;
using System.Linq;

namespace AcumaticaESign
{
    public static class ErrorCodeHelper
    {
        private static readonly Dictionary<string, string> ErrorCodes = new Dictionary<string, string>
        {
            {
                Constants.ErrorCode.DocuSignAccountNotExists, Messages.ESignAccountNotExists
            },
            {
                Constants.ErrorCode.DocuSignEnvelopeNotExists, Messages.ESignEnvelopeNotExists
            }
        };

        public static string GetValueByKey(string code)
        {
            return ErrorCodes.SingleOrDefault(x => x.Key == code).Value;
        }
    }
}