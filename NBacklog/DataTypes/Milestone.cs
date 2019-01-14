using System;

namespace NBacklog.DataTypes
{
    public class MilestoneSummary : BacklogItem
    {
        public string Name { get; }
        public string Description { get; }
        public DateTime StartDate { get; }
        public DateTime DueDate { get; }

        internal MilestoneSummary(_MilestoneSummary data)
            : base(data.id)
        {
            Name = data.name;
            Description = data.description;
            StartDate = data?.startDate ?? default;
            DueDate = data?.releaseDueDate ?? default;
        }
    }

    public class Milestone : CachableBacklogItem
    {
        public static DateTime MinDate = new DateTime(2000, 1, 1);

        public Project Project { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsArchived { get; set; }
        public int DisplayOrder { get; }

        public Milestone(int id)
            : base(id)
        { }

        public Milestone(string name)
            : base(-1)
        {
            Name = name;
        }

        internal Milestone(_Milestone data, Project project)
            : base(data.id)
        {
            Project = project;
            Name = data.name;
            Description = data.description;
            StartDate = data.startDate?.Date ?? default;
            DueDate = data.releaseDueDate?.Date ?? default;
            IsArchived = data.archived;
            DisplayOrder = data.displayOrder;
        }

        internal QueryParameters ToApiParameters()
        {
            var parameters = new QueryParameters();
            parameters.Add("name", Name);
            parameters.Add("description", Description ?? string.Empty);
            parameters.Add("startDate", (StartDate != default) ? StartDate.ToString("yyyy-MM-dd") : string.Empty);
            parameters.Add("releaseDueDate", (DueDate != default) ? DueDate.ToString("yyyy-MM-dd") : string.Empty);
            return parameters;
        }
    }
}
