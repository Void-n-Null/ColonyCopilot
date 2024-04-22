using NUnit.Framework;
using RimOpenAI.Assistants;
using System.Threading.Tasks;
using RimOpenAI;

namespace ColonyCopilot.Tests
{
    [TestFixture]
    public class AssistantTests
    {
        [Test]
        public async Task Create_ReturnsAssistantWithCorrectProperties()
        {
            // Arrange
            var client = new Client(StaticConst.APIKey);
            var name = "Test Assistant";
            var model = "gpt-4";
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

        // Add more test methods here
    }
}