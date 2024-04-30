// Thread.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ColonyCopilot.OpenAI.Functions;
using ColonyCopilot.OpenAI.Web;
using Newtonsoft.Json;
using ColonyCopilot.OpenAI.ResponseModels;
using JetBrains.Annotations;

namespace ColonyCopilot.OpenAI.Assistants
{
    public class Thread
    {
        public string Id { get; private set; }
        public Assistant Assistant { get; private set; }

        private float _defaultTimeout = 10f;
        
        public delegate void RanToolCallDelegate(string function);
        public event RanToolCallDelegate RanToolCall;
        public bool IsRunning { get; private set; }
        /// <summary>
        /// Creates a new thread with the specified assistant.
        /// </summary>
        /// <param name="assistant"> The assistant to use for the thread. </param>
        /// <param name="functionAssembly"> Optional. The assembly containing the functions to use. </param>
        /// <returns> The created thread. </returns>
        public static async Task<Thread> Create(Assistant assistant, float timeout = 10f)
        {
            var response = await HttpRequestHandler.SendPostRequest("https://api.openai.com/v1/threads", assistant.Client.DefaultHeaders);
            var responseData = JsonConvert.DeserializeObject<ThreadResponse>(response);
            var thread = new Thread
            {
                Id = responseData.Id,
                Assistant = assistant,
                _defaultTimeout = timeout
            };
            return thread;
        }

        private void VerifySelf()
        {
            if (Id == null)
            {
                throw new Exception("Thread not initialized. Id is null.");
            }

            if (Assistant == null)
            {
                throw new Exception("Thread not initialized. Assistant is null.");
            }

            if (Assistant.Client == null)
            {
                throw new Exception("Thread not initialized. Client is null.");
            }

            if (Assistant.Client.DefaultHeaders == null)
            {
                throw new Exception("Thread not initialized. DefaultHeaders is null.");
            }
            
            if (Assistant.Client.DefaultHeaders.Count == 0)
            {
                throw new Exception("Thread not initialized. DefaultHeaders is empty.");
            }
        }
        
        /// <summary>
        /// Adds a message to the thread.
        /// </summary>
        /// <param name="message">  The message to add. </param>
        /// <returns> The added message with an ID </returns>
        public async Task<Message> AddMessage(Message message)
        {
            VerifySelf();
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            
            var body = JsonConvert.SerializeObject(new
            {
                role = message.Role.ToString().ToLower(),
                content = message.Content
            });
            
            var response = await HttpRequestHandler.SendPostRequest($"https://api.openai.com/v1/threads/{Id}/messages",
                                                                            Assistant.Client.DefaultHeaders,
                                                                            body);

            MessageResponse responseObject;
            try
            {
                responseObject = JsonConvert.DeserializeObject<MessageResponse>(response);
            } catch (Exception e)
            {
                throw new JsonSerializationException("Error deserializing AddMessage response: " + e);
            }

            Message.RoleType role;
            try
            {
                bool hasRole = Enum.TryParse(responseObject.Role, true, out Message.RoleType parsedRole);
                role = hasRole ? parsedRole : message.Role;
            } catch (Exception e)
            {
                throw new InvalidDataException("Error parsing role: " + e);
            }
            
            return new Message
            {
                Role = role,
                Content = responseObject.Content[0].Text.Value,
                Id = responseObject.Id
            };
        }

        /// <summary>
        /// Retrieves all messages in the thread.
        /// </summary>
        /// <param name="cutoffMessageID"> Optional. If provided, only messages added after this ID will be returned. </param>
        /// <returns> A list of messages in the thread. </returns>
        public async Task<List<Message>> RetrieveMessages(string cutoffMessageID = null)
        {
            VerifySelf();
            var queryParameters = new Dictionary<string, string>();
            if (cutoffMessageID != null)
            {
                queryParameters.Add("before", cutoffMessageID);
            }
    
            var response = await HttpRequestHandler.SendGetRequest($"https://api.openai.com/v1/threads/{Id}/messages",
                                                                            Assistant.Client.DefaultHeaders,
                                                                            queryParameters);
            var responseObject = JsonConvert.DeserializeObject<MessageResponseList>(response);
            var messages = new List<Message>();
            foreach (var responseMessage in responseObject.Data)
            {
                Enum.TryParse(responseMessage.Role, true, out Message.RoleType parsedRole);
                messages.Add(new Message
                {
                    Role = parsedRole,
                    Content = responseMessage.Content[0].Text.Value,
                    Id = responseMessage.Id
                });
            }
            return messages;
        }
        
