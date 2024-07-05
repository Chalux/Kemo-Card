using KemoCard.Scripts.Presets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Mods.MainPackage.Scripts.Presets
{
    internal partial class P4 : BasePresetScript
    {
        public override void Init(Preset preset)
        {
            preset.Enemies = new() { 4 };
            preset.MinGoldReward = 100;
            preset.MaxGoldReward = 150;
            preset.GainExp = 50;
        }
    }
}
