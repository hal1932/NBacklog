using System;
using System.Linq;

namespace NBacklog.DataTypes
{
    public class Space : BacklogItem
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public int OwnerId { get; set; }
        public string Language { get; set; }
        public string TimeZone { get; set; }
        public string ReportSendTime { get; set; }
        public string TextFormattingRule { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        internal Space(_Space data)
            : base(-1)
        {
            Key = data.spaceKey;
            Name = data.name;
            OwnerId = data.ownerId;
            Language = data.lang;
            TimeZone = data.timezone;
            ReportSendTime = data.reportSendTime;
            TextFormattingRule = data.textFormattingRule;
            Created = data.created;
            Updated = data.updated;
        }
    }

    public class SpaceDiskUsage : BacklogItem
    {
        public int Capacity { get; set; }
        public int Issue { get; set; }
        public int Wiki { get; set; }
        public int File { get; set; }
        public int Subversion { get; set; }
        public int Git { get; set; }
        public SpaceDiskUsageDetail[] Details;

        internal SpaceDiskUsage(_SpaceDiskUsage data)
            : base(-1)
        {
            Capacity = data.capacity;
            Issue = data.issue;
            Wiki = data.wiki;
            File = data.file;
            Subversion = data.subversion;
            Git = data.git;
            Details = data.details.Select(x => new SpaceDiskUsageDetail(x)).ToArray();
        }
    }

    public class SpaceDiskUsageDetail : BacklogItem
    {
        public int ProjectId { get; set; }
        public int Issue { get; set; }
        public int Wiki { get; set; }
        public int File { get; set; }
        public int Subversion { get; set; }
        public int Git { get; set; }

        internal SpaceDiskUsageDetail(_SpaceDiskUsageDetail data)
            : base(-1)
        {
            ProjectId = data.projectId;
            Issue = data.issue;
            Wiki = data.wiki;
            File = data.file;
            Subversion = data.subversion;
            Git = data.git;
        }
    }
}
