using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ColonyCopilot.OpenAI
{
    public class Models
    {
        public static async Task<List<ModelResponse>> Retrieve(Client client)
        {
            // Retrieve models from OpenAI API
            var url = "https://api.openai.com/v1/models";
            var response = await Web.HttpRequestHandler.SendGetRequest(url, client.DefaultHeaders);
            var modelList = JsonConvert.DeserializeObject<ModelList>(response);
            return modelList.Data;
        }
    }

    public class ModelList
    {
        public List<ModelResponse> Data { get; set; }
    }

    public class ModelResponse
    {
        public string id;
    }
}