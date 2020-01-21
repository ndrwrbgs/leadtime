
namespace LeadTime.Library
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public struct DateRange : IComparable<DateRange>, IComparable, IEquatable<DateRange>
    {
        public DateTimeOffset StartInclusive { get; }
        public DateTimeOffset EndExclusive { get; }

        public bool Equals(DateRange other)
        {
            return this.StartInclusive.Equals(other.StartInclusive)
                   && this.EndExclusive.Equals(other.EndExclusive);
        }

        public override bool Equals(object obj)
        {
            return obj is DateRange other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.StartInclusive.GetHashCode() * 397) ^ this.EndExclusive.GetHashCode();
            }
        }

        public static bool operator ==(DateRange left, DateRange right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DateRange left, DateRange right)
        {
            return !left.Equals(right);
        }

        public DateRange(
            DateTimeOffset startInclusive,
            DateTimeOffset endExclusive)
        {
            if (startInclusive >= endExclusive)
            {
                throw new ArgumentException(
                    $"{nameof(startInclusive)} must be before {nameof(endExclusive)}");
            }

            this.StartInclusive = startInclusive;
            this.EndExclusive = endExclusive;
        }

        public bool Contains(DateTimeOffset date)
        {
            return date >= StartInclusive
                   && date < EndExclusive;
        }

        public int CompareTo(DateRange other)
        {
            int startInclusiveComparison = this.StartInclusive.CompareTo(other.StartInclusive);
            if (startInclusiveComparison != 0)
            {
                return startInclusiveComparison;
            }

            return this.EndExclusive.CompareTo(other.EndExclusive);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return 1;
            }

            return obj is DateRange other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(DateRange)}");
        }

        public static bool operator <(DateRange left, DateRange right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(DateRange left, DateRange right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(DateRange left, DateRange right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(DateRange left, DateRange right)
        {
            return left.CompareTo(right) >= 0;
        }

        public override string ToString()
        {
            return $"[{this.StartInclusive}, {this.EndExclusive})";
        }
    }
}