using KemoCard.Scripts.Presets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Mods.MainPackage.Scripts.Presets
{
    internal partial class Psmall_gnome : BasePresetScript
    {
        public override void Init(Preset preset)
        {
            preset.Enemies = new() { "small_gnome" };
            preset.MinGoldReward = 40;
            preset.MaxGoldReward = 75;
            preset.GainExp = 15;
        }
    }
}
