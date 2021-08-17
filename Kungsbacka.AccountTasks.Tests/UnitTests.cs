using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Kungsbacka.AccountTasks.Tests
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void DeserializeWithTime()
        {
            string json = "[{\"TaskName\":\"SamlId\",\"RetryCount\":42,\"WaitUntil\":\"2015-04-20T12:00:44\",\"Id\":4293}]";
            var deserializedTasks = TaskFactory.DeserializeTasks(json);
            var task = (SamlIdTask)deserializedTasks[0];
            Assert.IsNotNull(task.WaitUntil);
            Assert.AreEqual(((DateTime)task.WaitUntil).ToString("s"), "2015-04-20T12:00:44");
        }

        [TestMethod]
        public void SerializeDeserializeWithRetryCount()
        {
            var task = new MsolLicenseTask(new MsolLicense[] { new MsolLicense("SKU-ID") })
            {
                Id = 5
            };
            DateTime date = DateTime.Now.AddDays(1);
            task.WaitUntil = date;
            task.RetryCount = 3;
            string json = JsonConvert.SerializeObject(task);
            var deserializedTasks = TaskFactory.DeserializeTasks(json);
            var msolLicenseTask = (MsolLicenseTask)deserializedTasks[0];
            Assert.AreEqual(msolLicenseTask.WaitUntil, date);
            Assert.AreEqual(msolLicenseTask.License[0].SkuId, "SKU-ID");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SerializeDeserializeInvalidTask()
        {
            var task = new BasicTask("-InvalidTask-");
            string json = JsonConvert.SerializeObject(task);
            TaskFactory.DeserializeTasks(json);
        }

        [TestMethod]
        public void SerializeDeserializeMsolLicenseGroup()
        {
            var task = new MsolLicenseGroupTask(new Guid[] { Guid.Empty });
            var json = JsonConvert.SerializeObject(task);
            var deserializedTasks = TaskFactory.DeserializeTasks(json);
            Assert.AreEqual(((MsolLicenseGroupTask)deserializedTasks[0]).LicenseGroups.Length, 1);
        }

        [TestMethod]
        public void SerializeDeserializeSequenceTask()
        {
            var task = new MicrosoftOnlineWithMailboxTask(
                MailboxType.Employee,
                new Guid[] {Guid.Empty}
            );
            var json = JsonConvert.SerializeObject(task);
            var deserializedTasks = TaskFactory.DeserializeTasks(json);
            Assert.AreEqual(((MicrosoftOnlineWithMailboxTask)deserializedTasks[0]).Type, MailboxType.Employee);
        }
    }
}
