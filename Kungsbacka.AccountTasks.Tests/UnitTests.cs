using System;
using System.Text.Json;
using Xunit;

namespace Kungsbacka.AccountTasks.Tests
{
    public class UnitTests
    {
        // --- Existing tests ---

        [Fact]
        public void DeserializeWithTime()
        {
            string json = "[{\"TaskName\":\"SamlId\",\"RetryCount\":42,\"WaitUntil\":\"2015-04-20T12:00:44\",\"Id\":4293}]";
            var deserializedTasks = TaskFactory.DeserializeTasks(json);
            var task = (SamlIdTask)deserializedTasks[0];
            Assert.NotNull(task.WaitUntil);
            Assert.Equal("2015-04-20T12:00:44", ((DateTime)task.WaitUntil).ToString("s"));
        }

        [Fact]
        public void SerializeDeserializeWithRetryCount()
        {
            var task = new MsolEnableSyncTask()
            {
                Id = 5
            };
            DateTime date = DateTime.Now.AddDays(1);
            task.WaitUntil = date;
            task.RetryCount = 3;
            string json = task.ToJson();
            var deserializedTasks = TaskFactory.DeserializeTasks(json);
            var msolLicenseTask = (MsolEnableSyncTask)deserializedTasks[0];
            Assert.Equal(date.ToString("s"), msolLicenseTask.WaitUntil?.ToString("s"));
        }

        [Fact]
        public void SerializeDeserializeInvalidTask()
        {
            var task = new BasicTask("-InvalidTask-");
            string json = task.ToJson();
            Assert.Throws<ArgumentException>(() => TaskFactory.DeserializeTasks(json));
        }

        [Fact]
        public void SerializeDeserializeMsolLicenseGroup()
        {
            var task = new MsolLicenseGroupTask([Guid.Empty]);
            var json = task.ToJson();
            var deserializedTasks = TaskFactory.DeserializeTasks(json);
            Assert.Single(((MsolLicenseGroupTask)deserializedTasks[0]).LicenseGroups);
        }

        [Fact]
        public void SerializeDeserializeSequenceTask()
        {
            var task = new MicrosoftOnlineWithMailboxTask(
                MailboxType.Employee,
                [Guid.Empty]
            );
            var json = task.ToJson();
            var deserializedTasks = TaskFactory.DeserializeTasks(json);
            Assert.Equal(MailboxType.Employee, ((MicrosoftOnlineWithMailboxTask)deserializedTasks[0]).Type);
        }

        // --- Conditional serialization (ShouldSerialize* / JsonIgnore) ---

        [Fact]
        public void DisplayName_NotIncludedInJson()
        {
            var task = new MsolEnableSyncTask();
            var doc = JsonDocument.Parse(task.ToJson());
            Assert.False(doc.RootElement.TryGetProperty("DisplayName", out _));
        }

        [Fact]
        public void Id_NotIncludedInJson_WhenNull()
        {
            var task = new MsolEnableSyncTask();
            var doc = JsonDocument.Parse(task.ToJson());
            Assert.False(doc.RootElement.TryGetProperty("Id", out _));
        }

        [Fact]
        public void Id_IncludedInJson_WhenPositive()
        {
            var task = new MsolEnableSyncTask { Id = 42 };
            var doc = JsonDocument.Parse(task.ToJson());
            Assert.True(doc.RootElement.TryGetProperty("Id", out var prop));
            Assert.Equal(42, prop.GetInt64());
        }

        [Fact]
        public void RetryCount_NotIncludedInJson_WhenNull()
        {
            var task = new MsolEnableSyncTask();
            var doc = JsonDocument.Parse(task.ToJson());
            Assert.False(doc.RootElement.TryGetProperty("RetryCount", out _));
        }

        [Fact]
        public void RetryCount_NotIncludedInJson_WhenZero()
        {
            var task = new MsolEnableSyncTask { RetryCount = 0 };
            var doc = JsonDocument.Parse(task.ToJson());
            Assert.False(doc.RootElement.TryGetProperty("RetryCount", out _));
        }

        [Fact]
        public void RetryCount_IncludedInJson_WhenPositive()
        {
            var task = new MsolEnableSyncTask { RetryCount = 3 };
            var doc = JsonDocument.Parse(task.ToJson());
            Assert.True(doc.RootElement.TryGetProperty("RetryCount", out var prop));
            Assert.Equal(3, prop.GetInt64());
        }

