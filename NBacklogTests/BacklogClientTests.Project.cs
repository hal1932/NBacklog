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
    public class BacklogClientTests_Project
    {
        [TestMethod()]
        public void GetProjectsAsyncTest()
        {
            var projects = _client.GetProjectsAsync().Result.Content;
            Assert.AreEqual(projects.Length, 1);

            var project = projects[0];
            Assert.AreEqual(project.Id, 7352);
            Assert.AreEqual(project.Name, "test");
            Assert.AreEqual(project.Key, "TEST");
            Assert.AreEqual(project.IsArchived, false);
            Assert.AreEqual(project.CanProjectLeaderEditProjectLeader, false);
            Assert.AreEqual(project.IsChartEnabled, true);
            Assert.AreEqual(project.IsSubtaskingEnabled, true);
            Assert.AreEqual(project.TextFormattingRule, "markdown");
        }

        [TestMethod()]
        public void GetProjectAsyncTest()
        {
            var project = _client.GetProjectAsync(7352).Result.Content;
            Assert.AreEqual(project.Id, 7352);
            Assert.AreEqual(project.Name, "test");
            Assert.AreEqual(project.Key, "TEST");
            Assert.AreEqual(project.IsArchived, false);
            Assert.AreEqual(project.CanProjectLeaderEditProjectLeader, false);
            Assert.AreEqual(project.IsChartEnabled, true);
            Assert.AreEqual(project.IsSubtaskingEnabled, true);
            Assert.AreEqual(project.TextFormattingRule, "markdown");

            project = _client.GetProjectAsync("TEST").Result.Content;
            Assert.AreEqual(project.Id, 7352);
            Assert.AreEqual(project.Name, "test");
            Assert.AreEqual(project.Key, "TEST");
            Assert.AreEqual(project.IsArchived, false);
            Assert.AreEqual(project.CanProjectLeaderEditProjectLeader, false);
            Assert.AreEqual(project.IsChartEnabled, true);
            Assert.AreEqual(project.IsSubtaskingEnabled, true);
            Assert.AreEqual(project.TextFormattingRule, "markdown");
        }

        [TestMethod()]
        public void UpdateProjectAsyncTest()
        {
            if (_client.Domain == "backlog.com")
            {
                return;
            }

            var project = new Project(1775)
            {
                Name = "test1",
                Key = "TEST1",
                IsArchived = true,
                CanProjectLeaderEditProjectLeader = true,
                TextFormattingRule = "backlog",
            };
            var updatedProject = _client.UpdateProjectAsync(project).Result.Content;
            Assert.AreNotEqual(updatedProject, null);
            Assert.AreEqual(updatedProject.Name, project.Name);
            Assert.AreEqual(updatedProject.Key, project.Key);
            Assert.AreEqual(updatedProject.IsArchived, project.IsArchived);
            Assert.AreEqual(updatedProject.CanProjectLeaderEditProjectLeader, project.CanProjectLeaderEditProjectLeader);
            Assert.AreEqual(updatedProject.TextFormattingRule, project.TextFormattingRule);

            project.Name = "test";
            project.Key = "TEST";
            project.IsArchived = false;
            project.CanProjectLeaderEditProjectLeader = false;
            project.TextFormattingRule = "markdown";

            updatedProject = _client.UpdateProjectAsync(project).Result.Content;
            Assert.AreNotEqual(updatedProject, null);
            Assert.AreEqual(updatedProject.Name, project.Name);
            Assert.AreEqual(updatedProject.Key, project.Key);
            Assert.AreEqual(updatedProject.IsArchived, project.IsArchived);
            Assert.AreEqual(updatedProject.CanProjectLeaderEditProjectLeader, project.CanProjectLeaderEditProjectLeader);
            Assert.AreEqual(updatedProject.TextFormattingRule, project.TextFormattingRule);
        }

        //[TestMethod()]
        //public void DeleteProjectAsyncTest()
        //{
        //    if (_client.Domain == "backlog.com")
        //    {
        //        return;
        //    }
        //}

        private BacklogClient _client = Utils.CreateTestClient();
    }
}