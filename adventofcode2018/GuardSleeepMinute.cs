using System;

namespace adventofcode2018
{
    public class GuardSleeepMinute : IEquatable<GuardSleeepMinute>
    {
        public int Id { get; set; }
        public int Minute { get; set; }
        
        public bool Equals(GuardSleeepMinute other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Minute == other.Minute;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GuardSleeepMinute) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id * 397) ^ Minute;
            }
        }
    }
}