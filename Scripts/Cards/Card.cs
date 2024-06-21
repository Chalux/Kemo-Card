using Godot;
using StaticClass;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using static StaticClass.StaticEnums;

namespace KemoCard.Scripts.Cards
{
    public class Card
    {
        [JsonIgnore]
        public BaseRole owner;
        public uint Id { get; set; } = 0;
        // 这里的装备id是为了记录是否是装备加到卡组里的，idx同。非装备的话可以不填。
        public string EquipId { get; set; } = "";
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
        public string GetDesc
        {
            get
            {
                string str = "";
                HashSet<string> sets = new();
                foreach (var key in GlobalDict.Keys)
                {
                    if (!sets.Contains(key) && HintDictionary.ContainsKey(key))
                    {
                        var data = HintDictionary[key];
                        str += $"{data.Alias}:{data.Desc}\n";
                        sets.Add(key);
                    }
                }
                foreach (var key in InGameDict.Keys)
                {
                    if (!sets.Contains(key) && HintDictionary.ContainsKey(key))
                    {
                        var data = HintDictionary[key];
                        str += $"{data.Alias}:{data.Desc}\n";
                        sets.Add(key);
                    }
                }
                if (str != "") str = str.TrimEnd('\n');
                return str;
            }
        }
        public int Cost { get; set; } = 0;
        public CostType CostType { get; set; } = CostType.ACTIONPOINT;
        [JsonIgnore]
        public Func<BaseRole, List<BaseRole>, dynamic[], bool> UseFilter { get; set; }
        [JsonIgnore]
        public Action<BaseRole, List<BaseRole>, dynamic[]> FunctionUse { get; set; }
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
                var path = $"res://Mods/{cfg.mod_id}/Scripts/Cards/C{id}.cs";
                var res = ResourceLoader.Load<CSharpScript>(path);
                if (res != null)
                {
                    BaseCardScript @base = res.New().As<BaseCardScript>();
                    @base.OnCardScriptInit(this);
                }
                else
                {
                    string errorLog = "未找到卡牌脚本资源,id:" + id;
                    StaticInstance.MainRoot.ShowBanner(errorLog);
                    GD.PrintErr(errorLog);
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
        }
    }
}
