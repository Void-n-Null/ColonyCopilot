using System;
using System.Collections.Generic;
using NUnit.Framework;
using ColonyCopilot.OpenAI.Assistants;
using System.Threading.Tasks;
using NUnit.Framework.Internal;
using ColonyCopilot.OpenAI;

namespace ColonyCopilot.Tests
{
    [TestFixture]
    public class AssistantTests
    {
        [Test]
        public async Task AssistantCreate_ReturnsAssistantWithCorrectProperties()
        {
            // Arrange
            var client = new Client(StaticConst.APIKey);
            var name = "Test Assistant";
            var model = "gpt-4-turbo";
            var instructions = "You are a helpful assistant.";

            // Act
            var assistant = await Assistant.Create(client, name, model, instructions);

            // Assert
            Assert.That(assistant.Id, Is.Not.Null);
            Assert.That(assistant.Name, Is.EqualTo(name));
            Assert.That(assistant.Model, Is.EqualTo(model));
            Assert.That(assistant.Instructions, Is.EqualTo(instructions));
            Assert.That(assistant.Client, Is.SameAs(client));
        }

        [Test]
        public async Task AssistantRetrieveAll_ReturnsListOfAssistants()
        {
            // Arrange
            var client = new Client(StaticConst.APIKey);

            // Act
            var assistants = await Assistant.RetrieveAll(client);

            // Assert
            Assert.That(assistants, Is.InstanceOf<List<Assistant>>());
        }


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
            
            // Add more test methods here
        }
    }
}