
namespace LeadTime.Library.Core.DataTypes {
    using System;

    /// <summary>
    /// Just cast from <langword>double</langword>.
    /// A type-safe bounded double (type ensures between 0 and 1).
    /// If you need to validate user input, you can use <see cref="CanCast(double)"/>
    /// </summary>
    public struct Percentile
    {
        private readonly double value;
        private Percentile(double value)
        {
            this.value = value;
        }

        public static implicit operator double(Percentile p)
        {
            return p.value;
        }

        public static implicit operator Percentile(double p)
        {
            if (!CanCast(p))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(p),
                    p,
                    "Must be a percentile between 0 and 1 [inclusive]");
            }

            return new Percentile(p);
        }

        public static bool CanCast(double p)
        {
            return p >= 0 && p <= 1;
        }

        public override string ToString()
        {
            return this.value.ToString();
        }
    }
}