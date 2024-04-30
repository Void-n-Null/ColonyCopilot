
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace ColonyCopilot.OpenAI.Functions
{
    public static class FunctionManager
    {
        public static List<AIFunction> FindFunctions(Assembly extraAssembly = null)
        {
            //Find all methods with the AIFunction attribute. Could be on any class, not just this one.
            
            //
            var assemblies = new HashSet<Assembly>()
            {
                Assembly.GetCallingAssembly(),
                Assembly.GetExecutingAssembly()
            };
            
            if (extraAssembly != null)
            {
                assemblies.Add(extraAssembly);
            }
            var functions = new List<AIFunction>();
            
            //Go through all assemblies
            foreach (var assembly in assemblies)
            {
                //Get all methods in the assembly that have the AIFunction attribute
                var methods = assembly.GetTypes()
                    .SelectMany(t => t.GetMethods())
                    .Where(m => m.GetCustomAttributes<AIFunctionAttribute>().Any());
                
                //For each method, create an AIFunction object and add it to the list
                functions.AddRange(methods.Select(m => new AIFunction
                {
                    Method = m,
                    Name = m.GetCustomAttribute<AIFunctionAttribute>().Name,
                    Description = m.GetCustomAttribute<AIFunctionAttribute>().Description,
                    Parameters = m.GetParameters()
                }));
            }
            
            VerifyFunctions(functions);

            return functions;
        }

        /// <summary>
        /// Verifies that the functions are valid.
        /// </summary>
        /// <param name="functions"> The list of functions to verify. </param>
        /// <exception cref="Exception"> Thrown if any functions are invalid, details are provided in the message. </exception>
        private static void VerifyFunctions(List<AIFunction> functions)
        {
            var duplicates = functions.GroupBy(f => f.Name).Where(g => g.Count() > 1).ToList();
            var invalidReturnTypes = functions.Where(f => f.Method.ReturnType != typeof(string)).ToList();
            var nonStaticMethods = functions.Where(f => !f.Method.IsStatic).ToList();
            if (duplicates.Any())
            {
                throw new Exception($"Duplicate function names found: {string.Join(", ", duplicates.Select(d => d.Key))}");
            }
            if (invalidReturnTypes.Any())
            {
                throw new Exception($"Functions with invalid return types found: {string.Join(", ", invalidReturnTypes.Select(f => f.Name))}");
            }
            if (nonStaticMethods.Any())
            {
                throw new Exception($"Functions with non-static methods found: {string.Join(", ", nonStaticMethods.Select(f => f.Name))}");
            }
        }

        public static AIFunction FindFunction(string name, Assembly extraAssembly = null)
        {
            var functions = FindFunctions(extraAssembly);
            var function = functions.FirstOrDefault(f => string.Equals(f.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (function == null)
            {
                throw new Exception($"Function with name {name} not found.");
            }
            return function;
        }

        /// <summary>
        /// Parses the JSON data into the correct parameters for the specified function.
        /// </summary>
        /// <param name="targetFunction">The AIFunction object representing the function.</param>
        /// <param name="jsonData">The JSON data containing the parameter values.</param>
        /// <returns>An array of objects representing the parsed parameters.</returns>
        public static object[] ParseParameters(AIFunction targetFunction, string jsonData)
        {
            var jsonDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);
            //Numbers are deserialized as longs, so we need to convert them back to the correct type
            //Float for decimals, int for integers
            foreach (var parameter in targetFunction.Parameters)
            {
                if (parameter.ParameterType == typeof(float) && jsonDict.ContainsKey(parameter.Name))
                {
                    jsonDict[parameter.Name] = Convert.ToSingle(jsonDict[parameter.Name]);
                }
                if (parameter.ParameterType == typeof(int) && jsonDict.ContainsKey(parameter.Name))
                {
                    jsonDict[parameter.Name] = Convert.ToInt32(jsonDict[parameter.Name]);
                }
            }
            //Enums are serialized as strings, so we need to convert them back to the enum type
            foreach (var parameter in targetFunction.Parameters)
            {
                if (parameter.ParameterType.IsEnum && jsonDict.ContainsKey(parameter.Name))
                {
                    jsonDict[parameter.Name] = Enum.Parse(parameter.ParameterType, jsonDict[parameter.Name].ToString());
                }
            }
            return targetFunction.Parameters.Select(parameter => jsonDict[parameter.Name]).ToArray();
        }
        
        public static string RunFunction(AIFunction function, object[] parameters)
        {
            return function.Method.Invoke(null, parameters).ToString();
        }
    }
}
