using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBacklog.DataTypes;
using NBacklog.Query;
using NBacklog.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBacklog.DataTypes.Tests.DataTypes
{
    [TestClass()]
    public class TicketTests
    {
        [ClassInitialize]
        public static void SetupClass(TestContext _)
        {
            var project = _client.GetProjectAsync("TEST").Result.Content;
            _ticket = project.GetTicketAsync("TEST-83").Result.Content;
        }

        [TestMethod()]
        public void GetCommentCountTest()
        {
            var count = _ticket.GetCommentCountAsync().Result.Content;
            Assert.AreEqual(count, 2);
        }

        [TestMethod()]
        public void GetCommentsAsyncTest()
        {
            var comments = _ticket.GetCommentsAsync().Result.Content.OrderBy(x => x.Id).ToArray();
            Assert.AreEqual(comments.Length, 2);

            var comment = comments[0];
            Assert.AreEqual(comment.Id, 658441);
            Assert.AreEqual(comment.Content, "commen1");
            Assert.AreEqual(comment.ChangeLogs.Length, 0);
            Assert.AreEqual(comment.Notifications.Length, 1);
            Assert.AreEqual(comment.Notifications[0].User.Id, 18731);

            comment = comments[1];
            Assert.AreEqual(comment.Id, 658442);
            Assert.AreEqual(comment.Content, "comment2");
            Assert.AreEqual(comment.Notifications.Length, 0);
            Assert.AreEqual(comment.ChangeLogs.Length, 8);
        }

        [TestMethod()]
        public void AddDeleteCommentAsyncTest()
        {
            var comment = new Comment("test1");

            var newComment = _ticket.AddCommentAsync(comment).Result.Content;
            Assert.AreNotEqual(newComment, null);
            Assert.AreEqual(newComment.Content, "test1");

            var deletedComment = _ticket.DeleteCommentAsync(newComment).Result.Content;
            Assert.AreEqual(deletedComment.Id, newComment.Id);
            Assert.AreEqual(deletedComment.Content, newComment.Content);
        }

        [TestMethod()]
        public void UpdateCommentAsyncTest()
        {
            var comment = new Comment(658441);
            comment.Content = "aaaaa";

            var a = _ticket.GetCommentsAsync().Result.Content;

            var updatedComment = _ticket.UpdateCommentAsync(comment).Result.Content;
            Assert.AreEqual(updatedComment.Id, comment.Id);
            Assert.AreEqual(updatedComment.Content, comment.Content);

            comment.Content = "commen1";

            updatedComment = _ticket.UpdateCommentAsync(comment).Result.Content;
            Assert.AreEqual(updatedComment.Id, comment.Id);
            Assert.AreEqual(updatedComment.Content, comment.Content);
        }

        [TestMethod()]
        public void LinkUnlinkSharedFilesAsyncTest()
        {
            var files = _ticket.Project.GetSharedFilesAsync().Result.Content;

            var newFiles = _ticket.LinkSharedFilesAsync(files[0]).Result.Content;
            Assert.AreEqual(newFiles.Length, 1);
            Assert.AreEqual(newFiles[0].Id, files[0].Id);

            var deletedFile = _ticket.UnlinkSharedFilesAsync(files[0]).Result.Content;
            Assert.AreEqual(deletedFile.Id, files[0].Id);
        }

        private static BacklogClient _client = Utils.CreateTestClient();
        private static Ticket _ticket;
    }
}