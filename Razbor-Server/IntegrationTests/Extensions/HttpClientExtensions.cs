using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> SendPostAsync<TRequest>(
            this HttpClient client,
            string uri,
            TRequest body,
            string? token = null,
            params (string name, string value)[] queryParams)
        {
            string query = string.Empty;
            if (queryParams.Length > 0)
            {
                query = "?";
                foreach (var param in queryParams)
                    query += $"{param.name}={param.value}&";
                query.TrimEnd('&');
            }
            var fullUri = $"{uri}{query}";

            var request = new HttpRequestMessage(HttpMethod.Post, fullUri);
            var json = JsonConvert.SerializeObject(body);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            if (token is not null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await client.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> SendPostAsync(
            this HttpClient client,
            string uri,
            string? token = null,
            params (string name, string value)[] queryParams)
        {
            string query = string.Empty;
            if (queryParams.Length > 0)
            {
                query = "?";
                foreach (var param in queryParams)
                    query += $"{param.name}={param.value}&";
                query.TrimEnd('&');
            }
            var fullUri = $"{uri}{query}";

            var request = new HttpRequestMessage(HttpMethod.Post, fullUri);

            if (token is not null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await client.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> SendPutAsync<TRequest>(this HttpClient client, string uri, TRequest body, string? token = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, uri);
            var json = JsonConvert.SerializeObject(body);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            if (token is not null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await client.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> SendGetAsync(
            this HttpClient client,
            string uri,
            string? token = null,
            params (string name, string value)[] queryParams)
        {
            string query = string.Empty;
            if (queryParams.Length > 0)
            {
                query = "?";
                foreach (var param in queryParams)
                    query += $"{param.name}={param.value}&";
                query.TrimEnd('&');
            }

            var fullUri = $"{uri}{query}";
            var request = new HttpRequestMessage(HttpMethod.Get, fullUri);
            if (token is not null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await client.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> SendDeleteAsync(this HttpClient client, string uri, string? token = null)        
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, uri);
            if (token is not null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request);
            return response;
        }

        public static async Task<TResponse> GetObject<TResponse>(this HttpResponseMessage response)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(responseString);
        }
    }
}
