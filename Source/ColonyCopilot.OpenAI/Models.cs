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
            var response = await Web.HttpRequestHandler.SendGetRequest(Endpoints.Models, client.DefaultHeaders);
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