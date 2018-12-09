using Newtonsoft.Json;
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
            InitialDate = data.initialDate ?? default;
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
        public CustomFieldValueItem Value => Values[0];
        public CustomFieldValueItem[] Values { get; set; }
        public string OtherValue { get; set; }

        internal CustomFieldValue(_CustomFieldValue data)
            : base(data.id)
        {
            Type = (CustomFieldType)data.fieldTypeId;
            Name = data.name;
            OtherValue = data.otherValue;

            if (data.value == null)
            {
                Values = new CustomFieldValueItem[] { null };
            }
            else
            {
                var value = data.value;
                if (value is JArray)
                {
                    Values = (value as JArray).Select(x =>
                        (x.Type == JTokenType.Object) ? new CustomFieldValueItem(x as JObject) : new CustomFieldValueItem(x)
                        ).ToArray();
                }
                else if (value is JObject)
                {
                    Values = new[] { new CustomFieldValueItem(value as JObject) };
                }
                else
                {
                    Values = new[] { new CustomFieldValueItem(value) };
                }
            }
        }

        internal string ToJsonValue()
        {
            switch (Type)
            {
                case CustomFieldType.Text:
                case CustomFieldType.TextArea:
                case CustomFieldType.Numeric:
                case CustomFieldType.Date:
                    return Values[0].Name;

                case CustomFieldType.SingleList:
                case CustomFieldType.MultipleList:
                case CustomFieldType.CheckBox:
                case CustomFieldType.Radio:
                    return JsonConvert.SerializeObject(Values.Select(x => x.Name).ToArray());

                default:
                    throw new InvalidOperationException();
            }
        }
    }

    public class CustomFieldValueItem : BacklogItem
    {
        public string Name { get; set; }
        public int DisplayOrder { get; set; }

        public Type ValueType { get; }
        public int IntValue => (int)_value;
        public double DoubleValue => (double)_value;
        public DateTime DateValue => (DateTime)_value;
        public string StringValue => _value as string;

        internal CustomFieldValueItem(JObject data)
            : base(data.Value<int>("id"))
        {
            Name = data.Value<string>("name");
            DisplayOrder = data.Value<int>("displayOrder");
        }

        internal CustomFieldValueItem(JToken data)
        {
            switch (data.Type)
            {
                case JTokenType.Integer:
                    _value = (int)data.Value<long>();
                    ValueType = typeof(long);
                    break;

                case JTokenType.Float:
                    _value = data.Value<double>();
                    ValueType = typeof(double);
                    break;

                case JTokenType.Date:
                    _value = data.Value<DateTime>();
                    ValueType = typeof(DateTime);
                    break;

                case JTokenType.String:
                    var value = data.Value<string>();
                    if (DateTime.TryParse(value, out var date))
                    {
                        _value = date;
                        ValueType = typeof(DateTime);
                    }
                    else
                    {
                        _value = value;
                        ValueType = typeof(string);
                    }
                    break;

                default:
                    throw new ArgumentException($"invalid data type: {data.Type}");
            }
        }

        internal CustomFieldValueItem(object data)
        {
            if (data is long)
            {
                _value = (int)(long)data;
                ValueType = typeof(int);
            }
            else if (data is double)
            {
                _value = (double)data;
            }
            else if (data is string)
            {
                var value = data as string;
                if (DateTime.TryParse(value, out var date))
                {
                    _value = date;
                    ValueType = typeof(DateTime);
                }
                else
                {
                    _value = value;
                    ValueType = typeof(string);
                }
            }
            else
            {
                throw new ArgumentException($"invalid data type: {data.GetType()}");
            }
        }

        private object _value;
    }
}
