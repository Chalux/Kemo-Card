using KemoCard.Scripts.Presets;

namespace KemoCard.Mods.MainPackage.Scripts.Presets
{
    internal partial class Ptroll_bat : BasePresetScript
    {
        public override void Init(Preset preset)
        {
            preset.Enemies = new() { "bat", "troll" };
            preset.MinGoldReward = 80;
            preset.MaxGoldReward = 130;
            preset.GainExp = 30;
        }
    }
}
