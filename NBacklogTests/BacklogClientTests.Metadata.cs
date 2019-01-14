using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBacklog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBacklog.Tests
{
    [TestClass()]
    public class BacklogClientTests_Metadata
    {
        [TestMethod()]
        public void GetStatusTypesAsyncTest()
        {
            var types = _client.GetStatusTypesAsync().Result.Content.OrderBy(x => x.Id).ToArray();
            Assert.AreEqual(types.Length, 4);
            Assert.AreEqual(types[0].Id, 1);
            Assert.AreEqual(types[0].Name, "未対応");
            Assert.AreEqual(types[1].Id, 2);
            Assert.AreEqual(types[1].Name, "処理中");
            Assert.AreEqual(types[2].Id, 3);
            Assert.AreEqual(types[2].Name, "処理済み");
            Assert.AreEqual(types[3].Id, 4);
            Assert.AreEqual(types[3].Name, "完了");
        }

        [TestMethod()]
        public void GetResolutionTypesAsyncTest()
        {
            var types = _client.GetResolutionTypesAsync().Result.Content.OrderBy(x => x.Id).ToArray();
            Assert.AreEqual(types.Length, 5);
            Assert.AreEqual(types[0].Id, 0);
            Assert.AreEqual(types[0].Name, "対応済み");
            Assert.AreEqual(types[1].Id, 1);
            Assert.AreEqual(types[1].Name, "対応しない");
            Assert.AreEqual(types[2].Id, 2);
            Assert.AreEqual(types[2].Name, "無効");
            Assert.AreEqual(types[3].Id, 3);
            Assert.AreEqual(types[3].Name, "重複");
            Assert.AreEqual(types[4].Id, 4);
            Assert.AreEqual(types[4].Name, "再現しない");
        }

        [TestMethod()]
        public void GetPriorityTypeAsyncTest()
        {
            var types = _client.GetPriorityTypesAsync().Result.Content.OrderBy(x => x.Id).ToArray();
            Assert.AreEqual(types.Length, 3);
            Assert.AreEqual(types[0].Id, 2);
            Assert.AreEqual(types[0].Name, "高");
            Assert.AreEqual(types[1].Id, 3);
            Assert.AreEqual(types[1].Name, "中");
            Assert.AreEqual(types[2].Id, 4);
            Assert.AreEqual(types[2].Name, "低");
        }

        private BacklogClient _client = Utils.CreateTestClient();
    }
}