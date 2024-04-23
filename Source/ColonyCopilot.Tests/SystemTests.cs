using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyCopilot.OpenAI;
using ColonyCopilot.OpenAI.Assistants;
using NUnit.Framework;

namespace ColonyCopilot.Tests;

[TestFixture]
public class SystemTests
{
    [Test]
    public async Task SystemAssistantCanRespond_ReturnsMessageContent()
    {
        // Arrange
        var client = new Client(StaticConst.APIKey);
        var assistant = await Assistant.RetrieveOrCreate(client, "Test Assistant", "gpt-4-turbo", "You are a helpful assistant");
        var thread = await Thread.Create(assistant);
        var message = new Message
        {
            Content = "Hey whats up broski, respond funny",
            Role = Message.RoleType.User
        };
        message = await thread.AddMessage(message);

        Console.WriteLine("Message ID: " + message.Id);
        
        
        // Act
        var run = await thread.StartRun();
        while (run.Status != "completed")
        {
            if (run.Status == "failed")
            {
                throw new Exception("Run failed. Error: " + $"{run.LastError.Code} - {run.LastError.Message}");
            }
            await Task.Delay(1000);
            run = await run.Retrieve();
            Console.WriteLine("Waiting for run to complete... Current status: " + run.Status);
        }
        var newMessages = await thread.RetrieveMessages(message.Id);
        var response = newMessages[0].Content;
        
        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(newMessages, Is.InstanceOf<List<Message>>());
        Assert.That(response, Is.Not.Empty);
        Console.WriteLine($"Assistant Response: \n {response}");
        
    }
}