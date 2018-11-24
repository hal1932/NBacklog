using System;

namespace NBacklog.DataTypes
{
    public class Milestone : CachableBacklogItem
    {
        public Project Project { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsArchived { get; set; }
        public int DisplayOrder { get; set; }

        public Milestone(Project project, string name)
            : base(-1)
        {
            Project = project;
            Name = name;
        }

        internal Milestone(_Milestone data, Project project)
            : base(data.id)
        {
            Project = project;
            Name = data.name;
            Description = data.description;
            StartDate = data.startDate ?? default(DateTime);
            DueDate = data.releaseDueDate ?? default(DateTime);
            IsArchived = data.archived;
            DisplayOrder = data.displayOrder;
        }

        internal QueryParameters ToApiParameters()
        {
            var parameters = new QueryParameters();
            parameters.Add("name", Name);
            if (Description != null) parameters.Add("description", Description);
            if (StartDate != default(DateTime)) parameters.Add("startDate", StartDate.ToString("yyyy-MM-dd"));
            if (DueDate != default(DateTime)) parameters.Add("releaseDueData", DueDate.ToString("yyyy-MM-dd"));
            return parameters;
        }
    }
}
