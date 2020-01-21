
namespace LeadTime.Library.Git.DataTypes {
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Just cast from <langword>string</langword>.
    /// A type-safe 40-character SHA hash
    /// If you need to validate user input, you can use <see cref="CanCast(string)"/>
    /// </summary>
    public struct GitCommitHash
        : IEquatable<GitCommitHash>
    {
        public bool Equals(GitCommitHash other)
        {
            return this.value == other.value;
        }

        public override bool Equals(object obj)
        {
            return obj is GitCommitHash other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        public static bool operator ==(GitCommitHash left, GitCommitHash right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GitCommitHash left, GitCommitHash right)
        {
            return !left.Equals(right);
        }

        private static readonly Regex regex = new Regex(@"[0-9A-F]{40}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly string value;
        private GitCommitHash(string value)
        {
            this.value = value;
        }

        public static implicit operator string(GitCommitHash sha)
        {
            return sha.value;
        }

        public static explicit operator GitCommitHash(string sha)
        {
            if (!CanCast(sha))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(sha),
                    sha,
                    "Must be a SHA hash");
            }

            return new GitCommitHash(sha);
        }

        public static bool CanCast(string sha)
        {
            return regex.IsMatch(sha);
        }

        public override string ToString()
        {
            return this.value.ToString();
        }
    }
}