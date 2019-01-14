using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBacklog.DataTypes;
using NBacklog.Tests;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBacklog.DataTypes.Tests.DataTypes
{
    [TestClass()]
    public class ProjectTests
    {
        [ClassInitialize]
        public static void SetupClass(TestContext _)
        {
            _project = _client.GetProjectAsync("TEST").Result.Content;
        }

        #region user
        [TestMethod()]
        public void GetUsersAsyncTest()
        {
            var users = _project.GetUsersAsync(true).Result.Content.OrderBy(x => x.Name).ToArray();
            Assert.AreEqual(users.Length, 1);
            var user = users[0];
            Assert.AreEqual(user.Id, 3943);
            Assert.AreEqual(user.Name, "test_user_12345");
            Assert.AreEqual(user.MailAddress, "yu.arai.19@gmail.com");
            Assert.AreEqual(user.UserId, "6hMjddYcib");
            Assert.AreEqual(user.Language, "ja");
            Assert.AreEqual(user.Role, UserRole.Admin);

            users = _project.GetUsersAsync(false).Result.Content.OrderBy(x => x.Name).ToArray();
            Assert.AreEqual(users.Length, 2);
            user = users[0];
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
        public void AddDeleteUserAsyncTest()
        {
            var user = new User(16877);

            var newUser = _project.AddUserAsync(user).Result.Content;
            Assert.AreNotEqual(newUser, null);
            Assert.AreEqual(newUser.Id, user.Id);

            var deletedUser = _project.DeleteUserAsync(user).Result.Content;
            Assert.AreNotEqual(deletedUser, null);
            Assert.AreEqual(deletedUser.Id, user.Id);
        }
        #endregion

        [TestMethod()]
        public void TicketTest()
        {
            return;
            var rand = new Random();

            var types = _project.GetTicketTypesAsync().Result.Content;
            var priorities = _client.GetPriorityTypesAsync().Result.Content;
            var users = _project.GetUsersAsync().Result.Content;
            var categories = _project.GetCategoriesAsync().Result.Content;
            var milestones = _project.GetMilestonesAsync().Result.Content;
            var sharedFiles = _project.GetSharedFilesAsync().Result.Content;

            foreach (var ticket in _project.GetTicketsAsync().Result.Content)
            {
                _project.DeleteTicketAsync(ticket).Wait();
            }

            var tickets = Enumerable.Range(0, 50).Select(i =>
                new Ticket(_project, $"summary_{i}", types[rand.Next(types.Length - 1)], priorities[rand.Next(priorities.Length - 1)])
                {
                    Assignee = users[rand.Next(0, users.Length - 1)],
                    Categories = Enumerable.Range(0, rand.Next(3)).Select(_ => categories[rand.Next(categories.Length - 1)]).Distinct().ToArray(),
                    Description = $"desc_{i}",
                    DueDate = (DateTime.Now + TimeSpan.FromDays(rand.Next(10))).Date,
                    EstimatedHours = rand.Next(10),
                    Milestones = Enumerable.Range(0, rand.Next(3)).Select(_ => milestones[rand.Next(milestones.Length - 1)]).Distinct().ToArray(),
                    StartDate = (DateTime.Now - TimeSpan.FromDays(rand.Next(10))).Date,
                    Versions = Enumerable.Range(0, rand.Next(3)).Select(_ => milestones[rand.Next(milestones.Length - 1)]).Distinct().ToArray(),
                })
                .ToList();

            var ticketCount = _project.GetTicketCountAsync().Result.Content;
            Assert.AreEqual(ticketCount, 0);

            var newTickets = new List<Ticket>();
            foreach (var ticket in tickets)
            {
                var newTicket = _project.AddTicketAsync(ticket).Result.Content;
                Assert.AreNotEqual(newTicket, null);
                Assert.AreSame(newTicket.Project, ticket.Project);
                Assert.AreEqual(newTicket.Summary, ticket.Summary);
                Assert.AreEqual(newTicket.Type, ticket.Type);
                Assert.AreEqual(newTicket.Priority, ticket.Priority);
                Assert.AreEqual(newTicket.Assignee, ticket.Assignee);
                CollectionAssert.AreEquivalent(newTicket.Categories, ticket.Categories);
                Assert.AreEqual(newTicket.DueDate, ticket.DueDate);
                CollectionAssert.AreEquivalent(newTicket.Milestones, ticket.Milestones);
                CollectionAssert.AreEquivalent(newTicket.Versions, ticket.Versions);

                if (_client.Domain != "backlog.com")
                {
                    Assert.AreEqual(newTicket.StartDate, ticket.StartDate);
                    Assert.AreEqual(newTicket.EstimatedHours, ticket.EstimatedHours);
                }

                newTickets.Add(newTicket);
            }

            Assert.AreEqual(newTickets.Count, tickets.Count);

            foreach (var ticket in newTickets)
            {
                var deletedTicket = _project.DeleteTicketAsync(ticket).Result.Content;
                Assert.AreNotEqual(deletedTicket, null);
                Assert.AreEqual(deletedTicket.Id, ticket.Id);
                Assert.AreEqual(deletedTicket.Key, ticket.Key);
                Assert.AreEqual(deletedTicket.KeyId, ticket.KeyId);
                Assert.AreSame(deletedTicket.Project, ticket.Project);
                Assert.AreEqual(deletedTicket.Summary, ticket.Summary);
                Assert.AreSame(deletedTicket.Type, ticket.Type);
                Assert.AreSame(deletedTicket.Priority, ticket.Priority);
                Assert.AreSame(deletedTicket.Assignee, ticket.Assignee);
                CollectionAssert.AreEquivalent(deletedTicket.Categories, ticket.Categories);
                Assert.AreEqual(deletedTicket.DueDate, ticket.DueDate);
                Assert.AreEqual(deletedTicket.EstimatedHours, ticket.EstimatedHours);
                CollectionAssert.AreEquivalent(deletedTicket.Milestones, ticket.Milestones);
                Assert.AreEqual(deletedTicket.StartDate, ticket.StartDate);
                CollectionAssert.AreEquivalent(deletedTicket.Versions, ticket.Versions);
            }

            ticketCount = _project.GetTicketCountAsync().Result.Content;
            Assert.AreEqual(ticketCount, 0);
        }

        #region ticket type
        [TestMethod()]
        public void GetTicketTypesAsyncTest()
        {
            var types = _project.GetTicketTypesAsync().Result.Content.OrderBy(x => x.Id).ToArray();
            Assert.AreEqual(types.Length, 4);

            var type = types[0];
            Assert.AreEqual(type.Id, 7407);
            Assert.AreEqual(type.Name, "バグ");
            Assert.AreEqual(type.Color.ToArgb(), (int)TicketColor.Orange);
            Assert.AreSame(type.Project, _project);

            type = types[1];
            Assert.AreEqual(type.Id, 7408);
            Assert.AreEqual(type.Name, "タスク");
            Assert.AreEqual(type.Color.ToArgb(), (int)TicketColor.YellowGreen);
            Assert.AreSame(type.Project, _project);

            type = types[2];
            Assert.AreEqual(type.Id, 7409);
            Assert.AreEqual(type.Name, "要望");
            Assert.AreEqual(type.Color.ToArgb(), (int)TicketColor.Yellow);
            Assert.AreSame(type.Project, _project);

            type = types[3];
            Assert.AreEqual(type.Id, 7410);
            Assert.AreEqual(type.Name, "その他");
            Assert.AreEqual(type.Color.ToArgb(), (int)TicketColor.Blue);
            Assert.AreSame(type.Project, _project);
        }

        [TestMethod()]
        public void AddDeleteTicketTypeAsyncTest()
        {
            var type = new TicketType(_project, "テスト", TicketColor.Black);

            var newType = _project.AddTicketTypeAsync(type).Result.Content;
            Assert.AreNotEqual(newType, null);
            Assert.AreEqual(newType.Name, type.Name);
            Assert.AreEqual(newType.Color, type.Color);

            var subType = new TicketType(7410);
            var deletedType = _project.DeleteTicketTypeAsync(newType, subType).Result.Content;
            Assert.AreEqual(deletedType, newType);
        }

        [TestMethod()]
        public void UpdateTicketTypeAsyncTest()
        {
            var type = new TicketType(7407)
            {
                Name = "バグ1",
                Color = Color.FromArgb((int)TicketColor.Blue),
            };

            var updatedType = _project.UpdateTicketTypeAsync(type).Result.Content;
            Assert.AreNotEqual(updatedType, null);
            Assert.AreEqual(updatedType.Id, type.Id);
            Assert.AreEqual(updatedType.Name, type.Name);
            Assert.AreEqual(updatedType.Color, type.Color);

            type.Name = "バグ";
            type.Color = Color.FromArgb((int)TicketColor.Orange);
            updatedType = _project.UpdateTicketTypeAsync(type).Result.Content;
            Assert.AreNotEqual(updatedType, null);
            Assert.AreEqual(updatedType.Id, type.Id);
            Assert.AreEqual(updatedType.Name, type.Name);
            Assert.AreEqual(updatedType.Color, type.Color);
        }
        #endregion

        #region category
        [TestMethod()]
        public void GetCategoriesAsyncTest()
        {
            var categories = _project.GetCategoriesAsync().Result.Content.OrderBy(x => x.Id).ToArray();
            Assert.AreEqual(categories.Length, 3);

            var category = categories[0];
            Assert.AreEqual(category.Id, 3070);
            Assert.AreEqual(category.Name, "サブシステムA");

            category = categories[1];
            Assert.AreEqual(category.Id, 3071);
            Assert.AreEqual(category.Name, "リサーチ");

            category = categories[2];
            Assert.AreEqual(category.Id, 3072);
            Assert.AreEqual(category.Name, "デザイン");
        }

        [TestMethod()]
        public void AddDeleteCategoryAsyncTest()
        {
            var category = new Category("テスト");

            var newCategory = _project.AddCategoryAsync(category).Result.Content;
            Assert.AreNotEqual(newCategory, null);
            Assert.AreEqual(newCategory.Name, category.Name);

            var deletedCategory = _project.DeleteCategoryAsync(newCategory).Result.Content;
            Assert.AreEqual(deletedCategory, newCategory);
        }

        [TestMethod()]
        public void UpdateCategoryAsyncTest()
        {
            var category = new Category(3070)
            {
                Name = "サブシステムB",
            };

            var updatedCategory = _project.UpdateCategoryAsync(category).Result.Content;
            Assert.AreNotEqual(updatedCategory, null);
            Assert.AreEqual(updatedCategory.Name, category.Name);

            category.Name = "サブシステムA";
            updatedCategory = _project.UpdateCategoryAsync(category).Result.Content;
            Assert.AreNotEqual(updatedCategory, null);
            Assert.AreEqual(updatedCategory.Name, category.Name);
        }
        #endregion

        #region milestone
        [TestMethod()]
        public void GetMilestonesAsyncTest()
        {
            var milestones = _project.GetMilestonesAsync().Result.Content.OrderBy(x => x.Id).ToArray();
            Assert.AreEqual(milestones.Length, 3);

            var milestone = milestones[0];
            Assert.AreEqual(milestone.Id, 1591);
            Assert.AreEqual(milestone.Name, "1.0-M1");
            Assert.AreEqual(milestone.Description, "バージョン1.0のマイルストーンリリース第１弾");
            Assert.AreEqual(milestone.StartDate, new DateTime(2018, 11, 1));
            Assert.AreEqual(milestone.DueDate, new DateTime(2018, 12, 31));
            Assert.AreEqual(milestone.IsArchived, false);

            milestone = milestones[1];
            Assert.AreEqual(milestone.Id, 1952);
            Assert.AreEqual(milestone.Name, "test1");
            Assert.AreEqual(milestone.Description, null);
            Assert.AreEqual(milestone.StartDate, new DateTime(2018, 11, 1));
            Assert.AreEqual(milestone.DueDate, new DateTime(2018, 11, 22));
            Assert.AreEqual(milestone.IsArchived, false);

            milestone = milestones[2];
            Assert.AreEqual(milestone.Id, 3324);
            Assert.AreEqual(milestone.Name, "test2");
            Assert.AreEqual(milestone.Description, "test2");
            Assert.AreEqual(milestone.StartDate, new DateTime(2018, 12, 11));
            Assert.AreEqual(milestone.DueDate, new DateTime(2018, 12, 21));
            Assert.AreEqual(milestone.IsArchived, false);
        }

        [TestMethod()]
        public void AddDeleteMilestoneAsyncTest()
        {
            var milestone = new Milestone("テスト")
            {
                Description = "あああ",
                StartDate = new DateTime(2019, 1, 1),
                DueDate = new DateTime(2019, 1, 2),
            };

            var newMilestone = _project.AddMilestoneAsync(milestone).Result.Content;
            Assert.AreNotEqual(newMilestone, null);
            Assert.AreEqual(newMilestone.Name, milestone.Name);
            Assert.AreEqual(newMilestone.Description, milestone.Description);
            Assert.AreEqual(newMilestone.StartDate, milestone.StartDate);
            Assert.AreEqual(newMilestone.DueDate, milestone.DueDate);

            var deletedMilestone = _project.DeleteMilestoneAsync(newMilestone).Result.Content;
            Assert.AreNotEqual(deletedMilestone, null);
            Assert.AreEqual(deletedMilestone.Id, newMilestone.Id);
            Assert.AreEqual(deletedMilestone.Name, newMilestone.Name);
            Assert.AreEqual(deletedMilestone.Description, newMilestone.Description);
            Assert.AreEqual(deletedMilestone.StartDate, newMilestone.StartDate);
            Assert.AreEqual(deletedMilestone.DueDate, newMilestone.DueDate);
        }

        [TestMethod()]
        public void UpdateMilestoneAsyncTest()
        {
            var milestone = new Milestone(1952)
            {
                Name = "test1",
                Description = null,
                StartDate = new DateTime(2018, 11, 1),
                DueDate = new DateTime(2018, 11, 22),
            };

            milestone.Name = "test11";
            milestone.Description = "test111";
            milestone.StartDate = new DateTime(2018, 11, 2);
            milestone.DueDate = default;

            var updatedMilestone = _project.UpdateMilestoneAsync(milestone).Result.Content;
            Assert.AreNotEqual(updatedMilestone, null);
            Assert.AreEqual(updatedMilestone.Id, milestone.Id);
            Assert.AreEqual(updatedMilestone.Name, milestone.Name);
            Assert.AreEqual(updatedMilestone.Description, milestone.Description);
            Assert.AreEqual(updatedMilestone.StartDate, milestone.StartDate);
            Assert.AreEqual(updatedMilestone.DueDate, milestone.DueDate);
            Assert.AreEqual(updatedMilestone.IsArchived, milestone.IsArchived);

            milestone.Name = "test1";
            milestone.Description = null;
            milestone.StartDate = new DateTime(2019, 11, 1);
            milestone.DueDate = new DateTime(2019, 11, 22);

            updatedMilestone = _project.UpdateMilestoneAsync(milestone).Result.Content;
            Assert.AreNotEqual(updatedMilestone, null);
            Assert.AreEqual(updatedMilestone.Id, milestone.Id);
            Assert.AreEqual(updatedMilestone.Name, milestone.Name);
            Assert.AreEqual(updatedMilestone.Description, milestone.Description);
            Assert.AreEqual(updatedMilestone.StartDate, milestone.StartDate);
            Assert.AreEqual(updatedMilestone.DueDate, milestone.DueDate);
            Assert.AreEqual(updatedMilestone.IsArchived, milestone.IsArchived);
        }
        #endregion

        #region webhook
        [TestMethod()]
        public void GetWebhooksAsyncTest()
        {
            var user = new User(3943);

            var hooks = _project.GetWebhooksAsync().Result.Content.OrderBy(x => x.Id).ToArray();
            Assert.AreEqual(hooks.Length, 2);

            var hook = hooks[0];
            Assert.AreEqual(hook.Id, 74);
            Assert.AreEqual(hook.Name, "test1");
            Assert.AreEqual(hook.Description, "test1_desc");
            Assert.AreEqual(hook.HookUrl, "http://localhost");
            Assert.AreEqual(hook.Creator.Id, user.Id);
            Assert.AreEqual(hook.Created, new DateTime(2018, 12, 2, 0, 29, 56));
            Assert.AreEqual(hook.LastUpdater.Id, user.Id);
            Assert.AreEqual(hook.LastUpdated, new DateTime(2018, 12, 2, 5, 18, 11));
            Assert.AreEqual(hook.HookedActivities.Length, Enum.GetValues(typeof(ActivityEvent)).Length - 1);
            Assert.AreEqual(hook.IsAllActivitiesHooked, true);

            hook = hooks[1];
            Assert.AreEqual(hook.Id, 76);
            Assert.AreEqual(hook.Name, "test3");
            Assert.AreEqual(hook.Description, "test3_desc");
            Assert.AreEqual(hook.HookUrl, "http://localhost");
            Assert.AreEqual(hook.Creator.Id, user.Id);
            Assert.AreEqual(hook.Created, new DateTime(2018, 12, 2, 0, 30, 30));
            Assert.AreEqual(hook.LastUpdater.Id, user.Id);
            Assert.AreEqual(hook.LastUpdated, hook.Created);
            var activities = hook.HookedActivities.OrderBy(x => (int)x).ToArray();
            Assert.AreEqual(activities.Length, 3);
            Assert.AreEqual(activities[0], ActivityEvent.WikiDeleted);
            Assert.AreEqual(activities[1], ActivityEvent.FileUpdated);
            Assert.AreEqual(activities[2], ActivityEvent.GitPullRequestUpdated);
            Assert.AreEqual(hook.IsAllActivitiesHooked, false);
        }

        [TestMethod()]
        public void AddDeleteWebhookAsyncTest()
        {
            var hook = new Webhook("test", "http://localhost")
            {
                Description = "test_desc",
            };

            var newHook = _project.AddWebhookAsync(hook).Result.Content;
            Assert.AreNotEqual(newHook, null);
            Assert.AreEqual(newHook.Name, hook.Name);
            Assert.AreEqual(newHook.Description, hook.Description);
            Assert.AreEqual(newHook.HookUrl, hook.HookUrl);
            CollectionAssert.AreEquivalent(newHook.HookedActivities, hook.HookedActivities);

            var deletedHook = _project.DeleteWebhookAsync(newHook).Result.Content;
            Assert.AreEqual(deletedHook, newHook);
        }

        [TestMethod()]
        public void UpdateWebhookAsyncTest()
        {
            var hook = new Webhook(74);

            hook.Name = "test";
            hook.Description = "test_desc";
            hook.HookUrl = "http://test.com";
            hook.HookedActivities = new[] { ActivityEvent.FileAdded, ActivityEvent.GitPushed };

            var updatedHook = _project.UpdateWebhookAsync(hook).Result.Content;
            Assert.AreNotEqual(updatedHook, null);
            Assert.AreEqual(updatedHook.Name, hook.Name);
            Assert.AreEqual(updatedHook.Description, hook.Description);
            Assert.AreEqual(updatedHook.HookUrl, hook.HookUrl);
            Assert.AreEqual(updatedHook.IsAllActivitiesHooked, false);
            CollectionAssert.AreEquivalent(updatedHook.HookedActivities, hook.HookedActivities);

            hook.Name = "test1";
            hook.Description = "test1_desc";
            hook.HookUrl = "http://localhost";
            hook.HookedActivities = null;

            updatedHook = _project.UpdateWebhookAsync(hook).Result.Content;
            Assert.AreNotEqual(updatedHook, null);
            Assert.AreEqual(updatedHook.Name, hook.Name);
            Assert.AreEqual(updatedHook.Description, hook.Description);
            Assert.AreEqual(updatedHook.HookUrl, hook.HookUrl);
            Assert.AreEqual(updatedHook.IsAllActivitiesHooked, true);
            CollectionAssert.AreEquivalent(updatedHook.HookedActivities, Activity.AllEvents);
        }
        #endregion

        #region wiki
        [TestMethod()]
        public void GetWikipageCountAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetWikipagesAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetWikipageTagsAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddWikipageAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UpdateWikipageAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteWikipageAsyncTest()
        {
            Assert.Fail();
        }
        #endregion

        #region git
        [TestMethod()]
        public void GetGitRepositoriesAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetGitRepositoryAsyncTest()
        {
            Assert.Fail();
        }
        #endregion

        #region team
        [TestMethod()]
        public void GetTeamsAsyncTest()
        {
            var teams = _project.GetTeamsAsync().Result.Content;
            Assert.AreEqual(teams.Length, 1);

            var team = teams[0];
            Assert.AreEqual(team.Id, 5390);
            Assert.AreEqual(team.Name, "test_team_1");
            Assert.AreEqual(team.Creator, null);
            Assert.AreEqual(team.Created, new DateTime(2019, 1, 12, 16, 14, 1));
            Assert.AreEqual(team.LastUpdater, null);
            Assert.AreEqual(team.LastUpdated, new DateTime(2019, 1, 12, 18, 0, 2));
            var members = team.Members.OrderBy(x => x.Id).ToArray();
            Assert.AreEqual(members.Length, 2);
            Assert.AreEqual(members[0].Id, 3943);
            Assert.AreEqual(members[1].Id, 16877);
        }

        [TestMethod()]
        public void AddDeleteTeamAsyncTest()
        {
            if (_client.Domain == "backlog.com")
            {
                return;
            }

            var team = new Team("test");
            team.Members = new[] { new User(5390) };

            var newTeam = _project.AddTeamAsync(team).Result.Content;
        }

        [TestMethod()]
        public void UpdateTeamAsyncTest()
        {
            if (_client.Domain == "backlog.com")
            {
                return;
            }
        }
        #endregion

        #region misc
        [TestMethod()]
        public void GetCustomFieldsAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetSharedFilesAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetIconAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetActivitiesAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetDiskUsageAsyncTest()
        {
            Assert.Fail();
        }
        #endregion

        private static BacklogClient _client = Utils.CreateTestClient();
        private static Project _project;
    }
}