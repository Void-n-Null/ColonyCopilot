using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyCopilot.OpenAI;
using ColonyCopilot.OpenAI.Assistants;
using NUnit.Framework;

namespace ColonyCopilot.Tests;

[TestFixture]
public class ThreadTests
{
    [Test]
    public async Task ThreadCreate_ReturnsThreadWithCorrectProperties()
    {
        // Arrange
        var client = new Client(StaticConst.APIKey);
        var assistant =
            await Assistant.Create(client, "Test Assistant", "gpt-4-turbo", "You are a helpful assistant");

        // Act
        var thread = await Thread.Create(assistant);

        // Assert

        Assert.That(thread.Id, Is.Not.Null);
        Assert.That(thread.Id, Contains.Substring("thread_"));
        Assert.That(thread.Assistant, Is.SameAs(assistant));
    }

    [Test]
    public async Task ThreadAddMessage_ReturnsMessageWithCorrectProperties()
    {
        // Arrange
        var client = new Client(StaticConst.APIKey);
        var assistant = await Assistant.Create(client, "Test Assistant", "gpt-4-turbo", "You are a helpful assistant");
        
        var thread = await Thread.Create(assistant);
        
        var message = new Message
        {
            Content = "Hello, world!",
            Role = Message.RoleType.User
        };
        
        // Act
        var addedMessage = await thread.AddMessage(message);
        
        // Assert
        Assert.That(addedMessage.Content, Is.EqualTo(message.Content));
        Assert.That(addedMessage.Role, Is.EqualTo(message.Role));
    }

    [Test]
    public async Task ThreadRetrieveMessages_ReturnsListOfMessages()
    {
        // Arrange
        var client = new Client(StaticConst.APIKey);
        var assistant = await Assistant.Create(client, "Test Assistant", "gpt-4-turbo", "You are a helpful assistant");

        var thread = await Thread.Create(assistant);
        
        var messages = new List<Message>()
        {
            new Message
            {
                Content = "Hello, world!",
                Role = Message.RoleType.User
            },
            new Message
            {
                Content = "Hello, user!",
                Role = Message.RoleType.Assistant
            }
        };
        
        foreach (var message in messages)
        {
            await thread.AddMessage(message);
        }

        // Act
        var retrievedMessages = await thread.RetrieveMessages();

        // Assert
        Assert.That(retrievedMessages, Is.InstanceOf<List<Message>>());
        Assert.That(retrievedMessages.Count, Is.EqualTo(messages.Count));
    }

    [Test]
    public async Task ThreadRetrieveMessages_ReturnsListOfMessagesBeforeMessage()
    {
        // Arrange
        var client = new Client(StaticConst.APIKey);
        var assistant = await Assistant.Create(client, "Test Assistant", "gpt-4-turbo", "You are a helpful assistant");

        var thread = await Thread.Create(assistant);

        var oldMessage = new Message
        {
            Content = "Hello, world!",
            Role = Message.RoleType.User
        };

        oldMessage = await thread.AddMessage(oldMessage);
        //Delay for .5 second to ensure the messages are not added at the same time
        await Task.Delay(500);

        var newMessage = new Message
        {
            Content = "Hello, world 2!",
            Role = Message.RoleType.User
        };

        newMessage = await thread.AddMessage(newMessage);
        
        // Act
        var retrievedMessages = await thread.RetrieveMessages(oldMessage.Id);
        
        // Assert
        Assert.That(retrievedMessages, Is.InstanceOf<List<Message>>());
        Assert.That(retrievedMessages.Count, Is.EqualTo(1));
        Assert.That(retrievedMessages[0].Id, Is.EqualTo(newMessage.Id));
        Assert.That(retrievedMessages[0].Content, Is.EqualTo(newMessage.Content));
        Assert.That(retrievedMessages[0].Role, Is.EqualTo(newMessage.Role));
    }


    [Test]
    public async Task ThreadStartAndRetrieveRun_ReturnsRunWithCorrectProperties()
    {
        // Arrange
        var client = new Client(StaticConst.APIKey);
        var assistant = await Assistant.Create(client, "Test Assistant", "gpt-4-turbo", "You are a helpful assistant");

        var thread = await Thread.Create(assistant);
        
        var message = new Message
        {
            Content = "Hello, world!",
            Role = Message.RoleType.User
        };
        
        await thread.AddMessage(message);

        // Act 1
        var run = await thread.StartRun();
        
        // Assert 1
        Assert.That(run.Id, Is.Not.Null);
        Assert.That(run.Thread, Is.SameAs(thread));
        
        // Act 2
        var retrievedRun = await run.Retrieve();
        
        // Assert 2
        Assert.That(retrievedRun.Id, Is.EqualTo(run.Id));
        Assert.That(retrievedRun.Thread, Is.SameAs(thread));
    }
}