using NBacklog.DataTypes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBacklog
{
    public partial class BacklogClient
    {
        public async Task<BacklogResponse<User[]>> GetUsersAsync()
        {
            var response = await GetAsync<List<_User>>("/api/v2/users").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<User[]>.Create(
                response,
                data.Select(x => ItemsCache.Get(x.id, () => new User(x, this))).ToArray());
        }

        public async Task<BacklogResponse<User>> GetUserAsync(int id)
        {
            var response = await GetAsync<_User>($"/api/v2/users/{id}").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<User>.Create(
                response,
                ItemsCache.Get(data.id, () => new User(data, this)));
        }

        public async Task<BacklogResponse<User>> GetMyUserAsync()
        {
            var response = await GetAsync<_User>($"/api/v2/users/myself").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<User>.Create(
                response,
                ItemsCache.Get(data.id, () => new User(data, this)));
        }

        public async Task<BacklogResponse<User>> AddUserAsync(User user, string password)
        {
            var parameters = new
            {
                userId = user.UserId,
                password = password,
                name = user.Name,
                mailAddress = user.MailAddress,
                roleType = (int)user.Role,
            };

            var response = await PostAsync<_User>($"/api/v2/users", parameters).ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<User>.Create(
                response,
                ItemsCache.Get(data.id, () => new User(data, this)));
        }

        public async Task<BacklogResponse<User>> UpdateUserAsync(User user, string password = null)
        {
            object parameters;
            if (password != null)
            {
                parameters = new
                {
                    password = password,
                    name = user.Name,
                    mailAddress = user.MailAddress,
                    roleType = (int)user.Role,
                };
            }
            else
            {
                parameters = new
                {
                    name = user.Name,
                    mailAddress = user.MailAddress,
                    roleType = (int)user.Role,
                };
            }

            var response = await PatchAsync<_User>($"/api/v2/users/{user.Id}", parameters).ConfigureAwait(false);
            var data = response.Data;
            var updated = ItemsCache.Update(new User(data, this));
            return BacklogResponse<User>.Create(response, updated);
        }

        public async Task<BacklogResponse<User>> DeleteUserAsync(int id)
        {
            var response = await DeleteAsync<_User>($"/api/v2/users/{id}").ConfigureAwait(false);
            var data = response.Data;
            var deleted = ItemsCache.Delete(new User(data, this));
            return BacklogResponse<User>.Create(response, deleted);
        }
    }
}
