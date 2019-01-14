using System;

namespace NBacklog
{
    public abstract class BacklogItem : IEquatable<BacklogItem>
    {
        public int Id { get; }

        private protected BacklogItem() { }

        private protected BacklogItem(int id)
        {
            Id = id;
        }

        public bool Equals(BacklogItem other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(other, this))
            {
                return true;
            }

            var thisType = GetType();

            if (thisType != other.GetType())
            {
                return false;
            }

            if (Id != other.Id)
            {
                return false;
            }

            foreach (var prop in thisType.GetProperties())
            {
                var value = prop.GetValue(this);
                var otherValue = prop.GetValue(other);
                if (value == null)
                {
                    if (otherValue != null)
                    {
                        return false;
                    }
                }
                else if (!value.Equals(otherValue))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BacklogItem);
        }

        public override int GetHashCode()
        {
            var thisType = GetType();
            var code = thisType.GetHashCode();
            foreach (var prop in thisType.GetProperties())
            {
                var value = prop.GetValue(this);
                if (value != null)
                {
                    code ^= value.GetHashCode();
                }
            }
            return code;
        }

        public static bool operator ==(BacklogItem lhs, BacklogItem rhs)
        {
            if (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null))
            {
                return true;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(BacklogItem lhs, BacklogItem rhs)
        {
            return !(lhs == rhs);
        }
    }
}
