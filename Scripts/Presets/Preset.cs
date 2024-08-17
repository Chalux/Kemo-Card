using Godot;
using System.Collections.Generic;

namespace KemoCard.Scripts.Presets
{
    public class Preset
    {
        public List<string> Enemies { get; set; } = new() { "bat" };
        public uint Tier { get; set; } = 1;
        public int MinGoldReward { get; set; } = 0;
        public int MaxGoldReward { get; set; } = 0;
        public int GainExp { get; set; } = 0;
        public bool IsBoss { get; set; } = false;

        public Preset(string id)
        {
            if (Datas.Ins.PresetPool.ContainsKey(id))
            {
                var modinfo = Datas.Ins.PresetPool[id];
                Tier = modinfo.tier;
                IsBoss = modinfo.is_boss;
                var path = $"res://Mods/{modinfo.mod_id}/Scripts/Presets/P{modinfo.preset_id}.cs";
                var res = ResourceLoader.Load<CSharpScript>(path);
                if (res != null)
                {
                    var PresetScript = res.New().As<BasePresetScript>();
                    PresetScript.Init(this);
                }
            }
        }

        public Preset() { }
    }
}
