using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NBacklog.DataTypes
{
    /// <summary>
    /// API との認証に使用しているユーザー
    /// </summary>
    public class AuthorizedUser : User
    {
        internal AuthorizedUser(_User data, BacklogClient client)
            : base(data, client)
        { }

        public async Task<BacklogResponse<Star>> AddStarAsync(Ticket ticket)
        {
            return await Star.AddTo(new { issueId = ticket.Id }, _client).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<Star>> AddStarAsync(Comment comment)
        {
            return await Star.AddTo(new { commentId = comment.Id }, _client).ConfigureAwait(false);
        }

        public async Task<BacklogResponse<Ticket[]>> GetRecentlyViewedTickets()
        {
            var response = await _client.GetAsync("/api/v2/users/myself/recentlyViewedIssues")
                .ConfigureAwait(false);

            return await _client.CreateResponseAsync<Ticket[], List<_Ticket>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => new Ticket(x, null, _client)).ToArray())
                .ConfigureAwait(false);
        }
    }
}
