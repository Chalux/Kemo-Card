using KemoCard.Scripts.Presets;

namespace KemoCard.Mods.MainPackage.Scripts.Presets
{
    internal partial class Pfrog : BasePresetScript
    {
        public override void Init(Preset preset)
        {
            preset.Enemies = new() { "frog" };
            preset.MinGoldReward = 100;
            preset.MaxGoldReward = 150;
            preset.GainExp = 30;
        }
    }
}
