using KemoCard.Scripts.Presets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Mods.MainPackage.Scripts.Presets
{
    internal partial class Pgiant : BasePresetScript
    {
        public override void Init(Preset preset)
        {
            preset.Enemies = new() { "giant" };
            preset.MinGoldReward = 150;
            preset.MaxGoldReward = 200;
            preset.GainExp = 50;
        }
    }
}
