using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBacklog
{
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
                    }).ToArray(),
                });
        }
    }
}
