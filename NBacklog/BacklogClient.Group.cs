﻿using NBacklog.DataTypes;
using NBacklog.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NBacklog
{
    public partial class BacklogClient
    {
        [Obsolete]
        public async Task<BacklogResponse<Group[]>> GetGroupsAsync(GroupQuery query = null)
        {
            query = query ?? new GroupQuery();

            var response = await GetAsync($"/api/v2/groups", query.Build()).ConfigureAwait(false);
            return await CreateResponseAsync<Group[], List<_Group>>(
                response,
                HttpStatusCode.OK,
                data => data.Select(x => new Group(x, this)).ToArray()).ConfigureAwait(false);
        }

        [Obsolete]
        public async Task<BacklogResponse<Group>> AddGroupAsync(Group group)
        {
            var parameters = new
            {
                name = group.Name,
                members = group.Members.Select(x => x.Id).ToArray(),
            };

            var response = await PostAsync($"/api/v2/groups", parameters).ConfigureAwait(false);
            return await CreateResponseAsync<Group, _Group>(
                response,
                HttpStatusCode.Created,
                data => new Group(data, this)).ConfigureAwait(false);
        }

        [Obsolete]
        public async Task<BacklogResponse<Group>> GetGroupAsync(int id)
        {
            var response = await GetAsync($"/api/v2/groups/{id}").ConfigureAwait(false);
            return await CreateResponseAsync<Group, _Group>(
                response,
                HttpStatusCode.OK,
                data => new Group(data, this)).ConfigureAwait(false);
        }

        [Obsolete]
        public async Task<BacklogResponse<Group>> UpdateGroupAsync(Group group)
        {
            var parameters = new
            {
                name = group.Name,
                members = group.Members.Select(x => x.Id).ToArray(),
            };

            var response = await PatchAsync($"/api/v2/groups/{group.Id}", parameters).ConfigureAwait(false);
            return await CreateResponseAsync<Group, _Group>(
                response,
                HttpStatusCode.OK,
                data => new Group(data, this)).ConfigureAwait(false);
        }

        [Obsolete]
        public async Task<BacklogResponse<Group>> DeleteGroupAsync(Group group)
        {
            var response = await DeleteAsync($"/api/v2/groups/{group.Id}").ConfigureAwait(false);
            return await CreateResponseAsync<Group, _Group>(
                response,
                HttpStatusCode.OK,
                data => new Group(data, this)).ConfigureAwait(false);
        }
    }
}
