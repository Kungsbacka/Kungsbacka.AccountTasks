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
        public void Deserialize1()
        {
            string json = "{\"TaskName\":\"OnpremMailbox\",\"Type\":\"Shared\",\"Id\":123,\"Tasks\":[{\"TaskName\":\"EnableMailbox\"},{\"TaskName\":\"Wait\",\"Minutes\":3},{\"TaskName\":\"ConfigureMailbox\"},{\"TaskName\":\"ConfigureOwa\"},{\"TaskName\":\"ConfigureCalendar\"}]}";
            List<AccountTask> taskList = TaskFactory.DeserializeTasks(json);
            var onpremMailboxTask = taskList[0] as OnpremMailboxTask;
            Assert.AreEqual(onpremMailboxTask.Id, 123);
            var waitTask = onpremMailboxTask.Tasks[1] as WaitTask;
            Assert.AreEqual(waitTask.Minutes, 3);
        }

        [TestMethod]
        public void DeserializeSerializeWithTime()
        {
            string json = "[{\"TaskName\":\"SamlId\",\"RetryCount\":42,\"WaitUntil\":\"2015-04-20T12:00:44\",\"Id\":4293}]";
            List<AccountTask> taskList = TaskFactory.DeserializeTasks(json);
            var task = (SamlIdTask)taskList[0];
            Assert.IsNotNull(task.WaitUntil);
            Assert.AreEqual(((DateTime)task.WaitUntil).ToString("s"), "2015-04-20T12:00:44");
        }

        [TestMethod]
        public void SerializeDeserialize()
        {
            var msolLicenseTask = new MsolLicenseTask(MsolPredefinedLicensePackage.E3);
            msolLicenseTask.Id = 5;
            DateTime date = DateTime.Now.AddDays(1);
            msolLicenseTask.WaitUntil = date;
            msolLicenseTask.RetryCount = 3;
            string json = JsonConvert.SerializeObject(msolLicenseTask);
            List<AccountTask> list = TaskFactory.DeserializeTasks(json);
            var deserializedHomeFolderTask = list[0] as MsolLicenseTask;
            Assert.AreEqual(deserializedHomeFolderTask.WaitUntil, date);
            Assert.AreEqual(deserializedHomeFolderTask.License[0].SkuId, MsolPredefinedLicensePackage.E3[0].SkuId);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SerializeDeserializeInvalidTask()
        {
            var task = new BasicTask("TestTask");
            string json = JsonConvert.SerializeObject(task);
            List<AccountTask> list = TaskFactory.DeserializeTasks(json);
        }

        [TestMethod]
        public void SerializeDeserializeMsolLicense()
        {
            var a = new MsolLicenseTask(MsolPredefinedLicensePackage.Faculty);
            var json = JsonConvert.SerializeObject(a, Formatting.None);
            var l = TaskFactory.DeserializeTasks(json);
            Assert.AreEqual(((MsolLicenseTask)l[0]).License.Length, 2);
        }

        [TestMethod]
        public void SerializeDeserializeMsolLicenseGroup()
        {
            var a = new MsolLicenseGroupTask(new Guid[] { Guid.Empty });
            var json = JsonConvert.SerializeObject(a, Formatting.None);
            var l = TaskFactory.DeserializeTasks(json);
            Assert.AreEqual(((MsolLicenseGroupTask)l[0]).LicenseGroups.Length, 1);
        }
    }
}
