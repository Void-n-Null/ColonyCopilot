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
        var assistant =
            await Assistant.RetrieveOrCreate(client, "J Agent", "gpt-4-turbo", "You are a helpful assistant");
        var thread = await Thread.Create(assistant);
        var message = new Message
        {
            Content = "Hey whats up broski, what is the current value of J",
            Role = Message.RoleType.User
        };
        await thread.AddMessage(message);


        // Act
        var response = await thread.GetResponse();
        
        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Content, Is.Not.Null);
        Assert.That(response.Content, Is.Not.Empty);
        Assert.That(response.ToolsUsedCount, Is.GreaterThanOrEqualTo(0));
        Assert.That(response.ToolOutputs, Is.Not.Null);

        Console.WriteLine("Response: \n" + response.Content);
    }

    [Test]
    public async Task SystemAssistantCanRespondWithSpecificRunInstructions_ReturnsMessageContent()
    {
        // Arrange
        var client = new Client(StaticConst.APIKey);
        var assistant =
            await Assistant.RetrieveOrCreate(client, "J Agent", "gpt-4-turbo", "You are a helpful assistant");
        var thread = await Thread.Create(assistant);
        var response = await thread.GetResponse("Respond in cryptic, creepy, threats");

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Content, Is.Not.Null);
        Assert.That(response.Content, Is.Not.Empty);
        Assert.That(response.ToolsUsedCount, Is.GreaterThanOrEqualTo(0));

        Console.WriteLine("Response: \n" + response.Content);
    }
}