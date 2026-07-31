// Copyright (c) DIMSI. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Newtonsoft.Json;
using NUnit.Framework;
using XrmFramework.LogicApp.Models.Actions;

namespace XrmFramework.LogicApp.Tests.Models;

/// <summary>
/// Unit tests for Logic App action models.
/// </summary>
[TestFixture]
public class ActionModelsTests
{
    // ──────────────────────────────────────────────
    //  HttpAction
    // ──────────────────────────────────────────────

    [Test]
    public void HttpAction_Type_IsHttp()
    {
        var action = new HttpAction();
        Assert.AreEqual("Http", action.Type);
    }

    [Test]
    public void HttpAction_Serialize_ContainsMethodAndUri()
    {
        var action = new HttpAction { Method = "POST", Uri = "https://api.example.com" };

        var json = JsonConvert.SerializeObject(action);

        Assert.IsTrue(json.Contains("POST"));
        Assert.IsTrue(json.Contains("https://api.example.com"));
    }

    // ──────────────────────────────────────────────
    //  ComposeAction
    // ──────────────────────────────────────────────

    [Test]
    public void ComposeAction_Type_IsCompose()
    {
        var action = new ComposeAction();
        Assert.AreEqual("Compose", action.Type);
    }

    [Test]
    public void ComposeAction_WithValue_SerializesValue()
    {
        var action = new ComposeAction { Value = "Hello, World!" };

        var json = JsonConvert.SerializeObject(action);

        Assert.IsTrue(json.Contains("Hello, World!"),
            "The serialized JSON should contain the compose value.");
    }

    // ──────────────────────────────────────────────
    //  ResponseAction
    // ──────────────────────────────────────────────

    [Test]
    public void ResponseAction_Type_IsResponse()
    {
        var action = new ResponseAction();
        Assert.AreEqual("Response", action.Type);
    }

    [Test]
    public void ResponseAction_DefaultStatusCode_Is200()
    {
        var action = new ResponseAction();
        Assert.AreEqual(200, action.StatusCode);
    }

    [Test]
    public void ResponseAction_Serialize_ContainsStatusCode()
    {
        var action = new ResponseAction { StatusCode = 404 };

        var json = JsonConvert.SerializeObject(action);

        Assert.IsTrue(json.Contains("404"), "The serialized JSON should contain the status code.");
    }

    [Test]
    public void ResponseAction_WithBody_SerializesBody()
    {
        var action = new ResponseAction
        {
            StatusCode = 200,
            Body = new { message = "OK" }
        };

        var json = JsonConvert.SerializeObject(action);

        Assert.IsTrue(json.Contains("message"));
    }

    [Test]
    public void ResponseAction_NullBody_OmittedFromJson()
    {
        var action = new ResponseAction { StatusCode = 204, Body = null };

        var json = JsonConvert.SerializeObject(action);

        Assert.IsFalse(json.Contains("\"body\""), "Null body should be omitted.");
    }

    // ──────────────────────────────────────────────
    //  ConditionAction
    // ──────────────────────────────────────────────

    [Test]
    public void ConditionAction_Type_IsIf()
    {
        var action = new ConditionAction();
        Assert.AreEqual("If", action.Type);
    }

    [Test]
    public void ConditionAction_DefaultTrueActions_IsEmpty()
    {
        var action = new ConditionAction();
        Assert.IsNotNull(action.TrueActions);
        Assert.AreEqual(0, action.TrueActions.Count);
    }

    [Test]
    public void ConditionAction_WithTrueActions_Serializes()
    {
        var action = new ConditionAction
        {
            Expression = "@equals(1, 1)"
        };
        action.TrueActions["Send_Email"] = new HttpAction { Method = "POST", Uri = "https://mail.example.com" };

        var json = JsonConvert.SerializeObject(action);

        Assert.IsTrue(json.Contains("Send_Email"));
        Assert.IsTrue(json.Contains("If"));
    }

    [Test]
    public void ConditionAction_WithElseBranch_Serializes()
    {
        var action = new ConditionAction
        {
            Expression = "@equals(1, 2)",
            ElseBranch = new ConditionElseBranch()
        };
        action.ElseBranch.Actions["Log"] = new ComposeAction { Value = "else-path" };

        var json = JsonConvert.SerializeObject(action);

        Assert.IsTrue(json.Contains("else"));
        Assert.IsTrue(json.Contains("else-path"));
    }

    // ──────────────────────────────────────────────
    //  ForEachAction
    // ──────────────────────────────────────────────

    [Test]
    public void ForEachAction_Type_IsForeach()
    {
        var action = new ForEachAction();
        Assert.AreEqual("Foreach", action.Type);
    }

    [Test]
    public void ForEachAction_Sequential_False_OperationOptionsIsNull()
    {
        var action = new ForEachAction { Sequential = false };
        Assert.IsNull(action.OperationOptions);
    }

    [Test]
    public void ForEachAction_Sequential_True_OperationOptionsIsSequential()
    {
        var action = new ForEachAction { Sequential = true };
        Assert.AreEqual("Sequential", action.OperationOptions);
    }

    [Test]
    public void ForEachAction_Serialize_ContainsForeachExpression()
    {
        var action = new ForEachAction
        {
            CollectionExpression = "@body('Parse_JSON')?['items']"
        };

        var json = JsonConvert.SerializeObject(action);

        Assert.IsTrue(json.Contains("items"));
    }

    // ──────────────────────────────────────────────
    //  ScopeAction
    // ──────────────────────────────────────────────

    [Test]
    public void ScopeAction_Type_IsScope()
    {
        var action = new ScopeAction();
        Assert.AreEqual("Scope", action.Type);
    }

    [Test]
    public void ScopeAction_DefaultActions_IsEmpty()
    {
        var action = new ScopeAction();
        Assert.IsNotNull(action.Actions);
        Assert.AreEqual(0, action.Actions.Count);
    }
}
