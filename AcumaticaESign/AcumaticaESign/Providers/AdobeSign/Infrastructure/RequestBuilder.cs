using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using RestSharp;

namespace AcumaticaESign
{
    public class RequestBuilder
    {
        private const string IncorrectFileTypeMessage = "The mime type needs to be specified for all documents";
        private const string SentToYourselfMessage = "This workflow does not permit including yourself in some fields";
        private const string DocumentWasDeletedMessage = "The Agreement Asset ID specified is invalid";
        private const string SendWithoutRecipientMessage = "Recipient Set Info is missing.";

        public readonly Request Request;

        private readonly HttpStatusCode[] expectedStatuses =
        {
            HttpStatusCode.OK,
            HttpStatusCode.Created
        };

        private readonly IDictionary<string, string> fields = new Dictionary<string, string>
        {
            {"country_code", "country"},
            {"state_code", "state"}
        };

        internal RequestBuilder(RequestDependencies dependencies, Method method, string resource)
        {
            Request = new Request
            {
                Dependencies = dependencies,
                Method = method,
                Resource = resource
            };
        }

        public RequestBuilder AddFormContentTypeHeader()
        {
            Request.UseFormContentTypeHeader = true;
            return this;
        }

        public RequestBuilder AddAuthorizationHeader()
        {
            Request.UseAuthorizationHeader = true;
            return this;
        }

        public RequestBuilder AddDownloadParameter()
        {
            Request.IsDownload = true;
            return this;
        }

        public RequestBuilder AddQueryParameter(string name, object value)
        {
            if (value != null)
            {
                Request.QueryParameters.Add(name, value);
            }
            return this;
        }

        public RequestBuilder AddUrlSegment(string name, object value)
        {
            if (value != null)
            {
                Request.UrlParameters.Add(name, value);
            }
            return this;
        }

        public RequestBuilder AddFileParameter(string name, byte[] value)
        {
            if (value != null)
            {
                Request.FileParameters.Add(name, value);
            }
            return this;
        }

        public RequestBuilder AddBodyParameter(string name, object value)
        {
            if (value != null)
            {
                Request.BodyParameters.Add(name, value);
            }
            return this;
        }

        public T Execute<T>(bool checkStatusCode = true)
            where T : new()
        {
            return HandleResponse(() => Request.Execute<T>(), checkStatusCode).Data;
        }

        public RestResponse Execute(bool checkStatusCode = true)
        {
            return HandleResponse(() => Request.Execute(), checkStatusCode);
        }

        private T HandleResponse<T>(Func<T> getResponse, bool checkStatusCode)
            where T : RestResponse
        {
            var response = getResponse();

            HandleStatusCode(response, checkStatusCode);

            return response;
        }

        private void HandleStatusCode(RestResponse response, bool required)
        {
            if (required && !expectedStatuses.Contains(response.StatusCode))
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        throw new AdobeSignUnauthorizedException();
                    case HttpStatusCode.Forbidden:
                        throw new AdobeSignForbiddenException();
                    case HttpStatusCode.NotFound:
                        throw new AdobeSignNotFoundException();
                    case HttpStatusCode.BadRequest:
                        HandleValidationErrors(response);
                        break;
                    default:
                        throw new AdobeSignException($"Incorrect status code: {response.StatusDescription}");
                }
            }
        }

        private void HandleValidationErrors(RestResponse response)
        {
            dynamic errors = null;
            string message = null;
            try
            {
                var content = JsonConvert.DeserializeObject<dynamic>(response.Content);
                errors = content.errors;
                message = content.message;
            }
            catch
            {
                // ignored
            }

            if (errors != null)
            {
                IEnumerable<string> invalidFields = GetInvalidFields(errors);

                if (invalidFields.Any())
                {
                    throw new AdobeSignFieldValidationAggregateException(invalidFields.ToArray());
                }
            }
            if (message == null)
            {
                return;
            }
            if (message.Contains(IncorrectFileTypeMessage))
            {
                throw new AcumaticaESignIncorrectFileTypeException();
            }
            if (message.Contains(SentToYourselfMessage))
            {
                throw new AdobeSignSentToYourselfException();
            }
            if (message.Contains(DocumentWasDeletedMessage))
            {
                throw new AdobeSignDocumentWasDeletedException();
            }
            if (message.Contains(SendWithoutRecipientMessage))
            {
                throw new AdobeSignEnvelopeWithoutRecipientException();
            }
        }

        private IEnumerable<string> GetInvalidFields(dynamic errors)
        {
            var invalidFields = new List<string>();
            foreach (var field in fields)
            {
                if (errors[field.Key] != null)
                {
                    invalidFields.Add(field.Value);
                }
            }
            return invalidFields;
        }
    }
}
