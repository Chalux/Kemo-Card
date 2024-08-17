using System;

namespace KemoCard.Scripts
{
    public static class ExpCfg
    {
        public const uint GainPointPerUpgrade = 3;
        public const uint MaxLevel = 30;
        public static uint CalUpgradeNeedExp(uint level)
        {
            return level < 10 ? 2 * (uint)Math.Pow(level, 2) + 10 : 3 * level + 180;
        }
    }
}
