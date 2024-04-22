using System.Net;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace RimOpenAI.Web
{
    /// <summary>
    /// Utility class to handle UnityWebRequest.
    /// Takes responsibility for sending requests and returning responses.
    /// </summary>
    public static class UwrHandler
    {
        /// <summary>
        /// Send a UnityWebRequest and return the response.
        /// </summary>
        /// <param name="webRequest"> The UnityWebRequest to send. </param>
        /// <param name="body"> The body of the request. </param>
        /// <returns> The response from the request. </returns>
        /// <exception cref="WebException"> Thrown if the request fails. </exception>
        public static async Task<object> SendRequest(UnityWebRequest webRequest, string body = null)
        {

            if (body != null)
            {
                webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));
            }
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SendWebRequest();
            
            while (!webRequest.isDone)
            {
                await Task.Yield();
            }
            
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                throw new WebException(webRequest.error + ":\n" + webRequest.downloadHandler.text);
            }
            
            return webRequest.downloadHandler.text;
        }
    }
}
