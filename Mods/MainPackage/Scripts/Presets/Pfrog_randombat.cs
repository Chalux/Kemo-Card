using KemoCard.Scripts.Presets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Mods.MainPackage.Scripts.Presets
{
    internal partial class Pfrog_randombat : BasePresetScript
    {
        public override void Init(Preset preset)
        {
            preset.Enemies = new() { "frog" };
            Random r = new();
            var i = r.Next(0, 2);
            switch (i)
            {
                case 0:
                    preset.Enemies.Add("bat");
                    break;
                case 1:
                    preset.Enemies.Add("fire_bat");
                    break;
                case 2:
                    preset.Enemies.Add("water_bat");
                    break;
                default:
                    break;
            }
            preset.MinGoldReward = 120;
            preset.MaxGoldReward = 170;
            preset.GainExp = 35;
        }
    }
}
