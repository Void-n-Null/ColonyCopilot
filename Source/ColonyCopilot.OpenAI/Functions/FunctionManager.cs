
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace ColonyCopilot.OpenAI.Functions
{
    public class FunctionManager
    {
        [AIFunction("Test1", "Test function")]
        public static void Test()
        {
            
        }
        
        
        
        public static List<AIFunction> FindFunctions()
        {
            //Find all methods with the AIFunction attribute. Could be on any class, not just this one.
            
            //Find all classes in the assembly
            var callingTypes = Assembly.GetCallingAssembly().GetTypes();
            var localTypes = Assembly.GetExecutingAssembly().GetTypes();
            //Find all methods in each class    
            var methods = new List<MethodInfo>();
            foreach (var type in callingTypes)
            {
                methods.AddRange(type.GetMethods());
            }
            foreach (var type in localTypes)
            {
                methods.AddRange(type.GetMethods());
            }
            
            //Find all methods with the AIFunction attribute
            var functions = new List<AIFunction>();
            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes<AIFunctionAttribute>();
                foreach (var attribute in attributes)
                {
                    functions.Add(new AIFunction()
                    {
                        Method = method,
                        Parameters = method.GetParameters(),
                        Description = attribute.Description,
                        Name = attribute.Name
                    });
                }
            }
            return functions;
        }

        public static object[] ParseParameters(AIFunction example, string jsonData)
        {
            //Parse the JSON data into the correct parameters for the function
            var parameters = new List<object>();
            var jsonDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);
            foreach (var parameter in example.Parameters)
            {
                var value = jsonDict[parameter.Name];
                parameters.Add(value);
            }
            return parameters.ToArray();
        }
        
        public static void RunFunction(AIFunction function, object[] parameters)
        {
            function.Method.Invoke(null, parameters);
        }
        
    }
}
