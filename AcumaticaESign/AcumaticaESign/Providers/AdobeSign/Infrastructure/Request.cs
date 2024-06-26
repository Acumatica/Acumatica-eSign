﻿using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace AcumaticaESign
{
    public class Request
    {
        public RequestDependencies Dependencies { get; set; }

        public Method Method { private get; set; }
        public string Resource { private get; set; }
        public bool UseAuthorizationHeader { private get; set; }

        public bool IsDownload { private get; set; }
        public bool UseFormContentTypeHeader { private get; set; }

        public Dictionary<string, object> QueryParameters { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> BodyParameters { get; } = new Dictionary<string, object>();
        public Dictionary<string, byte[]> FileParameters { get; } = new Dictionary<string, byte[]>();
        public Dictionary<string, object> UrlParameters { get; set; } = new Dictionary<string, object>();

        public RestResponse Execute()
        {
            var request = BuildRestRequest();
            return Dependencies.RestClient.Execute(request);
        }

        public RestResponse<T> Execute<T>()
            where T : new()
        {
            var request = BuildRestRequest();
            return Dependencies.RestClient.Execute<T>(request);
        }

        public RestRequest BuildRestRequest()
        {
            var request = new RestRequest(Resource, Method);

            AddAuthorizationHeader(request);
            AddUseFormContentTypeHeader(request);
            AddQueryParameters(request);
            AddBodyParameters(request);
            AddFileParameters(request);
            AddDownloadParameter(request);
            AddUrlSegment(request);

            return request;
        }

        private void AddUseFormContentTypeHeader(RestRequest request)
        {
            if (UseFormContentTypeHeader)
            {
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            }
        }

        private void AddAuthorizationHeader(RestRequest request)
        {
            if (UseAuthorizationHeader)
            {
                request.AddHeader("Access-Token", Dependencies.Credentials.AccessToken);
            }
        }

        private void AddDownloadParameter(RestRequest request)
        {
            if (IsDownload)
            {
                request.AddHeader("Accept", "*/*");
            }
        }

        private void AddQueryParameters(RestRequest request)
        {
            foreach (var pair in QueryParameters)
            {
                request.AddParameter(pair.Key, pair.Value.ToString());
            }
        }

        private void AddUrlSegment(RestRequest request)
        {
            foreach (var pair in UrlParameters)
            {
                request.AddUrlSegment(pair.Key, (string) pair.Value);
            }
        }

        private void AddFileParameters(RestRequest request)
        {
            foreach (var pair in FileParameters)
            {
                request.AddFile("File", pair.Value, pair.Key);
            }
        }

        private void AddBodyParameters(RestRequest request)
        {
            if (BodyParameters.Count > 0)
            {
                var jObject = new JObject();
                var jsonSerializerSettings = GetJsonSerializerSettings();

                foreach (var pair in BodyParameters)
                {
                    var value = JsonConvert.SerializeObject(pair.Value, jsonSerializerSettings);
                    jObject.Add(pair.Key, JToken.Parse(value));
                }

                request.AddParameter("application/json", jObject.ToString(), ParameterType.RequestBody);
            }
        }

        private static JsonSerializerSettings GetJsonSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }
    }
}
