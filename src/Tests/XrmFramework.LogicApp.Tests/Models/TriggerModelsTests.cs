// Copyright (c) DIMSI. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Newtonsoft.Json;
using NUnit.Framework;
using XrmFramework.LogicApp.Models.Triggers;

namespace XrmFramework.LogicApp.Tests.Models;

/// <summary>
/// Tests unitaires pour les modèles de triggers Logic App.
/// </summary>
[TestFixture]
public class TriggerModelsTests
{
    // ──────────────────────────────────────────────
    //  HttpRequestTrigger
    // ──────────────────────────────────────────────

    [Test]
    public void HttpRequestTrigger_Type_IsRequest()
    {
        var trigger = new HttpRequestTrigger();
        Assert.AreEqual("Request", trigger.Type);
    }

    [Test]
    public void HttpRequestTrigger_Kind_IsHttp()
    {
        var trigger = new HttpRequestTrigger();
        Assert.AreEqual("Http", trigger.Kind);
    }

    [Test]
    public void HttpRequestTrigger_DefaultSchema_IsNull()
    {
        var trigger = new HttpRequestTrigger();
        Assert.IsNull(trigger.Schema);
    }

    [Test]
    public void HttpRequestTrigger_Serialize_ContainsTypeAndKind()
    {
        var trigger = new HttpRequestTrigger();

        var json = JsonConvert.SerializeObject(trigger);

        Assert.IsTrue(json.Contains("\"type\""));
        Assert.IsTrue(json.Contains("Request"));
        Assert.IsTrue(json.Contains("Http"));
    }

    [Test]
    public void HttpRequestTrigger_WithSchema_SerializesSchema()
    {
        var trigger = new HttpRequestTrigger
        {
            Schema = new { type = "object", properties = new { } }
        };

        var json = JsonConvert.SerializeObject(trigger);

        Assert.IsTrue(json.Contains("schema"));
    }

    // ──────────────────────────────────────────────
    //  RecurrenceTrigger
    // ──────────────────────────────────────────────

    [Test]
    public void RecurrenceTrigger_Type_IsRecurrence()
    {
        var trigger = new RecurrenceTrigger();
        Assert.AreEqual("Recurrence", trigger.Type);
    }

    [Test]
    public void RecurrenceTrigger_DefaultSchedule_Initialized()
    {
        var trigger = new RecurrenceTrigger();

        Assert.IsNotNull(trigger.Schedule);
        Assert.AreEqual(1, trigger.Schedule.Interval);
        Assert.AreEqual("Hour", trigger.Schedule.Frequency);
    }

    [Test]
    public void RecurrenceTrigger_Serialize_ContainsRecurrenceBlock()
    {
        var trigger = new RecurrenceTrigger();
        trigger.Schedule.Interval = 5;
        trigger.Schedule.Frequency = "Minute";

        var json = JsonConvert.SerializeObject(trigger);

        Assert.IsTrue(json.Contains("recurrence"));
        Assert.IsTrue(json.Contains("5"));
        Assert.IsTrue(json.Contains("Minute"));
    }

    [Test]
    public void RecurrenceTrigger_NullStartTime_OmittedFromJson()
    {
        var trigger = new RecurrenceTrigger();
        trigger.Schedule.StartTime = null;

        var json = JsonConvert.SerializeObject(trigger);

        Assert.IsFalse(json.Contains("startTime"), "Null startTime should be omitted.");
    }

    [Test]
    public void RecurrenceTrigger_WithTimeZone_SerializesTimeZone()
    {
        var trigger = new RecurrenceTrigger();
        trigger.Schedule.TimeZone = "Romance Standard Time";

        var json = JsonConvert.SerializeObject(trigger);

        Assert.IsTrue(json.Contains("Romance Standard Time"));
    }

    // ──────────────────────────────────────────────
    //  RecurrenceSchedule defaults
    // ──────────────────────────────────────────────

    [Test]
    public void RecurrenceSchedule_DefaultInterval_IsOne()
    {
        var schedule = new RecurrenceSchedule();
        Assert.AreEqual(1, schedule.Interval);
    }

    [Test]
    public void RecurrenceSchedule_DefaultFrequency_IsHour()
    {
        var schedule = new RecurrenceSchedule();
        Assert.AreEqual("Hour", schedule.Frequency);
    }
}
