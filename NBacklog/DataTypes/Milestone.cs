using System;

namespace NBacklog.DataTypes
{
    public class Milestone : CachableBacklogItem
    {
        public Project Project { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ReleaseDueDate { get; set; }
        public bool IsArchived { get; set; }
        public int DisplayOrder { get; set; }

        internal Milestone(_Milestone data, Project project)
            : base(data.id)
        {
            Project = project;
            Name = data.name;
            Description = data.description;
            StartDate = data.startDate ?? default(DateTime);
            ReleaseDueDate = data.releaseDueDate ?? default(DateTime);
            IsArchived = data.archived;
            DisplayOrder = data.displayOrder;
        }
    }
}
