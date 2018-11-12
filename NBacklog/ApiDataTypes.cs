using System;
using System.Collections.Generic;

namespace NBacklog
{
    class _TicketType
    {
        public int id { get; set; }
        public int projectId { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public int displayOrder { get; set; }
    }

    class _Category
    {
        public int id { get; set; }
        public string name { get; set; }
        public int displayOrder { get; set; }
    }

    class _Milestone
    {
        public int id { get; set; }
        public int projectId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime startDate { get; set; }
        public DateTime releaseDueDate { get; set; }
        public bool archived { get; set; }
        public int displayOrder { get; set; }
    }

    class _Ticket
    {
        public int id { get; set; }
        public int projectId { get; set; }
        public string issueKey { get; set; }
        public int keyId { get; set; }
        public _TicketType issueType { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
        public _ResolutionType resolutions { get; set; }
        public _PriorityType priority { get; set; }
        public _StatusType status { get; set; }
        public _User assignee { get; set; }
        public List<_Category> category { get; set; }
        public List<_Milestone> versions { get; set; }
        public List<_Milestone> milestone { get; set; }
        public DateTime startDate { get; set; }
        public DateTime dueDate { get; set; }
        public int estimatedHours { get; set; }
        public int actualHours { get; set; }
        public int parentIssueId { get; set; }
        public _User createdUser { get; set; }
        public DateTime created { get; set; }
        public _User updatedUser { get; set; }
        public DateTime updated { get; set; }
        //public List<_CustomField> customFields { get; set; }
        public List<_Attachment> attachments { get; set; }
        public List<_Sharedfile> sharedFiles { get; set; }
        public List<_Star> stars { get; set; }
    }

    class _Attachment
    {
        public int id { get; set; }
        public string name { get; set; }
        public int size { get; set; }
    }

    class _Sharedfile
    {
        public int id { get; set; }
        public string type { get; set; }
        public string dir { get; set; }
        public string name { get; set; }
        public int size { get; set; }
        public _User createdUser { get; set; }
        public DateTime created { get; set; }
        public _User updatedUser { get; set; }
        public DateTime updated { get; set; }
    }

    class _Star
    {
        public int id { get; set; }
        public object comment { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public _User presenter { get; set; }
        public DateTime created { get; set; }
    }

    class _Group
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<_User> members { get; set; }
        public int displayOrder { get; set; }
        public _User createdUser { get; set; }
        public DateTime created { get; set; }
        public _User updatedUser { get; set; }
        public DateTime updated { get; set; }
    }

    class _StatusType
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    class _ResolutionType
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    class _PriorityType
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    class _Project
    {
        public int id { get; set; }
        public string projectKey { get; set; }
        public string name { get; set; }
        public bool chartEnabled { get; set; }
        public bool subtaskingEnabled { get; set; }
        public bool projectLeaderCanEditProjectLeader { get; set; }
        public string textFormattingRule { get; set; }
        public bool archived { get; set; }
    }

    class _Space
    {
        public string spaceKey { get; set; }
        public string name { get; set; }
        public int ownerId { get; set; }
        public string lang { get; set; }
        public string timezone { get; set; }
        public string reportSendTime { get; set; }
        public string textFormattingRule { get; set; }
        public DateTime created { get; set; }
        public DateTime updated { get; set; }
    }

    class _SpaceNotification
    {
        public string content { get; set; }
        public DateTime updated { get; set; }
    }


    class _SpaceDiskUsage
    {
        public int capacity { get; set; }
        public int issue { get; set; }
        public int wiki { get; set; }
        public int file { get; set; }
        public int subversion { get; set; }
        public int git { get; set; }
        public List<_SpaceDiskUsageDetail> details { get; set; }
    }

    class _SpaceDiskUsageDetail
    {
        public int projectId { get; set; }
        public int issue { get; set; }
        public int wiki { get; set; }
        public int file { get; set; }
        public int subversion { get; set; }
        public int git { get; set; }
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
