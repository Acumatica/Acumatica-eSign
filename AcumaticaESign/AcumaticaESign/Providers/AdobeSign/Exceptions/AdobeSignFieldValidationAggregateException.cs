using System;

namespace AcumaticaESign
{
    public class AdobeSignFieldValidationAggregateException : Exception
    {
        private const string separator = ", ";

        public AdobeSignFieldValidationAggregateException()
        {
        }

        public AdobeSignFieldValidationAggregateException(string[] fieldNames)
            : base(FormatMessage(fieldNames))
        {
        }

        public AdobeSignFieldValidationAggregateException(string[] fieldNames, Exception inner)
            : base(FormatMessage(fieldNames), inner)
        {
        }

        private static string FormatMessage(string[] fieldNames)
        {
            return string.Format(Messages.MessagePattern, string.Join(separator, fieldNames));
        }
    }
}
