using KemoCard.Scripts.Presets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Mods.MainPackage.Scripts.Presets
{
    internal partial class P1 : BasePresetScript
    {
        public override void Init(Preset preset)
        {
            preset.Enemies = new() { 1 };
            preset.MaxGoldReward = 70;
            preset.MinGoldReward = 50;
            preset.GainExp = 7;
        }
    }
}
