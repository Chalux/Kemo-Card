using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using static KemoCard.Pages.BattleScene;
using static KemoCard.Scripts.StaticEnums;

namespace KemoCard.Scripts.Cards
{
    public class Card
    {
        [JsonIgnore] public BaseRole Owner;

        public string Id { get; set; }

        // 这里的装备id是为了记录是否是装备加到卡组里的，idx同。非装备的话可以不填。
        public string EquipId { get; init; } = "";
        public uint Rare { get; }

        /// <summary>
        /// 全局的词条，可以理解为永久的词条加成
        /// </summary>
        public Dictionary<string, float> GlobalDict { get; private set; } = new();

        /// <summary>
        /// 战斗内的词条，在战斗开始和结束时会清除掉
        /// </summary>
        public Dictionary<string, float> InGameDict { get; private set; } = new();

        public Dictionary<string, float> InRoundDict { get; } = new();
        public TargetType TargetType { get; set; } = TargetType.Self;

        // 这里的idx是记录在牌组里的idx顺位
        public uint Idx { get; set; }
        public string Alias { get; set; } = "未命名";
        public string Desc { get; set; } = "无描述";
        public bool IsTemp { get; set; }
        public HashSet<string> HintCardIds { get; set; } = [];
        [JsonIgnore] public ulong FilterFlags { get; set; }

        public string GetDesc
        {
            get
            {
                var str = "";
                HashSet<string> sets = [];
                foreach (var key in GlobalDict.Keys)
                {
                    if (sets.Contains(key) || !HintDictionary.TryGetValue(key, out var data)) continue;
                    str += $"{data.Alias}:{data.Desc}\n";
                    sets.Add(key);
                }

                foreach (var key in InGameDict.Keys)
                {
                    if (sets.Contains(key) || !HintDictionary.TryGetValue(key, out var data)) continue;
                    str += $"{data.Alias}:{data.Desc}\n";
                    sets.Add(key);
                }

                foreach (var key in InRoundDict.Keys)
                {
                    if (sets.Contains(key) || !HintDictionary.TryGetValue(key, out var data)) continue;
                    str += $"{data.Alias}:{data.Desc}\n";
                    sets.Add(key);
                }

                str = (from cardId in HintCardIds
                    where Datas.Ins.CardPool.ContainsKey(cardId)
                    select Datas.Ins.CardPool[cardId]).Aggregate(str,
                    (current, cardInfo) => current + $"{cardInfo.Alias}:{cardInfo.Desc}\n");
                if (str != "") str = str.TrimEnd('\n');
                return str;
            }
        }

        public int Cost { get; set; }
        public CostType CostType { get; set; } = CostType.ActionPoint;
        [JsonIgnore] public Func<BaseRole, List<BaseRole>, dynamic[], bool> UseFilter { get; set; }
        [JsonIgnore] public Action<BaseRole, List<BaseRole>, dynamic[]> FunctionUse { get; set; }
        [JsonIgnore] public Action<BaseRole, DisCardReason, BaseRole> DiscardAction { get; set; }

        public Card(string id)
        {
            Id = id;

            if (!Datas.Ins.CardPool.TryGetValue(id, out var value))
            {
                var errorLog = "未在脚本库中找到卡牌对应id，请检查Mod配置。id:" + id;
                StaticInstance.MainRoot.ShowBanner(errorLog);
                GD.PrintErr(errorLog);
            }
            else
            {
                Rare = value.Rare;
                FilterFlags = value.FilterFlag;
                Alias = value.Alias;
                Desc = value.Desc;
                Cost = (int)value.Cost;
                TargetType = (TargetType)value.TargetType;
                CostType = (CostType)value.CostType;
                var script = CardFactory.CreateCard(id);
                script?.OnCardScriptInit(this);
            }
        }

        public Card()
        {
        }

        ~Card()
        {
            GlobalDict?.Clear();
            GlobalDict = null;
            InGameDict?.Clear();
            InGameDict = null;
        }

        public bool CheckHasSymbol(string symbol)
        {
            return GlobalDict.ContainsKey(symbol) || InGameDict.ContainsKey(symbol);
        }
    }
}