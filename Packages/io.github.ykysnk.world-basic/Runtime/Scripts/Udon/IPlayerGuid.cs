using JetBrains.Annotations;

namespace io.github.ykysnk.WorldBasic.Udon
{
    [PublicAPI]
    public interface IPlayerGuid
    {
        PlayerGuid PlayerGuid { get; set; }
    }
}