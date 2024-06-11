using Godot;
using NLua;
using StaticClass;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using static StaticClass.StaticEnums;

namespace KemoCard.Scripts
{
    public class Card
    {
        [JsonIgnore]
        public BaseRole owner;
        public uint Id { get; set; } = 0;
        // 这里的装备id是为了记录是否是装备加到卡组里的，idx同。非装备的话可以不填。
        public uint EquipId { get; set; } = 0;
        public uint EquipIdx { get; set; } = 0;

        /// <summary>
        /// 全局的词条，可以理解为永久的词条加成
        /// </summary>
        public Dictionary<string, float> GlobalDict { get; set; } = new();
        /// <summary>
        /// 战斗内的词条，在战斗开始和结束时会清除掉
        /// </summary>
        public Dictionary<string, float> InGameDict { get; set; } = new();
        public TargetType TargetType { get; set; } = TargetType.SELF;

        // 这里的idx是记录在牌组里的idx顺位
        public uint Idx { get; set; } = 0;
        public string CardName { get; set; } = "未命名";
        public string Desc { get; set; } = "无描述";
        public int Cost { get; set; } = 0;
        public CostType CostType { get; set; } = CostType.ACTIONPOINT;
        [JsonIgnore]
        public LuaFunction FunctionUseFilter { get; set; }
        [JsonIgnore]
        public LuaFunction FunctionUse { get; set; }
        [JsonIgnore]
        private Lua LuaState { get; set; }
        public Card(uint id)
        {
            Id = id;

            if (!Datas.Ins.CardPool.ContainsKey(id))
            {
                string errorLog = "未在脚本库中找到卡牌对应id，请检查Mod配置。id:" + id;
                StaticInstance.MainRoot.ShowBanner(errorLog);
                GD.PrintErr(errorLog);
            }
            else
            {
                var cfg = Datas.Ins.CardPool[id];
                var res = FileAccess.Open("user://Mods/" + cfg.mod_id + "/card/C" + id + ".lua", FileAccess.ModeFlags.Read);
                if (res == null)
                {
                    string errorLog = "未找到卡牌脚本资源,id:" + id;
                    StaticInstance.MainRoot.ShowBanner(errorLog);
                    GD.PrintErr(errorLog);
                }
                else
                {
                    LuaState = StaticUtils.GetOneTempLua();
                    LuaState["c"] = this;
                    LuaState.DoString(res.GetAsText());
                }
            }
        }
        public Card() { }

        ~Card()
        {
            GlobalDict?.Clear();
            GlobalDict = null;
            InGameDict?.Clear();
            InGameDict = null;
            LuaState?.Dispose();
        }
    }
}
