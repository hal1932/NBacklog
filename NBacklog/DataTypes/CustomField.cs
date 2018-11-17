using System;
using System.Linq;

namespace NBacklog.DataTypes
{
    public enum CustomFieldType
    {
        Text = 1,
        TextArea = 2,
        Numeric = 3,
        Date = 4,
        SingleList = 5,
        MultipleList = 6,
        CheckBox = 7,
        Radio = 8,
    }

    public enum DateCustomFieldInitialValueType
    {
        Today = 1,
        TodayPlusShift = 2,
        Scheduled = 3,
    }

    public abstract class CustomField : CachableBacklogItem
    {
        public Project Project { get; set; }
        public CustomFieldType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsRequired { get; set; }
        public int[] ApplicableTicketTypeIds { get; set; }

        internal static CustomField Create()
        {
            return null;
        }

        internal CustomField(_CustomField data, Project project)
            : base(data.id)
        {
            Project = project;
            Type = (CustomFieldType)data.typeId;
            Name = data.name;
            Description = data.description;
            IsRequired = data.required;
            ApplicableTicketTypeIds = data.applicableIssueTypes.ToArray();
        }
    }

    public class NumericCustomField : CustomField
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public double InitialValue { get; set; }
        public string Unit { get; set; }

        internal NumericCustomField(_CustomField data, Project project)
            : base(data, project)
        {
            Min = double.Parse(data.min);
            Max = double.Parse(data.max);
            InitialValue = data.initialValue;
            Unit = data.unit;
        }
    }

    public class DateCustomField : CustomField
    {
        public DateTime Min { get; set; }
        public DateTime Max { get; set; }
        public DateCustomFieldInitialValueType InitialValueType { get; set; }
        public DateTime InitialDate { get; set; }
        public int InitialShift { get; set; }

        internal DateCustomField(_CustomField data, Project project)
            : base(data, project)
        {
            Min = DateTime.Parse(data.min);
            Max = DateTime.Parse(data.max);
            InitialValueType = (DateCustomFieldInitialValueType)data.initialValueType;
            InitialDate = data.initialDate;
            InitialShift = data.initialShift;
        }
    }

    public class ListCustomField : CustomField
    {
        public ListCustomFieldItem[] Items { get; set; }
        public bool IsDirectInputAllowed { get; set; }
        public bool IsAdditionAllowed { get; set; }

        internal ListCustomField(_CustomField data, Project project)
            : base(data, project)
        {
            Items = data.items.Select(x => new ListCustomFieldItem(x)).ToArray();
            IsDirectInputAllowed = data.allowInput;
            IsAdditionAllowed = data.allowAddItem;
        }
    }

    public class ListCustomFieldItem : CachableBacklogItem
    {
        public string Name { get; set; }
        public int DisplayOrder { get; set; }

        internal ListCustomFieldItem(_ListCustomFieldItem data)
            : base(data.id)
        {
            Name = data.name;
            DisplayOrder = data.displayOrder;
        }
    }
}
