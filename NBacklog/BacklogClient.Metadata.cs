using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBacklog
{
    public partial class BacklogClient
    {
        public async Task<BacklogResponse<StatusType[]>> GetStatusTypesAsync()
        {
            var response = await GetAsync<List<_StatusType>>("/api/v2/statuses").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<StatusType[]>.Create(
                response,
                data.Select(x => new StatusType(x)).ToArray());
        }

        public async Task<BacklogResponse<ResolutionType[]>> GetResolutionTypesAsync()
        {
            var response = await GetAsync<List<_ResolutionType>>("/api/v2/resolutions").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<ResolutionType[]>.Create(
                response,
                data.Select(x => new ResolutionType(x)).ToArray());
        }

        public async Task<BacklogResponse<PriorityType[]>> GetPriorityTypeAsync()
        {
            var response = await GetAsync<List<_PriorityType>>("/api/v2/priorities").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<PriorityType[]>.Create(
                response,
                data.Select(x => new PriorityType(x)).ToArray());
        }
    }
}
