using NBacklog.DataTypes;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NBacklog
{
    public partial class BacklogClient
    {
        public async Task<BacklogResponse<Status[]>> GetStatusTypesAsync()
        {
            var response = await GetAsync<List<_Status>>("/api/v2/statuses").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<Status[]>.Create(
                response,
                HttpStatusCode.OK,
                data.Select(x => new Status(x)).ToArray());
        }

        public async Task<BacklogResponse<Resolution[]>> GetResolutionTypesAsync()
        {
            var response = await GetAsync<List<_Resolution>>("/api/v2/resolutions").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<Resolution[]>.Create(
                response,
                HttpStatusCode.OK,
                data.Select(x => new Resolution(x)).ToArray());
        }

        public async Task<BacklogResponse<Priority[]>> GetPriorityTypeAsync()
        {
            var response = await GetAsync<List<_Priority>>("/api/v2/priorities").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<Priority[]>.Create(
                response,
                HttpStatusCode.OK,
                data.Select(x => new Priority(x)).ToArray());
        }
    }
}
