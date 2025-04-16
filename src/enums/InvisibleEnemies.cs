using System.Runtime.Serialization;

namespace CheaterTroll
{
    public enum InvisibleEnemiesMode
    {
        [EnumMember(Value = "full")] Full,
        [EnumMember(Value = "distance")] Distance,
        [EnumMember(Value = "random")] Random
    }
}
