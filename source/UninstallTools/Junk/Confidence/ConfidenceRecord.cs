/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

namespace UninstallTools.Junk.Confidence
{
    public sealed class ConfidenceRecord
    {
        public ConfidenceRecord(int change, string reason)
        {
            Change = change;
            Reason = reason;
        }

        public ConfidenceRecord(int change)
        {
            Change = change;
        }

        public int Change { get; }
        public string Reason { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is ConfidenceRecord casted))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return (casted.Change == Change) && (casted.Reason == Reason);
        }

        public override int GetHashCode()
        {
            return Change.GetHashCode() ^ Reason.GetHashCode();
        }
    }
}