        [Fact]
        public void WaitUntil_NotIncludedInJson_WhenNull()
        {
            var task = new MsolEnableSyncTask();
            var doc = JsonDocument.Parse(task.ToJson());
            Assert.False(doc.RootElement.TryGetProperty("WaitUntil", out _));
        }

        [Fact]
        public void WaitUntil_NotIncludedInJson_WhenInThePast()
        {
            var task = new MsolEnableSyncTask { WaitUntil = DateTime.Now.AddDays(-1) };
            var doc = JsonDocument.Parse(task.ToJson());
            Assert.False(doc.RootElement.TryGetProperty("WaitUntil", out _));
        }

        [Fact]
        public void WaitUntil_IncludedInJson_WhenInTheFuture()
        {
            var future = DateTime.Now.AddDays(1);
            var task = new MsolEnableSyncTask { WaitUntil = future };
            var doc = JsonDocument.Parse(task.ToJson());
            Assert.True(doc.RootElement.TryGetProperty("WaitUntil", out var prop));
            Assert.Equal(future.ToString("s"), prop.GetDateTime().ToString("s"));
        }

        [Fact]
        public void SkipSyncCheck_NotIncludedInJson_WhenFalse()
        {
            var task = new MsolLicenseGroupTask([Guid.Empty], skipSyncCheck: false, skipDynamicGroupCheck: false);
            var doc = JsonDocument.Parse(task.ToJson());
            Assert.False(doc.RootElement.TryGetProperty("SkipSyncCheck", out _));
        }

        [Fact]
        public void SkipSyncCheck_IncludedInJson_WhenTrue()
        {
            var task = new MsolLicenseGroupTask([Guid.Empty], skipSyncCheck: true, skipDynamicGroupCheck: false);
            var doc = JsonDocument.Parse(task.ToJson());
            Assert.True(doc.RootElement.TryGetProperty("SkipSyncCheck", out var prop));
            Assert.True(prop.GetBoolean());
        }

        [Fact]
        public void WaitTask_Minutes_NotIncludedInJson_WhenNull()
        {
            var task = new WaitTask(null);
            var doc = JsonDocument.Parse(task.ToJson());
            Assert.False(doc.RootElement.TryGetProperty("Minutes", out _));
        }

        [Fact]
        public void WaitTask_Minutes_IncludedInJson_WhenSet()
        {
            var task = new WaitTask(5);
            var doc = JsonDocument.Parse(task.ToJson());
            Assert.True(doc.RootElement.TryGetProperty("Minutes", out var prop));
            Assert.Equal(5, prop.GetInt64());
        }

        // --- Enum serialization ---

        [Fact]
        public void MailboxType_SerializedAsString()
        {
            var task = new MicrosoftOnlineWithMailboxTask(MailboxType.Employee, [Guid.Empty]);
            var doc = JsonDocument.Parse(task.ToJson());
            Assert.True(doc.RootElement.TryGetProperty("Type", out var prop));
            Assert.Equal(JsonValueKind.String, prop.ValueKind);
            Assert.Equal("Employee", prop.GetString());
        }

        [Fact]
        public void SequenceTask_SubTasks_RoundTrip()
        {
            var task = new MicrosoftOnlineWithMailboxTask(MailboxType.Employee, [Guid.Empty]);
            var json = task.ToJson();
            var deserialized = (MicrosoftOnlineWithMailboxTask)TaskFactory.DeserializeTasks(json)[0];
            Assert.NotNull(deserialized.Tasks);
            Assert.NotEmpty(deserialized.Tasks);
        }

        // --- TaskFactory edge cases ---

        [Fact]
        public void DeserializeTasks_SingleObject_NotArray()
        {
            var json = "{\"TaskName\":\"SamlId\",\"Id\":7}";
            var tasks = TaskFactory.DeserializeTasks(json);
            Assert.Single(tasks);
            Assert.Equal(7, tasks[0].Id);
        }

        // --- ToJson formatting ---

        [Fact]
        public void ToJson_Indented_ContainsNewlines()
        {
            var task = new MsolEnableSyncTask();
            Assert.Contains("\n", task.ToJson(indented: true));
        }

        [Fact]
        public void ToJson_NotIndented_NoNewlines()
        {
            var task = new MsolEnableSyncTask();
            Assert.DoesNotContain("\n", task.ToJson(indented: false));
        }
    }
}
