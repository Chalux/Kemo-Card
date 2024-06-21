using Godot;
using KemoCard.Scripts.Equips;
using StaticClass;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using static KemoCard.Scripts.Datas;
using static StaticClass.StaticEnums;

namespace KemoCard.Scripts
{
    public class Equip
    {
        private uint _id;
        // 临时的属性修改都放在这里面，只有永久修改的才写代码改属性值
        public Dictionary<string, float> Symbol { get; set; } = new();
        public uint Id
        {
            get => _id;
            set
            {
                _id = value;
                InitScript();
            }
        }
        public string Uuid { get; set; }
        [JsonIgnore]
        public BaseRole owner;
        public EquipType EquipType { get; set; } = EquipType.OTHER;
        public Equip(uint id, BaseRole owner = null)
        {
            this.owner = owner;
            Id = id;
        }
        public Equip()
        {
        }
        [JsonIgnore]
        public EquipImplBase EquipScript { get; set; } = new();
        public EquipStruct @struct;
        private void InitScript()
        {
            EquipScript = new();
            if (Ins.EquipPool.ContainsKey(Id))
            {
                @struct = Ins.EquipPool[Id];
                string path = $"res://Mods/{@struct.mod_id}/Scripts/Equips/EQ{Id}.cs";
                var res = FileAccess.Open(path, FileAccess.ModeFlags.Read);
                if (res == null)
                {
                    string errorLog = "未找到装备脚本资源,id:" + Id;
                    StaticInstance.MainRoot.ShowBanner(errorLog);
                    GD.PrintErr(errorLog);
                }
                else
                {
                    var s = ResourceLoader.Load<CSharpScript>(path).New().As<BaseEquipScript>();
                    s.OnEquipInit(EquipScript);
                }
                EquipType = (EquipType)@struct.equip_type;
            }
            else
            {
                string errorLog = "未在脚本库中找到对应装备id，请检查Mod配置。id:" + Id;
                StaticInstance.MainRoot.ShowBanner(errorLog);
                GD.PrintErr(errorLog);
            }
            EquipScript.OnCreated?.Invoke();
            EquipScript.Binder = this;
            Uuid = Guid.NewGuid().ToString();
        }

        ~Equip()
        {
            owner = null;
        }
    }
}
