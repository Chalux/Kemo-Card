using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;
using System.Collections.Generic;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Ccreate_equip : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.Cost = 1;
            c.Alias = "创物";
            c.Desc = $"消耗。从2张牌中选择1张牌临时加入你的牌组。敌方单体造成5 + (1*工艺)点无属性物理伤害/获得2 + (1*工艺)点护甲";
            c.TargetType = StaticEnums.TargetType.SELF;
            c.CostType = StaticEnums.CostType.ACTIONPOINT;
            c.GlobalDict.Add("Exhaust", 1);
            c.FunctionUse = new((user, targets, datas) =>
            {
                Card c1 = new()
                {
                    Alias = "创物-攻击",
                    Cost = 1,
                    Desc = $"造成5 + (1*工艺)点无属性物理伤害",
                    TargetType = StaticEnums.TargetType.ENEMY_SINGLE,
                    CostType = StaticEnums.CostType.ACTIONPOINT,
                    FunctionUse = new(C1UseFunction)
                };
                Card c2 = new()
                {
                    Alias = "创物-防御",
                    Cost = 1,
                    Desc = $"获得2 + (1*工艺)点护甲",
                    TargetType = StaticEnums.TargetType.SELF,
                    CostType = StaticEnums.CostType.ACTIONPOINT,
                    FunctionUse = new(C2UseFunction)
                };
                List<Card> list = new() { c1, c2 };
                var bs = StaticUtils.TryGetBattleScene();
                if (bs != null)
                {
                    PackedScene res = ResourceLoader.Load<PackedScene>("res://Pages/ChoseCardScene.tscn");
                    if (res != null)
                    {
                        ChoseCardScene scene = res.Instantiate<ChoseCardScene>();
                        StaticInstance.windowMgr.AddScene(scene, list, 1, 1);
                    }
                }
            });
        }

        private void C1UseFunction(BaseRole user, List<BaseRole> targets, params object[] datas)
        {
            if (StaticInstance.currWindow is BattleScene bs)
            {
                bs.DealDamage(5 + user.CurrCraftEquip, StaticEnums.AttackType.Physics, user, targets);
            }
        }

        private void C2UseFunction(BaseRole user, List<BaseRole> targets, params object[] datas)
        {
            if (StaticInstance.currWindow is BattleScene bs)
            {
                (user as PlayerRole).CurrPBlock += 2 + user.CurrCraftEquip;
            }
        }
    }
}
