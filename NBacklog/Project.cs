using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBacklog
{
    public struct TicketType
    {
        public int Id;
        public Project Project;
        public string Name;
        public Color Color;
        public int DisplayOrder;
    }

    public struct Category
    {
        public int Id;
        public string Name;
        public int DisplayOrder;
    }

    public struct Milestone
    {
        public int Id;
        public Project Project;
        public string Name;
        public string Description;
        public DateTime StartDate;
        public DateTime ReleaseDueDate;
        public bool IsArchived;
        public int DisplayOrder;
    }

    public struct CustomField
    {
    }

    public class Project
    {
        public int Id { get; }
        public string Key { get; set; }
        public string Name { get; set; }
        public bool IsChartEnabled { get; set; }
        public bool IsSubtaskingEnabled { get; set; }
        public bool CanProjectLeaderEditProjectLeader { get; set; }
        public string TextFormattingRule { get; set; }
        public bool IsArchived { get; set; }

        internal Project(_Project data, BacklogClient client)
        {
            Id = data.id;
            Key = data.projectKey;
            Name = data.name;
            IsChartEnabled = data.chartEnabled;
            IsSubtaskingEnabled = data.subtaskingEnabled;
            CanProjectLeaderEditProjectLeader = data.projectLeaderCanEditProjectLeader;
            TextFormattingRule = data.textFormattingRule;
            IsArchived = data.archived;
            _client = client;
        }

        public async Task<BacklogResponse<User[]>> GetUsersAsync(bool excludeGroupMembers = true)
        {
            var parameters = new
            {
                excludeGroupMembers = excludeGroupMembers,
            };

            var response = await _client.GetAsync<List<_User>>($"/api/v2/projects/{Id}/users", parameters).ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<User[]>.Create(
                response,
                data.Select(x => new User(x)).ToArray());
        }

        public async Task<BacklogResponse<User>> AddUserAsync(User user)
        {
            var parameters = new
            {
                userId = user.Id,
            };

            var response = await _client.PostAsync<_User>($"/api/v2/projects/{Id}/users", parameters).ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<User>.Create(
                response,
                new User(data));
        }

        public async Task<BacklogResponse<User>> DeleteUserAsync(User user)
        {
            var parameters = new
            {
                userId = user.Id,
            };

            var response = await _client.DeleteAsync<_User>($"/api/v2/projects/{Id}/users", parameters).ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<User>.Create(
                response,
                new User(data));
        }

        public async Task<BacklogResponse<TicketType[]>> GetTicketTypesAsync()
        {
            var response = await _client.GetAsync<List<_TicketType>>($"/api/v2/projects/{Id}/issueTypes").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<TicketType[]>.Create(
                response,
                data.Select(x => new TicketType()
                {
                    Id = x.id,
                    Project = null,
                    Name = x.name,
                    Color = null,
                    DisplayOrder = x.displayOrder,
                }).ToArray());
        }

        public struct _TicketType
        {
            public int id { get; set; }
            public int projectId { get; set; }
            public string name { get; set; }
            public string color { get; set; }
            public int displayOrder { get; set; }
        }


        private BacklogClient _client;
    }
}