        /// <summary>
        /// Start a run on the thread.
        /// </summary>
        /// <param name="instructions"> Optional. The instructions for the run. </param>
        /// <returns> The created run. </returns>
        public async Task<Run> StartRun(string instructions = "")
        {
            VerifySelf();
            return await Run.Create(Assistant, this, instructions);
        }
        
        private async Task<Message> GetLatestMessage()
        {
            var messages = await RetrieveMessages();
            if (messages.Count == 0)
            {
                return null;
            }
            return messages[0];
        }
        
        /// <summary>
        /// Create a run, execute it, and return the first message it created.
        /// It also handles tool calls! It will execute code in the tool calls and submit the outputs.
        /// </summary>
        /// <returns> The response from the assistant. </returns>
        /// <exception cref="Exception"> Thrown if the execution fails. </exception>
        /// <exception cref="TimeoutException"> Thrown if the execution times out. </exception>
        public async Task<ExecutionResult> GetResponse(string instructions = "")
        {
            VerifySelf();
            float time = 0f;
            var latestMessageBeforeRun = await GetLatestMessage();
            var run = await StartRun(instructions);
            int stepsTaken = 0;
            var toolOutputs  = new List<Dictionary<string, string>>();
            IsRunning = true;
            while (run.Status != "completed")
            {
                var inputOutputDict = new Dictionary<string, string>();
                stepsTaken++;
                if (run.Status == "failed")
                {
                    IsRunning = false;
                    throw new Exception("Run failed. Error: " + $"{run.LastError.Code} - {run.LastError.Message}");
                }

                if (run.Status == "requires_action")
                {
                    var steps = await run.RetrieveSteps();
                    var mostRecentStep = steps[0];
                    var outputs = ExecuteToolCalls(mostRecentStep, out var functionStrings);
                    
                    for (var i = 0; i < outputs.Count; i++)
                    {
                        var output = outputs.ElementAt(i);
                        var functionString = functionStrings[i];
                        inputOutputDict.Add(functionString, output.Value);
                    }
                    toolOutputs.Add(inputOutputDict);
                    run = await SubmitToolCalls(run, outputs);
                    time = 0f;
                }
                await Task.Delay(500);
                time += 0.5f;
                if (time >= _defaultTimeout)
                {
                    IsRunning = false;
                    throw new TimeoutException($"Run timed out. Last status: {run.Status} after {stepsTaken} steps.");
                }
                run = await run.Retrieve();
            }

            List<Message> newMessages;
            if (latestMessageBeforeRun != null)
            {
                newMessages = await RetrieveMessages(latestMessageBeforeRun?.Id);
            }
            else
            {
                newMessages = await RetrieveMessages();
            }
            
            var response = newMessages[0].Content;
            byte[] nonUnicodeBytes = System.Text.Encoding.Default.GetBytes(response);
            string finalizedContent = System.Text.Encoding.UTF8.GetString(nonUnicodeBytes);
            IsRunning = false;
            return new ExecutionResult(finalizedContent, toolOutputs.Count, toolOutputs);
        }

        public Dictionary<string,string> ExecuteToolCalls(RunStep runStep, out List<string> functionStrings)
        {
            var dict = new Dictionary<string, string>();
            var functionObjects = runStep.StepDetails.ToolCalls;
            functionStrings = new List<string>();
            var functions = Assistant.Functions;
            
            foreach (var functionObject in functionObjects)
            {
                //Setup
                var function = functions.Find(f => f.Name == functionObject.Function.Name);
                var parameters = FunctionManager.ParseParameters(function, functionObject.Function.Arguments);
                var functionString = $"{function.Name}({string.Join(", ", parameters)})";
                functionStrings.Add(functionString);
                //Execute
                RanToolCall?.Invoke(functionString);
                var result = FunctionManager.RunFunction(function, parameters);

                
                //Add to dictionary
                dict.Add(functionObject.Id, result);
            }
            return dict;
        }
        
        public async Task<Run> SubmitToolCalls(Run run, Dictionary<string,string> IDsAndOutputs)
        {
            //Convert the dictionary to a list of objects
            var toolOutputs = IDsAndOutputs.Select(pair => new
            {
                tool_call_id = pair.Key,
                output = pair.Value
            }).ToList();
            
            var body = JsonConvert.SerializeObject(new
            {
                tool_outputs = toolOutputs
            });
            Console.WriteLine(body);
            var response = await HttpRequestHandler.SendPostRequest($"https://api.openai.com/v1/threads/{Id}/runs/{run.Id}/submit_tool_outputs", Assistant.Client.DefaultHeaders, body);
            var newRun = JsonConvert.DeserializeObject<Run>(response);
            newRun.Thread = this;
            return newRun;
        }
    }
}