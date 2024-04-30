using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using ColonyCopilot.OpenAI.Functions;
using NUnit.Framework;

namespace ColonyCopilot.Tests.Unit;



[TestFixture]
public class FunctionTests
{
    [AIFunction("ExplicitTest", "A function created explicitly for use in unit tests")]
    public static string ExplicitTest([AIParameter("The text to display")] TestEnum text)
    {
        return $"The text was: {text.ToString()}";
    }
    
    public enum TestEnum
    {
        Test1,
        Test2
    }
    
    [Test]
    public void FunctionManager_RunFunctionWithParsedParameters()
    {
        //Arrange
        var testFunction = FunctionManager.FindFunction("ExplicitTest");
        var jsonData = "{\"text\": \"Test1\"}";
        var parameters = FunctionManager.ParseParameters(testFunction, jsonData);
        
        //Act
        var result = testFunction.Method.Invoke(null, parameters);
        
        //Display
        Console.WriteLine(result);
    }
    
    [Test]
    public void FunctionManager_AllFunctionsToString()
    {
        //Arrange + Act
        var functions = FunctionManager.FindFunctions();
        
        //Assert
        Assert.That(functions, Is.Not.Null);
        Assert.That(functions.Count, Is.GreaterThan(0));
        Assert.That(functions[0].ToString(), Is.Not.Null);
        Assert.That(functions[0].ToString(), Is.Not.Empty);

        
        //Display
        foreach (var function in functions)
        {
            Console.WriteLine(function.ToString());
        }
    }
}