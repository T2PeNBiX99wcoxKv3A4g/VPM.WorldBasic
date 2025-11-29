using System;

namespace io.github.ykysnk.WorldBasic
{
    [Serializable]
    public struct UniqueID
    {
        public string value;

        public override string ToString() => $"ID: {value}";

        public static implicit operator string(UniqueID uniqueID) => uniqueID.value;

        public static implicit operator UniqueID(string value) => new()
        {
            value = value
        };
    }
}