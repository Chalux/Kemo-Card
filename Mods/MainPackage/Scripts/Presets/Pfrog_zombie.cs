using KemoCard.Scripts.Presets;

namespace KemoCard.Mods.MainPackage.Scripts.Presets
{
    internal partial class Pfrog_zombie : BasePresetScript
    {
        public override void Init(Preset preset)
        {
            preset.Enemies = new() { "frog", "zombie" };
            preset.MinGoldReward = 200;
            preset.MaxGoldReward = 300;
            preset.GainExp = 50;
        }
    }
}
