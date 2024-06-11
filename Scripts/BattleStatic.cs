using System.Collections.Generic;

namespace KemoCard.Scripts
{
    internal class BattleStatic
    {
        public static CardObject currCard = null;
        public static HashSet<BaseRole> Targets = new();

        public static void Reset()
        {
            currCard = null;
            Targets.Clear();
        }
    }
}
