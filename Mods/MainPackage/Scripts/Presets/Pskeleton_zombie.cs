using KemoCard.Scripts.Presets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Mods.MainPackage.Scripts.Presets
{
    internal partial class Pskeleton_zombie : BasePresetScript
    {
        public override void Init(Preset preset)
        {
            preset.Enemies = new() { "skeleton", "zombie" };
            preset.MinGoldReward = 250;
            preset.MaxGoldReward = 400;
            preset.GainExp = 75;
        }
    }
}
