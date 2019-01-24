using System;
using System.Collections.Generic;

namespace NBacklog.DataTypes
{
    class _Errors
    {
        public List<_Error> errors { get; set; }
    }

    class _Error
    {
        public string message { get; set; }
        public int code { get; set; }
        public string moreInfo { get; set; }
    }

    class _Count
    {
        public int count { get; set; }
    }

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

    class _MilestoneSummary
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? releaseDueDate { get; set; }
    }

    class _Milestone
    {
        public int id { get; set; }
        public int projectId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? releaseDueDate { get; set; }
        public bool archived { get; set; }
        public int displayOrder { get; set; }
    }

    class _TicketSummary
    {
        public int id { get; set; }
        public int keyId { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
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
        public _Resolution resolution { get; set; }
        public _Priority priority { get; set; }
        public _Status status { get; set; }
        public _User assignee { get; set; }
        public List<_Category> category { get; set; }
        public List<_Milestone> versions { get; set; }
        public List<_Milestone> milestone { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? dueDate { get; set; }
        public double? estimatedHours { get; set; }
        public double? actualHours { get; set; }
        public int? parentIssueId { get; set; }
        public _User createdUser { get; set; }
        public DateTime? created { get; set; }
        public _User updatedUser { get; set; }
        public DateTime? updated { get; set; }
        public List<_CustomFieldValue> customFields { get; set; }
        public List<_Attachment> attachments { get; set; }
        public List<_SharedFile> sharedFiles { get; set; }
        public List<_Star> stars { get; set; }
    }

    class _CommentSummary
    {
        public int id { get; set; }
        public string content { get; set; }
    }

    class _Comment
    {
        public int id { get; set; }
        public string content { get; set; }
        public List<_ChangeLog> changeLog { get; set; }
        public _User createdUser { get; set; }
        public DateTime? created { get; set; }
        public DateTime? updated { get; set; }
        public List<_Star> stars { get; set; }
        public List<_Notification> notifications { get; set; }
    }

    class _ChangeLog
    {
        public string field { get; set; }
        public string originalValue { get; set; }
        public string newValue { get; set; }
        public _AttachmentInfo attachmentInfo { get; set; }
        public _AttributeInfo attributeInfo { get; set; }
        public _NotificationInfo notificationInfo { get; set; }
    }

    class _Activity
    {
        public int id { get; set; }
        public _Project project { get; set; }
        public int type { get; set; }
        public Newtonsoft.Json.Linq.JObject content { get; set; }
        public _User createdUser { get; set; }
        public DateTime? created { get; set; }
    }

    class _AttachmentInfo
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    class _AttributeInfo
    {
        public int id { get; set; }
        public string typeId { get; set; }
    }

    class _Notification
    {
        public int id { get; set; }
        public bool alreadyRead { get; set; }
        public int reason { get; set; }
        public _User user { get; set; }
        public bool resourceAlreadyRead { get; set; }
    }

    class _NotificationInfo
    {
        public string type { get; set; }
    }

    class _Attachment
    {
        public int id { get; set; }
        public string name { get; set; }
        public long size { get; set; }
        public _User createdUser { get; set; }
        public DateTime? created { get; set; }
    }

    class _SharedFile
    {
        public int id { get; set; }
        public string type { get; set; }
        public string dir { get; set; }
        public string name { get; set; }
        public long? size { get; set; }
        public _User createdUser { get; set; }
        public DateTime? created { get; set; }
        public _User updatedUser { get; set; }
        public DateTime? updated { get; set; }
    }

    class _Star
    {
        public int id { get; set; }
        public object comment { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public _User presenter { get; set; }
        public DateTime? created { get; set; }
    }

    class _GroupSummary
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    class _Group
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<_User> members { get; set; }
        public int displayOrder { get; set; }
        public _User createdUser { get; set; }
        public DateTime? created { get; set; }
        public _User updatedUser { get; set; }
        public DateTime? updated { get; set; }
    }

    class _Team
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<_User> members { get; set; }
        public int displayOrder { get; set; }
        public _User createdUser { get; set; }
        public DateTime? created { get; set; }
        public _User updatedUser { get; set; }
        public DateTime? updated { get; set; }
    }

    class _Status
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    class _Resolution
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    class _Priority
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

    class _ProjectDiskUsage
    {
        public int projectId { get; set; }
        public int issue { get; set; }
        public int wiki { get; set; }
        public int file { get; set; }
        public int subversion { get; set; }
        public int git { get; set; }
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
        public DateTime? created { get; set; }
        public DateTime? updated { get; set; }
    }

    class _SpaceNotification
    {
        public string content { get; set; }
        public DateTime? updated { get; set; }
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

    class _CustomFieldInitialDate
    {
        public int id { get; set; }
        public int? shift { get; set; }
        public DateTime? date { get; set; }
    }

    class _CustomField
    {
        public int id { get; set; }
        public int typeId { get; set; }
        public long version { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool required { get; set; }
        public bool useIssueType { get; set; }
        public List<int> applicableIssueTypes { get; set; }

        public string min { get; set; }
        public string max { get; set; }

        public double? initialValue { get; set; }
        public string unit { get; set; }

        public _CustomFieldInitialDate initialDate { get; set; }

        public List<_ListCustomFieldItem> items { get; set; }
        public bool? allowAddItem { get; set; }
        public bool? allowInput { get; set; }
    }

    class _ListCustomFieldItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public int displayOrder { get; set; }
    }

    class _CustomFieldValue
    {
        public int id { get; set; }
        public int fieldTypeId { get; set; }
        public string name { get; set; }
        public object value { get; set; } // _CustomFieldItemValue or List<_CustomFieldItemValue>
        public string otherValue { get; set; }
    }

    class _CustomFieldItemValue
    {
        public int id { get; set; }
        public string name { get; set; }
        public int displayOrder { get; set; }
    }

    class _Webhook
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string hookUrl { get; set; }
        public bool allEvent { get; set; }
        public List<int> activityTypeIds { get; set; }
        public _User createdUser { get; set; }
        public DateTime? created { get; set; }
        public _User updatedUser { get; set; }
        public DateTime? updated { get; set; }
    }

    class _WikipageSummary
    {
        public int id { get; set; }
        public string name { get; set; }
        public string content { get; set; }
        public List<_Attachment> attachments { get; set; }
        public List<_SharedFile> shared_files { get; set; }
    }

    class _WikipageTag
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    class _Wikipage
    {
        public int id { get; set; }
        public int projectId { get; set; }
        public string name { get; set; }
        public string content { get; set; }
        public List<_WikipageTag> tags { get; set; }
        public List<_Attachment> attachments { get; set; }
        public List<_SharedFile> sharedFiles { get; set; }
        public List<_Star> stars { get; set; }
        public _User createdUser { get; set; }
        public DateTime? created { get; set; }
        public _User updatedUser { get; set; }
        public DateTime? updated { get; set; }
    }

    class _GitRepoSummary
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    class _GitRepository
    {
        public int id { get; set; }
        public int projectId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string hookUrl { get; set; }
        public string httpUrl { get; set; }
        public string sshUrl { get; set; }
        public int displayOrder { get; set; }
        public DateTime? pushedAt { get; set; }
        public _User createdUser { get; set; }
        public DateTime? created { get; set; }
        public _User updater { get; set; }
        public DateTime? updated { get; set; }
    }

    class _PullRequestSummary
    {
        public int id { get; set; }
        public int number { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
    }

    class _PullRequestStatus
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    class _PullRequestIssue
    {
        public int id { get; set; }
    }

    class _PullRequest
    {
        public int id { get; set; }
        public int projectId { get; set; }
        public int repositoryId { get; set; }
        public int number { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
        public string @base { get; set; }
        public string branch { get; set; }
        public _PullRequestStatus status { get; set; }
        public _User assignee { get; set; }
        public _PullRequestIssue issue { get; set; }
        public string baseCommit { get; set; }
        public string branchCommit { get; set; }
        public DateTime? closeAt { get; set; }
        public DateTime? mergeAt { get; set; }
        public _User createdUser { get; set; }
        public DateTime? created { get; set; }
        public _User updatedUser { get; set; }
        public DateTime? updated { get; set; }
        public List<_Attachment> attachments { get; set; }
        public List<_Star> stars { get; set; }
    }
}
