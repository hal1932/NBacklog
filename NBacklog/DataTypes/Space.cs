using NBacklog.Query;
using NBacklog.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NBacklog.DataTypes
{
    public class SpaceDiskUsage : BacklogItem
    {
        public int Capacity { get; }
        public int Issue { get; }
        public int Wiki { get; }
        public int File { get; }
        public int Subversion { get; }
        public int Git { get; }
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
        public int ProjectId { get; }
        public int Issue { get; }
        public int Wiki { get; }
        public int File { get; }
        public int Subversion { get; }
        public int Git { get; }

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
            Created = data.created ?? default;
            Updated = data.updated ?? default;
        }

        public async Task<BacklogResponse<SpaceNotification>> GetNotificationAsync()
        {
            var response = await _client.GetAsync("/api/v2/space/notification").ConfigureAwait(false);
            return await _client.CreateResponseAsync<SpaceNotification, _SpaceNotification>(
                response,
                HttpStatusCode.OK,
                data => new SpaceNotification(data)).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<SpaceNotification>> UpdateNotificationAsync(string content)
        {
            var response = await _client.PutAsync("/api/v2/space/notification", new { content }).ConfigureAwait(false);
            return await _client.CreateResponseAsync<SpaceNotification, _SpaceNotification>(
                response,
                HttpStatusCode.OK,
                data => new SpaceNotification(data)).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<SpaceDiskUsage>> GetDiskUsageAsync()
        {
            var response = await _client.GetAsync("/api/v2/space/diskUsage").ConfigureAwait(false);
            return await _client.CreateResponseAsync<SpaceDiskUsage, _SpaceDiskUsage>(
                response,
                HttpStatusCode.OK,
                data => new SpaceDiskUsage(data)).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<Activity[]>> GetActivitiesAsync(ActivityQuery query = null)
        {
            query = query ?? new ActivityQuery();
            var response = await _client.GetAsync("/api/v2/space/activities", query.Build()).ConfigureAwait(false);
            return await _client.CreateResponseAsync<Activity[], List<_Activity>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => new Activity(x, _client)).ToArray()).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<MemoryStream>> GetIconAsync()
        {
            var response = await _client.GetAsync($"/api/v2/space/image").ConfigureAwait(false);
            return await _client.CreateResponseAsync(
                response,
                HttpStatusCode.OK,
                data => new MemoryStream(data)).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<Attachment>> AddAttachment(FileInfo file)
        {
            var response = await _client.SendFileAsync(
                "/api/v2/space/attachment",
                Method.POST,
                "file",
                file)
                .ConfigureAwait(false);
            return await _client.CreateResponseAsync<Attachment, _Attachment>(
                response,
                HttpStatusCode.OK,
                data => new Attachment(data, null)).ConfigureAwait(false);
        }

        private BacklogClient _client;
    }
}
