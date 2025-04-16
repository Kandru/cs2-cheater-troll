using CounterStrikeSharp.API.Core;

namespace CheaterTroll
{
    public partial class CheaterTroll : BasePlugin
    {
        private void DebugPrint(string message)
        {
            if (Config.Debug)
            {
                Console.WriteLine(Localizer["core.debugprint"].Value.Replace("{message}", message));
            }
        }
    }
}