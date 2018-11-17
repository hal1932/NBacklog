using Newtonsoft.Json.Linq;
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

        internal static CustomField Create(_CustomField data, Project project)
        {
            switch ((CustomFieldType)data.typeId)
            {
                case CustomFieldType.Text:
                case CustomFieldType.TextArea:
                    return new TextCustomField(data, project);

                case CustomFieldType.Numeric:
                    return new NumericCustomField(data, project);

                case CustomFieldType.Date:
                    return new DateCustomField(data, project);

                case CustomFieldType.SingleList:
                case CustomFieldType.MultipleList:
                case CustomFieldType.CheckBox:
                case CustomFieldType.Radio:
                    return new ListCustomField(data, project);

                default:
                    throw new ArgumentException($"invalid data.typeId: {data.typeId}");
            }
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

    public class TextCustomField : CustomField
    {
        internal TextCustomField(_CustomField data, Project project)
            : base(data, project)
        { }
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

    public class CustomFieldValue : CachableBacklogItem
    {
        public CustomFieldType Type { get; set; }
        public string Name { get; set; }
        public CustomFieldValueItem[] Values { get; set; }
        public string OtherValue { get; set; }

        internal CustomFieldValue(_CustomFieldValue data)
            : base(data.id)
        {
            Type = (CustomFieldType)data.fieldTypeId;
            Name = data.name;
            OtherValue = data.otherValue;

            var valueType = data.value.GetType();
            if (valueType.IsAssignableFrom(typeof(JObject)))
            {
                Values = new[] { new CustomFieldValueItem(data.value as JObject) };
            }
            else if (valueType.IsAssignableFrom(typeof(JArray)))
            {
                Values = (data.value as JArray).Select(x => new CustomFieldValueItem(x as JObject)).ToArray();
            }
            else
            {
                throw new ArgumentException($"invalid data.value: {valueType}");
            }
        }
    }

    public class CustomFieldValueItem : CachableBacklogItem
    {
        public string Name { get; set; }
        public int DisplayOrder { get; set; }

        internal CustomFieldValueItem(JObject data)
            : base(data.Value<int>("id"))
        {
            Name = data.Value<string>("name");
            DisplayOrder = data.Value<int>("displayOrder");
        }
    }
}
