// HttpRequestHandler.cs
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ColonyCopilot.OpenAI.Web
{
    /// <summary>
    /// Utility class to handle HTTP requests.
    /// Takes responsibility for sending requests and returning responses.
    /// </summary>
    public static class HttpRequestHandler
    {
        /// <summary>
        /// Send a GET request and return the response.
        /// </summary>
        /// <param name="url"> The URL to send the request to. </param>
        /// <param name="headers"> The headers to include in the request. </param>
        /// <returns> The response from the request. </returns>
        /// <exception cref="WebException"> Thrown if the request fails. </exception>
        public static async Task<string> SendGetRequest(string url, Dictionary<string, string> headers = null)
        {
            using (var client = new WebClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.Headers[header.Key] = header.Value;
                    }
                }

                try
                {
                    return await client.DownloadStringTaskAsync(url);
                }
                catch (WebException ex)
                {
                    throw new WebException($"Request failed: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Send a POST request and return the response.
        /// </summary>
        /// <param name="url"> The URL to send the request to. </param>
        /// <param name="headers"> The headers to include in the request. </param>
        /// <param name="body"> The body of the request. </param>
        /// <returns> The response from the request. </returns>
        /// <exception cref="WebException"> Thrown if the request fails. </exception>
        public static async Task<string> SendPostRequest(string url, Dictionary<string, string> headers = null, string body = null)
        {
            using (var webClient = new WebClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        webClient.Headers[header.Key] = header.Value;
                    }
                }

                try
                {
                    return await webClient.UploadStringTaskAsync(url, body);
                }
                catch (WebException ex)
                {
                    throw new WebException($"Request failed: {ex.Message}", ex);
                }
            }
        }
    }
}