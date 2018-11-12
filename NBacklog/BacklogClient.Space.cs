using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBacklog
{
    public struct Space
    {
        public string Key;
        public string Name;
        public int OwnerId;
        public string Language;
        public string TimeZone;
        public string ReportSendTime;
        public string TextFormattingRule;
        public DateTime Created;
        public DateTime Updated;
    }

    public struct SpaceNotification
    {
        public string Content;
        public DateTime Updated;
    }

    public struct SpaceDiskUsage
    {
        public int Capacity;
        public int Issue;
        public int Wiki;
        public int File;
        public int Subversion;
        public int Git;
        public IReadOnlyCollection<SpaceDiskUsageDetail> Details;
    }

    public struct SpaceDiskUsageDetail
    {
        public int ProjectId;
        public int Issue;
        public int Wiki;
        public int File;
        public int Subversion;
        public int Git;
    }

    public partial class BacklogClient
    {
        public async Task<BacklogResponse<Space>> GetSpaceAsync()
        {
            var response = await GetAsync<_Space>("/api/v2/space").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<Space>.Create(
                response,
                new Space()
                {
                    Key = data.spaceKey,
                    Name = data.name,
                    OwnerId = data.ownerId,
                    Language = data.lang,
                    TimeZone = data.timezone,
                    ReportSendTime = data.reportSendTime,
                    TextFormattingRule = data.textFormattingRule,
                    Created = data.created,
                    Updated = data.updated,
                });
        }

        public async Task<BacklogResponse<SpaceNotification>> GetSpaceNotificationAsync()
        {
            var response = await GetAsync<_SpaceNotification>("/api/v2/space/notification").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<SpaceNotification>.Create(
                response,
                new SpaceNotification()
                {
                    Content = data.content,
                    Updated = data.updated,
                });
        }

        public async Task<BacklogResponse<SpaceNotification>> UpdateSpaceNotificationAsync(string content)
        {
            var response = await PutAsync<_SpaceNotification>("/api/v2/space/notification", new { content = content })
                .ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<SpaceNotification>.Create(
                response,
                new SpaceNotification()
                {
                    Content = data.content,
                    Updated = data.updated,
                });
        }

        public async Task<BacklogResponse<SpaceDiskUsage>> GetSpaceDiskUsageAsync()
        {
            var response = await GetAsync<_SpaceDiskUsage>("/api/v2/space/diskUsage")
                .ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<SpaceDiskUsage>.Create(
                response,
                new SpaceDiskUsage()
                {
                    Capacity = data.capacity,
                    Issue = data.issue,
                    Wiki = data.wiki,
                    File = data.file,
                    Subversion = data.subversion,
                    Git = data.git,
                    Details = data.details.Select(x => new SpaceDiskUsageDetail()
                    {
                        ProjectId = x.projectId,
                        Issue = x.issue,
                        Wiki = x.wiki,
                        File = x.file,
                        Subversion = x.subversion,
                        Git = x.git,
                    }).ToList().AsReadOnly(),
                });
        }

        class _Space
        {
            public string spaceKey { get; set; }
            public string name { get; set; }
            public int ownerId { get; set; }
            public string lang { get; set; }
            public string timezone { get; set; }
            public string reportSendTime { get; set; }
            public string textFormattingRule { get; set; }
            public DateTime created { get; set; }
            public DateTime updated { get; set; }
        }

        class _SpaceNotification
        {
            public string content { get; set; }
            public DateTime updated { get; set; }
        }


        class _SpaceDiskUsage
        {
            public int capacity { get; set; }
            public int issue { get; set; }
            public int wiki { get; set; }
            public int file { get; set; }
            public int subversion { get; set; }
            public int git { get; set; }
            public List<_SpaceDiskUsageDetail> details { get; set; }
        }

        class _SpaceDiskUsageDetail
        {
            public int projectId { get; set; }
            public int issue { get; set; }
            public int wiki { get; set; }
            public int file { get; set; }
            public int subversion { get; set; }
            public int git { get; set; }
        }

    }
}
