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
        public async Task AssistantRetrieveOrCreate_ReturnsExistingAssistant()
        {
            // Create an assistant
            var client = new Client(StaticConst.APIKey);
            var name = "Test Assistant";
            var model = "gpt-4-turbo";
            var instructions = "You are a helpful assistant.";
            var assistant = await Assistant.Create(client, name, model, instructions);

            //Act
            var retrievedAssistant = await Assistant.RetrieveOrCreate(client, name, model, instructions);
            
            //Assert
            Assert.That(retrievedAssistant.Id, Is.EqualTo(assistant.Id));
            Assert.That(retrievedAssistant.Name, Is.EqualTo(assistant.Name));
            Assert.That(retrievedAssistant.Model, Is.EqualTo(assistant.Model));
            Assert.That(retrievedAssistant.Instructions, Is.EqualTo(assistant.Instructions));
            Assert.That(retrievedAssistant.Client, Is.SameAs(assistant.Client));
        }
        
        [Test]
        public async Task AssistantRetrieveOrCreate_ReturnsNewAssistant()
        {
            // Create an assistant
            var client = new Client(StaticConst.APIKey);
            //Randomize name
            var name = Guid.NewGuid().ToString();
            var model = "gpt-4-turbo";
            var instructions = "You are a helpful assistant.";

            //Act
            var createdAssistant = await Assistant.RetrieveOrCreate(client, name, model, instructions);
            
            //Assert
            Assert.That(createdAssistant.Id, Is.Not.Null);
            Assert.That(createdAssistant.Name, Is.EqualTo(name));
            Assert.That(createdAssistant.Model, Is.EqualTo(model));
            Assert.That(createdAssistant.Instructions, Is.EqualTo(instructions));
            Assert.That(createdAssistant.Client, Is.SameAs(client));
        }
        
        [Test]
        public async Task AssistantDeleteAll_DeletesAllAssistants()
        {
            var client = new Client(StaticConst.APIKey);
            //Act
            await Assistant.DeleteAll(client);
            
            
            //Assert
            await Task.Delay(1000);
            var assistants = await Assistant.RetrieveAll(client);
            Assert.That(assistants, Is.Not.Null);
            Assert.That(assistants, Is.InstanceOf<List<Assistant>>());
            Assert.That(assistants.Count, Is.EqualTo(0));
        }
        
        [Test]
        public async Task AssistantUpdateModel_FromSettingClient()
        {
            //Arrange
            var client = new Client(StaticConst.APIKey);
            var name = "Test Assistant";
            var model = "gpt-4-turbo";
            var instructions = "You are a helpful assistant.";
            var assistant = await Assistant.Create(client, name, model, instructions);
            var newModel = "gpt-3.5-turbo-16k";
            
            //Act
            client.SetModel(newModel);
            Task.Delay(1000);
            
            //Assert
            Assert.That(assistant.Model, Is.EqualTo(newModel));
            Assert.That(assistant.Client.Model, Is.EqualTo(newModel));
            Assert.That(assistant.Client, Is.SameAs(client));
        }
    }
}