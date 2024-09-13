using KemoCard.Scripts.Presets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Mods.MainPackage.Scripts.Presets
{
    internal partial class Pghost : BasePresetScript
    {
        public override void Init(Preset preset)
        {
            preset.Enemies = new() { "ghost" };
            preset.MinGoldReward = 30;
            preset.MaxGoldReward = 50;
            preset.GainExp = 10;
        }
    }
}
