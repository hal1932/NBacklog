using System;
using System.Linq;

namespace NBacklog.DataTypes
{
    public class Webhook : CachableBacklogItem
    {
        public Project Project { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string HookUrl { get; set; }
        public bool IsAllActivitiesHooked => HookedActivities == null;
        public ActivityEvent[] HookedActivities { get; set; }
        public User Creator { get; }
        public DateTime Created { get; }
        public User LastUpdater { get; }
        public DateTime? LastUpdated { get; }

        public Webhook(int id)
            : base(id)
        { }

        public Webhook(string name, string hookUrl, ActivityEvent[] activities = null)
            : base(-1)
        {
            Name = name;
            HookUrl = hookUrl;
            HookedActivities = activities ?? Activity.AllEvents;
        }

        internal Webhook(_Webhook data, Project project)
            : base(data.id)
        {
            Project = project;
            Name = data.name;
            Description = data.description;
            HookUrl = data.hookUrl;
            Creator = project.Client.ItemsCache.Update(data.createdUser.id, () => new User(data.createdUser, project.Client));
            Created = data.created;
            LastUpdater = project.Client.ItemsCache.Update(data.updatedUser?.id, () => new User(data.updatedUser, project.Client));
            LastUpdated = data.updated;

            HookedActivities = data.allEvent ? null : data.activityTypeIds.Select(x => (ActivityEvent)x).ToArray();
        }

        internal QueryParameters ToApiParameters(bool toCreate)
        {
            var parameters = new QueryParameters();

            parameters.Add("name", Name, toCreate);
            parameters.Add("hookUrl", HookUrl, toCreate);

            parameters.Add("description", Description);
            parameters.Add("allEvent", IsAllActivitiesHooked);

            if (!IsAllActivitiesHooked)
            {
                parameters.AddRange("activityTypeIds[]", HookedActivities.Cast<int>());
            }

            return parameters;
        }

        public bool Contains(ActivityEvent activity)
        {
            return IsAllActivitiesHooked ? true : HookedActivities.Contains(activity);
        }
    }
}
