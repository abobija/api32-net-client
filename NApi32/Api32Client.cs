using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NApi32
{
    public class Api32Client
    {
        public static readonly Encoding Encoding = Encoding.ASCII;
        
        public Uri Uri { get; private set; }

        protected BasicAuth Auth { get; set; }

        /// <summary>
        /// Uri example: http://192.168.0.103 
        /// </summary>
        public Api32Client(string uri)
        {
            Uri = new Uri(uri);
        }

        public Api32Client Authorize(BasicAuth auth)
        {
            Auth = auth;
            return this;
        }

        public Api32Client Authorize(string username, string password) 
            => Authorize(new BasicAuth(username, password));

        public T DoGet<T>(string endpoint)
            => Invoke<T>(WebRequestMethods.Http.Get, endpoint, null);

        public T DoPost<T>(string endpoint, object reqObject)
            => Invoke<T>(WebRequestMethods.Http.Post, endpoint, reqObject);

        public async Task<T> DoGetAsync<T>(string endpoint) 
            => await InvokeAsync<T>(WebRequestMethods.Http.Get, endpoint, null);

        public async Task<T> DoPostAsync<T>(string endpoint, object reqObject) 
            => await InvokeAsync<T>(WebRequestMethods.Http.Post, endpoint, reqObject);

        protected async Task<T> InvokeAsync<T>(string method, string endpoint, object reqObject)
            => await Task.Run(() => Invoke<T>(method, endpoint, reqObject));

        protected T Invoke<T>(string method, string endpoint, object reqObject)
        {
            endpoint = endpoint.Trim(new char[] { '/' });

            var request = WebRequest.Create($"{Uri.Scheme}://{Uri.Host}/{endpoint}") as HttpWebRequest;

            request.Timeout = 5000;
            request.ReadWriteTimeout = 5000;
            request.Method = method;

            if (Auth != null)
            {
                request.Headers.Add("Authorization", $"Basic {Auth.Key()}");
            }

            if (request.Method != WebRequestMethods.Http.Get && reqObject != null)
            {
                using (var reqStream = request.GetRequestStream())
                using (var writer = new StreamWriter(reqStream, Encoding))
                {
                    // Write to request body
                    writer.WriteLine(JsonConvert.SerializeObject(reqObject));
                }
            }

            string responseJson = null;

            using (var response = request.GetResponse())
            {
                using (var resStream = response.GetResponseStream())
                using (var reader = new StreamReader(resStream, Encoding))
                {
                    // Read from response body
                    responseJson = reader.ReadToEnd();
                }
            }

            return string.IsNullOrWhiteSpace(responseJson) ? default(T)
                : JsonConvert.DeserializeObject<T>(responseJson);
        }
    }
}
