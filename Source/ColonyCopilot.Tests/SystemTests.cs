using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
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
        await thread.AddMessage(message);
        
        
        // Act
        var response = await thread.GetResponse();
        
        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.Not.Empty);
        Assert.That(response, Is.Not.EqualTo("Hey whats up broski, respond funny"));

        Console.WriteLine("Response: \n" + response);
    }
}