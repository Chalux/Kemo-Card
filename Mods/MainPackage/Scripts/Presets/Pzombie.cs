using KemoCard.Scripts.Presets;

namespace KemoCard.Mods.MainPackage.Scripts.Presets
{
    internal partial class Pzombie : BasePresetScript
    {
        public override void Init(Preset preset)
        {
            preset.Enemies = new() { "zombie" };
            preset.MinGoldReward = 100;
            preset.MaxGoldReward = 150;
            preset.GainExp = 20;
        }
    }
}
