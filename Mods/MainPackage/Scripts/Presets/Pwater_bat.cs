using KemoCard.Scripts.Presets;

namespace KemoCard.Mods.MainPackage.Scripts.Presets
{
    internal partial class Pwater_bat : BasePresetScript
    {
        public override void Init(Preset preset)
        {
            preset.Enemies = new() { "water_bat" };
            preset.MaxGoldReward = 70;
            preset.MinGoldReward = 50;
            preset.GainExp = 7;
        }
    }
}
