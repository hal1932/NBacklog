using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NBacklog.DataTypes
{
    public class SpaceDiskUsage : BacklogItem
    {
        public int Capacity { get; set; }
        public int Issue { get; set; }
        public int Wiki { get; set; }
        public int File { get; set; }
        public int Subversion { get; set; }
        public int Git { get; set; }
        public SpaceDiskUsageDetail[] Details;

        internal SpaceDiskUsage(_SpaceDiskUsage data)
            : base(-1)
        {
            Capacity = data.capacity;
            Issue = data.issue;
            Wiki = data.wiki;
            File = data.file;
            Subversion = data.subversion;
            Git = data.git;
            Details = data.details.Select(x => new SpaceDiskUsageDetail(x)).ToArray();
        }
    }

    public class SpaceDiskUsageDetail : BacklogItem
    {
        public int ProjectId { get; set; }
        public int Issue { get; set; }
        public int Wiki { get; set; }
        public int File { get; set; }
        public int Subversion { get; set; }
        public int Git { get; set; }

        internal SpaceDiskUsageDetail(_SpaceDiskUsageDetail data)
            : base(-1)
        {
            ProjectId = data.projectId;
            Issue = data.issue;
            Wiki = data.wiki;
            File = data.file;
            Subversion = data.subversion;
            Git = data.git;
        }
    }

    public class Space : BacklogItem
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public int OwnerId { get; set; }
        public string Language { get; set; }
        public string TimeZone { get; set; }
        public string ReportSendTime { get; set; }
        public string TextFormattingRule { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        internal Space(_Space data, BacklogClient client)
            : base(-1)
        {
            _client = client;
            Key = data.spaceKey;
            Name = data.name;
            OwnerId = data.ownerId;
            Language = data.lang;
            TimeZone = data.timezone;
            ReportSendTime = data.reportSendTime;
            TextFormattingRule = data.textFormattingRule;
            Created = data.created ?? default(DateTime);
            Updated = data.updated ?? default(DateTime);
        }

        public async Task<BacklogResponse<SpaceNotification>> GetNotificationAsync()
        {
            var response = await _client.GetAsync<_SpaceNotification>("/api/v2/space/notification").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<SpaceNotification>.Create(
                response,
                HttpStatusCode.OK,
                new SpaceNotification(data));
        }

        public async Task<BacklogResponse<SpaceNotification>> UpdateNotificationAsync(string content)
        {
            var response = await _client.PutAsync<_SpaceNotification>("/api/v2/space/notification", new { content = content })
                .ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<SpaceNotification>.Create(
                response,
                HttpStatusCode.OK,
                new SpaceNotification(data));
        }

        public async Task<BacklogResponse<SpaceDiskUsage>> GetDiskUsageAsync()
        {
            var response = await _client.GetAsync<_SpaceDiskUsage>("/api/v2/space/diskUsage")
                .ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<SpaceDiskUsage>.Create(
                response,
                HttpStatusCode.OK,
                new SpaceDiskUsage(data));
        }

        public async Task<BacklogResponse<Activity[]>> GetActivitiesAsync()
        {
            var response = await _client.GetAsync<List<_Activity>>("/api/v2/space/activities")
                .ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<Activity[]>.Create(
                response,
                HttpStatusCode.OK,
                data.Select(x => new Activity(x, _client)).ToArray());
        }

        private BacklogClient _client;
    }
}
