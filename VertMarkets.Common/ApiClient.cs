using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VertMarket.Model;


namespace VertMarket.Common
{
    public class ApiClient
    {

        private readonly HttpClient _HttpClient;
        private Uri _BaseEndPoint { get; set; }

        private string Token { get; set; }
        public ApiClient(Uri BaseEndpoint)
        {
            _BaseEndPoint = BaseEndpoint;
            _HttpClient = new HttpClient();

            if (string.IsNullOrEmpty(Token)) { 
            var resultObject = GetAsync<ApiResponse>("api/token");
            Token = resultObject.Result.Token;
        }
            
        }

        //private async Task<ApiResponse> GenerateToken(string RelativePath)
        //{
        //    HttpResponseMessage response = null;
        //    var RequestUrl=CreateUri(RelativePath);           
        //    response = await _HttpClient.GetAsync(RequestUrl, HttpCompletionOption.ResponseHeadersRead);                           
        //    response.EnsureSuccessStatusCode();
        //    var data = response.Content.ReadAsStringAsync();
        //    return data.Result ;
        //}



        public async Task<T> GetAsync<T>(string relativePath)
        {
            var requestUrl = CreateUri(relativePath);
            var response = await _HttpClient.GetAsync(requestUrl.ToString(), HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(data.Result);
        }

        private Uri CreateUri(string relativePath)
        {
            relativePath=relativePath.Replace("{token}", Token);
            var endpoint = new Uri(_BaseEndPoint, relativePath);
            var uriBuilder = new UriBuilder(endpoint);
           // uriBuilder.Query = queryString;
            return uriBuilder.Uri;
        }


        public async Task<HttpResponseMessage> PostAsync<T>(string relativePath, T content)
        {
            var requestUrl=CreateUri(relativePath);
            var response = await _HttpClient.PostAsync(requestUrl.ToString(), CreateHttpContent(content));
            response.EnsureSuccessStatusCode();

            return response;
        }


        private HttpContent CreateHttpContent<T>(T content)
        {
            var json = JsonConvert.SerializeObject(content);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

    }
}
