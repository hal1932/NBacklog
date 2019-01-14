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
    public class BacklogClientTests_Team
    {
        [TestMethod()]
        public void GetTeamsAsyncTest()
        {
            var teams = _client.GetTeamsAsync().Result.Content.OrderBy(x => x.Name).ToArray();
            Assert.AreEqual(teams.Length, 2);

            var team = teams[0];
            Assert.AreNotEqual(team, null);
            Assert.AreEqual(team.Id, 5390);
            Assert.AreEqual(team.Name, "test_team_1");
            Assert.AreEqual(team.Creator, null);    // Backlogの仕様(?)でユーザー情報が返ってこない
            Assert.AreEqual(team.LastUpdater, null);// Backlogの仕様(?)でユーザー情報が返ってこない
            var members = team.Members.OrderBy(x => x.Name).ToArray();
            Assert.AreEqual(members.Length, 2);
            Assert.AreEqual(members[0].Name, "test_account");
            Assert.AreEqual(members[1].Name, "test_user_12345");

            team = teams[1];
            Assert.AreNotEqual(team, null);
            Assert.AreEqual(team.Id, 5392);
            Assert.AreEqual(team.Name, "test_team_2");
            Assert.AreEqual(team.Creator, null);
            Assert.AreEqual(team.LastUpdater, null);
            Assert.AreEqual(team.Members.Length, 1);
            Assert.AreEqual(team.Members[0].Name, "test_user_12345");
        }

        [TestMethod()]
        public void AddDeleteTeamAsyncTest()
        {
            if (_client.Domain == "backlog.com")
            {
                return;
            }

            var team = new Team("_test_team_3");
            team.Members = new[] 
            {
                new User(3943),
            };
            var newTeam = _client.AddTeamAsync(team).Result.Content;

            Assert.AreEqual(newTeam.Name, team.Name);
            Assert.AreEqual(newTeam.Members.Length, team.Members.Length);
            for (var i = 0; i < team.Members.Length; ++i)
            {
                Assert.AreEqual(newTeam.Members[i].Id, team.Members[i].Id);
            }

            var deletedTeam = _client.DeleteTeamAsync(newTeam).Result.Content;
            Assert.AreEqual(deletedTeam.Name, newTeam.Name);
            Assert.AreEqual(deletedTeam.Members.Length, newTeam.Members.Length);
            for (var i = 0; i < team.Members.Length; ++i)
            {
                Assert.AreEqual(deletedTeam.Members[i].Id, newTeam.Members[i].Id);
            }
        }

        [TestMethod()]
        public void GetTeamAsyncTest()
        {
            var team = _client.GetTeamAsync(5390).Result.Content;
            Assert.AreNotEqual(team, null);
            Assert.AreEqual(team.Id, 5390);
            Assert.AreEqual(team.Name, "test_team_1");
            Assert.AreEqual(team.Creator, null);
            Assert.AreEqual(team.LastUpdater, null);
            var members = team.Members.OrderBy(x => x.Name).ToArray();
            Assert.AreEqual(members.Length, 2);
            Assert.AreEqual(members[0].Name, "test_account");
            Assert.AreEqual(members[1].Name, "test_user_12345");
        }

        [TestMethod()]
        public void UpdateTeamAsyncTest()
        {
            if (_client.Domain == "backlog.com")
            {
                return;
            }

            var team = new Team("test_team_1");
            team.Members = new[]
            {
                new User(3943),
                new User(16877),
            };

            var updatedTeam = _client.UpdateTeamAsync(team).Result.Content;
            Assert.AreNotEqual(updatedTeam, team);
            Assert.AreEqual(updatedTeam.Id, team.Id);
            Assert.AreEqual(updatedTeam.Name, team.Name);
            Assert.AreEqual(updatedTeam.Creator, team.Creator);
            Assert.AreEqual(updatedTeam.LastUpdater, team.LastUpdater);

            var members = updatedTeam.Members.OrderBy(x => x.Name).ToArray();
            Assert.AreEqual(updatedTeam.Members.Length, 2);
            Assert.AreEqual(updatedTeam.Members[0].Name, "test_user_12345");
            Assert.AreEqual(updatedTeam.Members[1].Name, "test_account");

            team.Members = new[] { team.Members[0] };
            updatedTeam = _client.UpdateTeamAsync(team).Result.Content;

            members = updatedTeam.Members.OrderBy(x => x.Name).ToArray();
            Assert.AreEqual(updatedTeam.Members.Length, 1);
            Assert.AreEqual(updatedTeam.Members[0].Name, "test_user_12345");
        }

        private BacklogClient _client = Utils.CreateTestClient();
    }
}