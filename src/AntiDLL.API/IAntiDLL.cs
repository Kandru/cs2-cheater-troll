namespace AntiDLL.API
{
    using CounterStrikeSharp.API.Core;

    public delegate void AntiDLLCallback(CCSPlayerController player, string eventName);

    public interface IAntiDLL
    {
        public event AntiDLLCallback OnDetection;
    }
}
