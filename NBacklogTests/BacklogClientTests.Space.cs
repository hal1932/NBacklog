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
    public class BacklogClientTests_Space
    {
        [TestMethod()]
        public void GetSpaceAsyncTest()
        {
            var space = _client.GetSpaceAsync().Result.Content;
            Assert.AreNotEqual(space, null);
            Assert.AreEqual(space.Key, "hal1932");
            Assert.AreEqual(space.Language, "ja");
            Assert.AreEqual(space.Name, "テスト組織");
            Assert.AreEqual(space.OwnerId, 3943);
            Assert.AreEqual(space.TextFormattingRule, "markdown");
            Assert.AreEqual(space.TimeZone, "Asia/Tokyo");
        }

        private BacklogClient _client = Utils.CreateTestClient();
    }
}