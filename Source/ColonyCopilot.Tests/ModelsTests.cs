using System;
using System.Threading.Tasks;
using ColonyCopilot.OpenAI;
using NUnit.Framework;

namespace ColonyCopilot.Tests;

[TestFixture]
public class ModelsTests
{
    [Test]
    public async Task RetrieveTest()
    {
        // Arrange
        var client = new Client(StaticConst.APIKey);
        
        // Act
        var models = await Models.Retrieve(client);
        
        // Assert
        Assert.That(models, Is.Not.Null);
        
        foreach (var model in models)
        {
            Assert.That(model.id, Is.Not.Null);
            Console.WriteLine(model.id);
        }
    }
}