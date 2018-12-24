using NBacklog.Query;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NBacklog.DataTypes
{
    public enum UserRole
    {
        Admin = 1,
        User = 2,
        Reporter = 3,
        Viewer = 4,
        GuestReporter = 5,
        GuestViewer = 6,
    }

    public class UserSummary : CachableBacklogItem
    {
        public string UserId { get; }
        public string Name { get; set; }
        public UserRole Role { get; set; }
        public string Language { get; }
        public string MailAddress { get; set; }

        internal UserSummary(_User data)
            : base(data.id)
        {
            UserId = data.userId;
            Name = data.name;
            Role = (UserRole)data.roleType;
            Language = data.lang;
            MailAddress = data.mailAddress;
        }
    }

    public class User : CachableBacklogItem
    {
        public string UserId { get; }
        public string Name { get; set; }
        public UserRole Role { get; set; }
        public string Language { get; }
        public string MailAddress { get; set; }

        public User(string userId, string name, UserRole role, string mailAddress)
            : base(-1)
        {
            UserId = userId;
            Name = name;
            Role = role;
            MailAddress = mailAddress;
        }

        internal User(_User data, BacklogClient client)
            : base(data.id)
        {
            UserId = data.userId;
            Name = data.name;
            Role = (UserRole)data.roleType;
            Language = data.lang;
            MailAddress = data.mailAddress;
            _client = client;
        }

        internal User(JObject data, BacklogClient client)
            : base(data.Value<int>("id"))
        {
            UserId = data.Value<string>("userId");
            Name = data.Value<string>("name");
            Role = (UserRole)data.Value<int>("roleType");
            Language = data.Value<string>("lang");
            MailAddress = data.Value<string>("mailAddress");
            _client = client;
        }

        public async Task<BacklogResponse<MemoryStream>> GetIconAsync()
        {
            var response = await _client.GetAsync($"/api/v2/users/{Id}/icon").ConfigureAwait(false);
            return await _client.CreateResponseAsync(
                response,
                HttpStatusCode.OK,
                data => new MemoryStream(data)).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<Activity[]>> GetActivitiesAsync(ActivityQuery query = null)
        {
            query = query ?? new ActivityQuery();
            var response = await _client.GetAsync($"/api/v2/users/{Id}/activities", query.Build()).ConfigureAwait(false);
            return await _client.CreateResponseAsync<Activity[], List<_Activity>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => new Activity(x, _client)).ToArray()).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<int>> GetStarCountAsync(DateTime since = default, DateTime until = default(DateTime))
        {
            var query = new QueryParameters();
            if (since != default)
            {
                query.Add("since", since.ToString("yyyy-MM-dd"));
            }
            if (until != default)
            {
                query.Add("until", until.ToString("yyyy-MM-dd"));
            }

            var response = await _client.GetAsync($"/api/v2/users/{Id}/stars/count", query.Build()).ConfigureAwait(false);
            return await _client.CreateResponseAsync<int, _Count>(
                response,
                HttpStatusCode.OK,
                data => data.count).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<Star[]>> GetStarsAsync(StarQuery query = null)
        {
            query = query ?? new StarQuery();
            var response = await _client.GetAsync($"/api/v2/users/{Id}/stars", query.Build()).ConfigureAwait(false);
            return await _client.CreateResponseAsync<Star[], List<_Star>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => new Star(x, _client)).ToArray()).ConfigureAwait(false);
        }

        private protected BacklogClient _client;
    }
}
