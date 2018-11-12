using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                data.Select(x => new User(x)).ToArray());
        }

        public async Task<BacklogResponse<User>> GetUserAsync(int id)
        {
            var response = await GetAsync<_User>($"/api/v2/users/{id}").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<User>.Create(
                response,
                new User(data));
        }

        public async Task<BacklogResponse<User>> GetMyUserAsync()
        {
            var response = await GetAsync<_User>($"/api/v2/users/myself").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<User>.Create(
                response,
                new User(data));
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
                new User(data));
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
            return BacklogResponse<User>.Create(
                response,
                new User(data));
        }

        public async Task<BacklogResponse<User>> DeleteUserAsync(int id)
        {
            var response = await DeleteAsync<_User>($"/api/v2/users/{id}").ConfigureAwait(false);
            var data = response.Data;
            return BacklogResponse<User>.Create(
                response,
                new User(data));
        }
    }

    class _User
    {
        public int id { get; set; }
        public string userId { get; set; }
        public string name { get; set; }
        public int roleType { get; set; }
        public string lang { get; set; }
        public string mailAddress { get; set; }
    }
}
