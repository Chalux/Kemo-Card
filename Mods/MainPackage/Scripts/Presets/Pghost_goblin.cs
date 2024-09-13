using KemoCard.Scripts.Presets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Mods.MainPackage.Scripts.Presets
{
    internal partial class Pghost_goblin : BasePresetScript
    {
        public override void Init(Preset preset)
        {
            preset.Enemies = new() { "ghost", "goblin" };
            preset.MinGoldReward = 80;
            preset.MaxGoldReward = 120;
            preset.GainExp = 20;
        }
    }
}
