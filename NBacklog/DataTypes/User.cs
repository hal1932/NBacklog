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
            var response = await _client.GetAsync($"/api/v2/users/{Id}/icon");
            return _client.CreateResponse(
                response,
                HttpStatusCode.OK,
                data => new MemoryStream(data));
        }

        public async Task<BacklogResponse<Activity[]>> GetActivitiesAsync(ActivityQuery query = null)
        {
            query = query ?? new ActivityQuery();
            var response = await _client.GetAsync($"/api/v2/users/{Id}/activities", query.Build()).ConfigureAwait(false);
            return _client.CreateResponse<Activity[], List<_Activity>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => new Activity(x, _client)).ToArray());
        }

        public async Task<BacklogResponse<int>> GetStarCountAsync(DateTime since = default(DateTime), DateTime until = default(DateTime))
        {
            var query = new QueryParameters();
            if (since != default(DateTime))
            {
                query.Add("since", since.ToString("yyyy-MM-dd"));
            }
            if (until != default(DateTime))
            {
                query.Add("until", until.ToString("yyyy-MM-dd"));
            }

            var response = await _client.GetAsync($"/api/v2/users/{Id}/stars/count", query.Build()).ConfigureAwait(false);
            return _client.CreateResponse<int, _Count>(
                response,
                HttpStatusCode.OK,
                data => data.count);
        }

        public async Task<BacklogResponse<Star[]>> GetStarsAsync(StarQuery query = null)
        {
            query = query ?? new StarQuery();
            var response = await _client.GetAsync($"/api/v2/users/{Id}/stars", query.Build()).ConfigureAwait(false);
            return _client.CreateResponse<Star[], List<_Star>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => new Star(x, _client)).ToArray());
        }

        private BacklogClient _client;
    }
}
