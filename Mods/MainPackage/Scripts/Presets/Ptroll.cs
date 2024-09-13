using KemoCard.Scripts.Presets;

namespace KemoCard.Mods.MainPackage.Scripts.Presets
{
    internal partial class Ptroll : BasePresetScript
    {
        public override void Init(Preset preset)
        {
            preset.Enemies = new() { "troll" };
            preset.MinGoldReward = 100;
            preset.MaxGoldReward = 150;
            preset.GainExp = 50;
        }
    }
}
