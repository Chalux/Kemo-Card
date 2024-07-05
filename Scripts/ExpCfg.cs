namespace KemoCard.Scripts
{
    public static class ExpCfg
    {
        public static uint GainPointPerUpgrade = 3;
        public static uint MaxLevel = 30;
        public static uint CalUpgradeNeedExp(uint level)
        {
            return level < 10 ? 2 * level ^ 2 + 10 : level + 210;
        }
    }
}
