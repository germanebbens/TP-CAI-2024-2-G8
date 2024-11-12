using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ElectroHogar.Config;
using ElectroHogar.Presentacion.Utils;

namespace ElectroHogar.Persistencia.Utils
{
    public class WebHelper
    {
        private static readonly HttpClient _httpClient;
        private static readonly string _baseUrl;
        private const string _contentType = "application/json";

        // static constructor to init httpclient (singleton)
        static WebHelper()
        {
            _httpClient = new HttpClient();
            _baseUrl = ConfigHelper.GetValue("WebServiceBaseUrl");

            // needed configuration to always acept JSON response
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
            );
        }

        private static Uri BuildUri(string endpoint)
        {
            return new Uri($"{_baseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}");
        }

        private static StringContent CreateJsonContent(string jsonData)
        {
            return new StringContent(jsonData, Encoding.UTF8, _contentType);
        }

        private static HttpResponseMessage ExecuteRequest(Func<Task<HttpResponseMessage>> request)
        {
            try
            {
                return request().Result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la comunicación con el servidor: {ex.Message}", ex);
            }
        }

        public static HttpResponseMessage Get(string endpoint)
        {
            return ExecuteRequest(() => _httpClient.GetAsync(BuildUri(endpoint)));
        }

        public static HttpResponseMessage Post(string endpoint, string jsonRequest)
        {
            return ExecuteRequest(() => _httpClient.PostAsync(
                BuildUri(endpoint),
                CreateJsonContent(jsonRequest)
            ));
        }

        public static HttpResponseMessage Put(string endpoint, string jsonRequest)
        {
            return ExecuteRequest(() => _httpClient.PutAsync(
                BuildUri(endpoint),
                CreateJsonContent(jsonRequest)
            ));
        }

        public static HttpResponseMessage Patch(string endpoint, string jsonRequest)
        {
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), BuildUri(endpoint))
            {
                Content = CreateJsonContent(jsonRequest)
            };

            return ExecuteRequest(() => _httpClient.SendAsync(request));
        }

        public static HttpResponseMessage Delete(string endpoint)
        {
            return ExecuteRequest(() => _httpClient.DeleteAsync(BuildUri(endpoint)));
        }

        public static HttpResponseMessage DeleteWithBody(string endpoint, string jsonRequest)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = BuildUri(endpoint),
                Content = CreateJsonContent(jsonRequest)
            };

            return ExecuteRequest(() => _httpClient.SendAsync(request));
        }
    }
}