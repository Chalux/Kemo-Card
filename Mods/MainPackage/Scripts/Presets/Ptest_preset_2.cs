using KemoCard.Scripts.Presets;

namespace KemoCard.Mods.MainPackage.Scripts.Presets
{
    internal partial class Ptest_preset_2 : BasePresetScript
    {
        public override void Init(Preset preset)
        {
            preset.Enemies = new() { "fire_bat" };
            preset.MaxGoldReward = 70;
            preset.MinGoldReward = 50;
            preset.GainExp = 7;
        }
    }
}
