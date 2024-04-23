using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using ColonyCopilot.OpenAI.Functions;
using NUnit.Framework;

namespace ColonyCopilot.Tests;

[TestFixture]
public class FunctionTests
{
    public class TestFunction
    {
        [AIFunction("Test2", "Test function")]
        public static void Test(double text)
        {
            Console.WriteLine($"Text: {text}");
        }
    }
    
    [Test]
    public void FunctionManager_GetFunctionCount_ReturnsInt()
    {

        
        //Act
        var functions = FunctionManager.FindFunctions();
        
        //Assert
        Assert.That(functions, Is.Not.Null);
        Assert.That(functions, Is.TypeOf<List<AIFunction>>());
        foreach (var function in functions)
        {
            Assert.That(function, Is.TypeOf<AIFunction>());
            Console.WriteLine($"Function: {function.Name}");
            var parameters = FunctionManager.ParseParameters(function, "{\"text\":\"Hello, World!\"}");
            FunctionManager.RunFunction(function,parameters);
        }
    }
    
    [Test]
    public void FunctionManager_RunFunction()
    {
        //Arrange
        var functions = FunctionManager.FindFunctions();
        foreach (var function in functions)
        {
            if (function.Name == "Test2")
            {
                var parameters = FunctionManager.ParseParameters(function, "{\"text\": 3.2}");
                //Act
                FunctionManager.RunFunction(function, parameters);
            }
        }
    }
}