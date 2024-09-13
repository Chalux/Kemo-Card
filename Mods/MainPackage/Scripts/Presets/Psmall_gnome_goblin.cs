using KemoCard.Scripts.Presets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Mods.MainPackage.Scripts.Presets
{
    internal partial class Psmall_gnome_goblin : BasePresetScript
    {
        public override void Init(Preset preset)
        {
            preset.Enemies = new() { "small_gnome", "goblin" };
            preset.MinGoldReward = 60;
            preset.MaxGoldReward = 80;
            preset.GainExp = 15;
        }
    }
}
