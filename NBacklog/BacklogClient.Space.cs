using NBacklog.DataTypes;
using System.Net;
using System.Threading.Tasks;

namespace NBacklog
{
    public partial class BacklogClient
    {
        public async Task<BacklogResponse<Space>> GetSpaceAsync()
        {
            var response = await GetAsync("/api/v2/space").ConfigureAwait(false);
            return CreateResponse<Space, _Space>(
                response,
                HttpStatusCode.OK,
                data => new Space(data, this));
        }
    }
}
