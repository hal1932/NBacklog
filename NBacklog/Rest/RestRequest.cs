using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace NBacklog.Rest
{
    internal class RestRequest
    {
        public RestRequest(string resource, Method method, DataFormat dataFormat = DataFormat.Json, JsonSerializer serializer = null)
        {
            _resource = resource;
            _method = method;
            _dataFormat = dataFormat;
            _serializer = serializer ?? RestClient.DefaultSerializer;
        }

        public void AddHeader(string key, string value)
        {
            _headers.Add(new Parameter(key, value));
        }

        public void AddParameter(string key, object value)
        {
            _parameters.Add(new Parameter(key, value));
        }

        public void AddFile(string key, string path, string contentType = "application/octet-stream")
        {
            _files.Add(new FileParameter(key, path, contentType));
        }

        public HttpRequestMessage Build(string baseUri)
        {
            var url = $"{baseUri.Trim('/')}/{_resource.TrimStart('/')}";

            HttpContent content = null;

            if (_method == Method.GET)
            {
                var query = string.Join(
                    "&",
                    _parameters.Select(x => $"{x.Key}={HttpUtility.UrlEncode(x.Value.ToString())}").ToArray());

                if (query.Length > 0)
                {
                    url += $"?{query}";
                }
            }
            else
            {
                switch (_dataFormat)
                {
                    case DataFormat.Json:
                        if (_parameters.Any())
                        {
                            var stringBuilder = new StringBuilder();
                            using (var stringWriter = new StringWriter(stringBuilder))
                            using (var jsonWriter = new JsonTextWriter(stringWriter))
                            {
                                _serializer.Serialize(jsonWriter, _parameters.ToDictionary(x => x.Key, x => x.Value));
                            }

                            content = new StringContent(stringBuilder.ToString(), Encoding.UTF8, "application/json");
                        }
                        break;

                    case DataFormat.FormUrlEncoded:
                        if (_parameters.Any())
                        {
                            content = new FormUrlEncodedContent(_parameters.Select(x => x.ToKeyValuePair()));
                        }
                        break;

                    case DataFormat.MultiPart:
                        if (_parameters.Any() || _files.Any())
                        {
                            var multiContent = new MultipartFormDataContent();

                            if (_parameters.Any())
                            {
                                multiContent.Add(new FormUrlEncodedContent(_parameters.Select(x => x.ToKeyValuePair())));
                            }

                            if (_files.Any())
                            {
                                foreach (var file in _files)
                                {
                                    var fileContent = new ByteArrayContent(File.ReadAllBytes(file.Path));
                                    fileContent.Headers.Add("Content-Type", file.ContentType);
                                    multiContent.Add(fileContent, file.Key, Path.GetFileName(file.Path));
                                }
                            }

                            content = multiContent;
                        }
                        break;
                }
            }

            var request = new HttpRequestMessage(_method.HttpMethod, url)
            {
                Content = content,
            };

            foreach (var header in _headers)
            {
                request.Headers.Add(header.Key, header.Value as string);
            }

            return request;
        }

        private string _resource;
        private Method _method;
        private DataFormat _dataFormat;
        private JsonSerializer _serializer;

        private List<Parameter> _headers = new List<Parameter>();
        private List<Parameter> _parameters = new List<Parameter>();
        private List<FileParameter> _files = new List<FileParameter>();

        private class Parameter
        {
            public string Key { get; }
            public object Value { get; }

            public Parameter(string key, object value)
            {
                Key = key;
                Value = value;
            }

            public KeyValuePair<string, string> ToKeyValuePair()
            {
                var valueStr = (Value is bool) ? Value.ToString().ToLower() : Value.ToString();
                return new KeyValuePair<string, string>(Key, valueStr);
            }
        }

        private class FileParameter
        {
            public string Key { get; }
            public string Path { get; }
            public string ContentType { get; }

            public FileParameter(string key, string path, string contentType)
            {
                Key = key;
                Path = path;
                ContentType = contentType;
            }
        }
    }
}
