using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBacklog;
using NBacklog.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBacklog.Tests
{
    [TestClass()]
    public class BacklogClientTests_User
    {
        [TestMethod()]
        public void GetUsersAsyncTest()
        {
            var users = _client.GetUsersAsync().Result.Content.OrderBy(x => x.Name).ToArray();
            Assert.AreEqual(users.Length, 2);

            var user = users[0];
            Assert.AreEqual(user.Id, 16877);
            Assert.AreEqual(user.Name, "test_account");
            Assert.AreEqual(user.MailAddress, "kvhn1h1u6sjk@sute.jp");
            Assert.AreEqual(user.UserId, "*AWMVaKoRkG");
            Assert.AreEqual(user.Language, "ja");
            Assert.AreEqual(user.Role, UserRole.User);

            user = users[1];
            Assert.AreEqual(user.Id, 3943);
            Assert.AreEqual(user.Name, "test_user_12345");
            Assert.AreEqual(user.MailAddress, "yu.arai.19@gmail.com");
            Assert.AreEqual(user.UserId, "6hMjddYcib");
            Assert.AreEqual(user.Language, "ja");
            Assert.AreEqual(user.Role, UserRole.Admin);
        }

        [TestMethod()]
        public void GetUserAsyncTest()
        {
            var user = _client.GetUserAsync(3943).Result.Content;
            Assert.AreNotEqual(user, null);
            Assert.AreEqual(user.Id, 3943);
            Assert.AreEqual(user.Name, "test_user_12345");
            Assert.AreEqual(user.MailAddress, "yu.arai.19@gmail.com");
            Assert.AreEqual(user.UserId, "6hMjddYcib");
            Assert.AreEqual(user.Language, "ja");
            Assert.AreEqual(user.Role, UserRole.Admin);
        }

        [TestMethod()]
        public void GetLoginUserAsyncTest()
        {
            var user = _client.GetLoginUserAsync().Result.Content;
            Assert.AreNotEqual(user, null);
            Assert.AreEqual(user.Id, 3943);
            Assert.AreEqual(user.Name, "test_user_12345");
            Assert.AreEqual(user.MailAddress, "yu.arai.19@gmail.com");
            Assert.AreEqual(user.UserId, "6hMjddYcib");
            Assert.AreEqual(user.Language, "ja");
            Assert.AreEqual(user.Role, UserRole.Admin);
        }

        //[TestMethod()]
        //public void AddUserAsyncTest()
        //{
        //    if (_client.Domain == "backlog.com")
        //    {
        //        return;
        //    }
        //}

        [TestMethod()]
        public void UpdateUserAsyncTest()
        {
            if (_client.Domain == "backlog.com")
            {
                return;
            }

            var user = new User(16877)
            {
                Name = "test_account1",
                MailAddress = "test@example.com",
                Role = UserRole.Admin,
            };

            var updatedUser = _client.UpdateUserAsync(user).Result.Content;
            Assert.AreNotEqual(updatedUser, null);
            Assert.AreEqual(updatedUser.Name, user.Name);
            Assert.AreEqual(updatedUser.MailAddress, user.MailAddress);
            Assert.AreEqual(updatedUser.Role, user.Role);

            user.Name = "test_account";
            user.MailAddress = "kvhn1h1u6sjk@sute.jp";
            user.Role = UserRole.User;

            updatedUser = _client.UpdateUserAsync(user).Result.Content;
            Assert.AreNotEqual(updatedUser, null);
            Assert.AreEqual(updatedUser.Name, user.Name);
            Assert.AreEqual(updatedUser.MailAddress, user.MailAddress);
            Assert.AreEqual(updatedUser.Role, user.Role);
        }

        //[TestMethod()]
        //public void DeleteUserAsyncTest()
        //{
        //    if (_client.Domain == "backlog.com")
        //    {
        //        return;
        //    }
        //}

        private BacklogClient _client = Utils.CreateTestClient();
    }
}