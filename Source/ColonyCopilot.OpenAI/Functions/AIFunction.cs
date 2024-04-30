using System;
using System.Reflection;
using System.Text;

namespace ColonyCopilot.OpenAI.Functions
{
    /*
     EXAMPLE STRING FUNCTION
     * {
        "name": "get_current_temperature",
        "description": "Get the current temperature for a specific location",
        "parameters": {
          "type": "object",
          "properties": {
            "location": {
              "type": "string",
              "description": "The city and state, e.g., San Francisco, CA"
            },
            "unit": {
              "type": "string",
              "enum": ["Celsius", "Fahrenheit"],
              "description": "The temperature unit to use. Infer this from the user's location."
            }
          },
          "required": ["location", "unit"]
        }
      }
     */
    public class AIFunction
    {
        public MethodInfo Method { get; set; }
        public ParameterInfo[] Parameters { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            //Create a very specific string representation of the function
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append($"\"name\": \"{Name}\",");
            sb.Append($"\"description\": \"{Description}\",");
            sb.Append("\"parameters\": {");
            sb.Append("\"type\": \"object\",");
            sb.Append("\"properties\":");
            if (Parameters.Length > 0)
            {
                SetupParameters(sb);
            } else
            {
                sb.Append("{},");
            }

            sb.Append("\"required\":");
            sb.Append("[");

            if (Parameters.Length > 0)
            {
                foreach (var parameter in Parameters)
                {
                    sb.Append($"\"{parameter.Name}\",");
                }
                //Remove last comma
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]");
            sb.Append("}");
            sb.Append("}");
            return sb.ToString();
        }

        private void SetupParameters(StringBuilder sb)
        {
            sb.Append("{");
            foreach (var parameter in Parameters)
            {
                sb.Append($"\"{parameter.Name}\": {{");
                sb.Append($"\"type\": \"{GetParameterType(parameter)}\",");
                var description = parameter.GetCustomAttribute<AIParameterAttribute>()?.Description;
                if (description != null)
                {
                    sb.Append($"\"description\": \"{description}\",");
                }
                else
                {
                    sb.Append($"\"description\": \"{parameter.Name}\",");
                }
                sb.Append(CreateEnumString(parameter));
                sb.Append("},");
            }
            //Remove last comma
            sb.Remove(sb.Length - 1, 1);
            sb.Append("},");
        }

        private static string GetParameterType(ParameterInfo parameter)
        {
            switch (parameter.ParameterType.Name.ToLower())
            {
                case "string":
                    return "string";
                case "int32":
                    return "number";
                case "int64":
                    return "number";
                case "double":
                    return "number";
                case "boolean":
                    return "boolean";
                default:
                    return "string";
            }
        }

        private static string CreateEnumString(ParameterInfo parameter)
        {
            if (parameter.ParameterType.IsEnum)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\"enum\": [");
                foreach (var value in parameter.ParameterType.GetEnumNames())
                {
                    sb.Append($"\"{value}\",");
                }

                //Remove last comma
                sb.Remove(sb.Length - 1, 1);
                sb.Append("]");
                return sb.ToString();
            }

            return "";
        }

        public static string CreateTool(AIFunction function)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append($"\"type\": \"function\",");
            sb.Append($"\"function\": {function.ToString()}");
            sb.Append("}");
            return sb.ToString();
        }
    }
}
