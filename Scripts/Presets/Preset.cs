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
        public bool IsBoss { get; set; }

        public Preset(string id)
        {
            if (!Datas.Ins.PresetPool.TryGetValue(id, out var value)) return;
            Tier = value.Tier;
            IsBoss = value.IsBoss;
            var presetScript = PresetFactory.CreatePreset(id);
            presetScript?.Init(this);
        }

        public Preset()
        {
        }
    }
}