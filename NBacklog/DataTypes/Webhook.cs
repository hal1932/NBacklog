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
        public bool IsAllActivitiesHooked { get; set; }
        public ActivityType[] HookedActivities { get; set; }
        public User Creator { get; }
        public DateTime Created { get; }
        public User LastUpdater { get; }
        public DateTime LastUpdated { get; }

        public Webhook(Project project, string name, string hookUrl, ActivityType[] activities = null)
            : base(-1)
        {
            Project = project;
            Name = name;
            HookUrl = hookUrl;
            HookedActivities = activities ?? Activity.GetAllTypes();
            IsAllActivitiesHooked = activities == null;
        }

        internal Webhook(_Webhook data, Project project)
            : base(data.id)
        {
            Project = project;
            Name = data.name;
            Description = data.description;
            HookUrl = data.hookUrl;
            IsAllActivitiesHooked = data.allEvent;
            HookedActivities = data.activityTypeIds.Select(x => (ActivityType)x).ToArray();
            Creator = project.Client.ItemsCache.Update(data.createdUser.id, () => new User(data.createdUser, project.Client));
            Created = data.created;
            LastUpdater = project.Client.ItemsCache.Update(data.updatedUser?.id, () => new User(data.updatedUser, project.Client));
            LastUpdated = data.updated ?? default;

            if (IsAllActivitiesHooked)
            {
                HookedActivities = Activity.GetAllTypes();
            }
        }

        internal QueryParameters ToApiParameters()
        {
            var parameters = new QueryParameters();

            parameters.Add("name", Name);
            parameters.Add("description", Description);
            parameters.Add("hookUrl", HookUrl);
            parameters.Add("allEvent", IsAllActivitiesHooked);

            if (!IsAllActivitiesHooked)
            {
                parameters.AddRange("activityTypeIds[]", HookedActivities.Cast<int>());
            }

            return parameters;
        }
    }
}
