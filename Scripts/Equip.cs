using Godot;
using NLua;
using StaticClass;
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
        [JsonIgnore]
        public BaseRole owner;
        [JsonIgnore]
        public Lua LuaState { get; set; }
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
        public EquipImplBase EquipScript { get; set; }
        public EquipStruct @struct;
        private void InitScript()
        {
            EquipScript = new();
            if (!Ins.EquipPool.ContainsKey(Id))
            {
                string errorLog = "未在脚本库中找到对应装备id，请检查Mod配置。id:" + Id;
                StaticInstance.MainRoot.ShowBanner(errorLog);
                GD.PrintErr(errorLog);
            }
            @struct = Ins.EquipPool[Id];
            var res = FileAccess.Open("user://Mods/" + @struct.mod_id + "/equip/EQ" + Id + ".lua", FileAccess.ModeFlags.Read);
            if (res == null)
            {
                string errorLog = "未找到装备脚本资源,id:" + Id;
                StaticInstance.MainRoot.ShowBanner(errorLog);
                GD.PrintErr(errorLog);
            }
            else
            {
                LuaState = StaticUtils.GetOneTempLua();
                LuaState["eq"] = EquipScript;
                LuaState.DoString(res.GetAsText());
            }
            EquipType = (EquipType)@struct.equip_type;
            EquipScript.OnCreated?.Invoke();
            EquipScript.Binder = this;
        }

        ~Equip()
        {
            LuaState?.Dispose();
            LuaState = null;
        }
    }
}
