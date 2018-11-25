﻿using NBacklog.DataTypes;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NBacklog
{
    public partial class BacklogClient
    {
        public async Task<BacklogResponse<User[]>> GetUsersAsync()
        {
            var response = await GetAsync("/api/v2/users").ConfigureAwait(false);
            return CreateResponse<User[], List<_User>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => ItemsCache.Update(new User(x, this))).ToArray());
        }

        public async Task<BacklogResponse<User>> GetUserAsync(int id)
        {
            var response = await GetAsync($"/api/v2/users/{id}").ConfigureAwait(false);
            return CreateResponse<User, _User>(
                response,
                HttpStatusCode.OK,
                data => ItemsCache.Update(new User(data, this)));
        }

        public async Task<BacklogResponse<User>> GetMyUserAsync()
        {
            var response = await GetAsync($"/api/v2/users/myself").ConfigureAwait(false);
            return CreateResponse<User, _User>(
                response,
                HttpStatusCode.OK,
                data => ItemsCache.Update(new User(data, this)));
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

            var response = await PostAsync($"/api/v2/users", parameters).ConfigureAwait(false);
            return CreateResponse<User, _User>(
                response,
                HttpStatusCode.Created,
                data => ItemsCache.Update(new User(data, this)));
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

            var response = await PatchAsync($"/api/v2/users/{user.Id}", parameters).ConfigureAwait(false);
            return CreateResponse<User, _User>(
                response,
                HttpStatusCode.OK,
                data => ItemsCache.Update(new User(data, this)));
        }

        public async Task<BacklogResponse<User>> DeleteUserAsync(int id)
        {
            var response = await DeleteAsync($"/api/v2/users/{id}").ConfigureAwait(false);
            return CreateResponse<User, _User>(
                response,
                HttpStatusCode.OK,
                data => ItemsCache.Delete(new User(data, this)));
        }
    }
}
