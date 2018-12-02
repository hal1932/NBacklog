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
            var response = await GetAsync("/api/v2/statuses").ConfigureAwait(false);
            return await CreateResponseAsync<Status[], List<_Status>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => new Status(x)).ToArray()).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<Resolution[]>> GetResolutionTypesAsync()
        {
            var response = await GetAsync("/api/v2/resolutions").ConfigureAwait(false);
            return await CreateResponseAsync<Resolution[], List<_Resolution>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => new Resolution(x)).ToArray()).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<Priority[]>> GetPriorityTypeAsync()
        {
            var response = await GetAsync("/api/v2/priorities").ConfigureAwait(false);
            return await CreateResponseAsync<Priority[], List<_Priority>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => new Priority(x)).ToArray()).ConfigureAwait(false);
        }
    }
}
