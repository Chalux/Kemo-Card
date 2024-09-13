using KemoCard.Scripts.Presets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Mods.MainPackage.Scripts.Presets
{
    internal partial class Pgoblin : BasePresetScript
    {
        public override void Init(Preset preset)
        {
            preset.Enemies = new() { "goblin" };
            preset.MinGoldReward = 20;
            preset.MaxGoldReward = 30;
            preset.GainExp = 5;
        }
    }
}
