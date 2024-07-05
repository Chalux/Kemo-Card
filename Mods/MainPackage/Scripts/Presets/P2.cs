using KemoCard.Scripts.Presets;

namespace KemoCard.Mods.MainPackage.Scripts.Presets
{
    internal partial class P2 : BasePresetScript
    {
        public override void Init(Preset preset)
        {
            preset.Enemies = new() { 2 };
            preset.MaxGoldReward = 70;
            preset.MinGoldReward = 50;
            preset.GainExp = 7;
        }
    }
}
