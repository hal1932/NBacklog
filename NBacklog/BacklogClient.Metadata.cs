using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBacklog
{
    public struct StatusType
    {
        public int Id;
        public string Name;
    }

    public struct ResolutionType
    {
        public int Id;
        public string Name;
    }

	public struct PriorityType
    {
        public int Id;
        public string Name;
    }

    public partial class BacklogClient
    {
        public async Task<BacklogResponse<StatusType[]>> GetStatusTypesAsync()
        {
            var response = await GetAsync<List<_StatusType>>("/api/v2/statuses").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<StatusType[]>.Create(
                response,
                data.Select(x => new StatusType()
                {
					Id = x.id,
					Name = x.name,
                }).ToArray());
        }

        public async Task<BacklogResponse<ResolutionType[]>> GetResolutionTypesAsync()
        {
            var response = await GetAsync<List<_StatusType>>("/api/v2/resolutions").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<ResolutionType[]>.Create(
                response,
                data.Select(x => new ResolutionType()
                {
                    Id = x.id,
                    Name = x.name,
                }).ToArray());
        }

        public async Task<BacklogResponse<PriorityType[]>> GetPriorityTypeAsync()
        {
            var response = await GetAsync<List<_StatusType>>("/api/v2/priorities").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<PriorityType[]>.Create(
                response,
                data.Select(x => new PriorityType()
                {
                    Id = x.id,
                    Name = x.name,
                }).ToArray());
        }

        struct _StatusType
        {
			public int id { get; set; }
			public string name { get; set; }
        }
    }
}
