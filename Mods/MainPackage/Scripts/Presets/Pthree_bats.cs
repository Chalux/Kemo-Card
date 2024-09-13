using KemoCard.Scripts.Presets;

namespace KemoCard.Mods.MainPackage.Scripts.Presets
{
    internal partial class Pthree_bats : BasePresetScript
    {
        public override void Init(Preset preset)
        {
            preset.Enemies = new() { "bat", "fire_bat", "water_bat" };
            preset.MinGoldReward = 70;
            preset.MaxGoldReward = 120;
            preset.GainExp = 15;
        }
    }
}
