using NBacklog.DataTypes;
using System.Net;
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
                HttpStatusCode.OK,
                new Space(data));
        }

        public async Task<BacklogResponse<Notification>> GetSpaceNotificationAsync()
        {
            var response = await GetAsync<_Notification>("/api/v2/space/notification").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<Notification>.Create(
                response,
                HttpStatusCode.OK,
                new Notification(data));
        }

        public async Task<BacklogResponse<Notification>> UpdateSpaceNotificationAsync(string content)
        {
            var response = await PutAsync<_Notification>("/api/v2/space/notification", new { content = content })
                .ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<Notification>.Create(
                response,
                HttpStatusCode.OK,
                new Notification(data));
        }

        public async Task<BacklogResponse<SpaceDiskUsage>> GetSpaceDiskUsageAsync()
        {
            var response = await GetAsync<_SpaceDiskUsage>("/api/v2/space/diskUsage")
                .ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<SpaceDiskUsage>.Create(
                response,
                HttpStatusCode.OK,
                new SpaceDiskUsage(data));
        }
    }
}
