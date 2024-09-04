using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;
using System.Collections.Generic;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Ccreate_book : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.Cost = 1;
            c.Alias = "创术";
            c.Desc = $"消耗。从2张牌中选择1张牌临时加入你的牌组。敌方单体造成5 + (1*书写)点无属性魔法伤害/获得2 + (1*书写)点魔甲";
            c.TargetType = StaticEnums.TargetType.SELF;
            c.CostType = StaticEnums.CostType.ACTIONPOINT;
            c.GlobalDict.Add("Exhaust", 1);
            c.FunctionUse = new(UseFunction);
        }

        private void UseFunction(BaseRole user, List<BaseRole> targets, params object[] datas)
        {
            Card c1 = new()
            {
                Alias = "创术-攻击",
                Cost = 1,
                Desc = $"造成5 + (1*书写)点无属性魔法伤害",
                TargetType = StaticEnums.TargetType.ENEMY_SINGLE,
                CostType = StaticEnums.CostType.ACTIONPOINT,
                FunctionUse = new(C1UseFunction),
            };
            Card c2 = new()
            {
                Alias = "创术-防御",
                Cost = 1,
                Desc = $"获得2 + (1*书写)点魔甲",
                TargetType = StaticEnums.TargetType.SELF,
                CostType = StaticEnums.CostType.ACTIONPOINT,
                FunctionUse = new(C2UseFunction),
            };
            List<Card> cs = new() { c1, c2 };
            var bs = StaticUtils.TryGetBattleScene();
            if (bs != null)
            {
                PackedScene res = ResourceLoader.Load<PackedScene>("res://Pages/ChoseCardScene.tscn");
                if (res != null)
                {
                    ChoseCardScene scene = res.Instantiate<ChoseCardScene>();
                    StaticInstance.windowMgr.AddScene(scene, cs, 1, 1);
                }
            }
        }

        private void C1UseFunction(BaseRole user, List<BaseRole> targets, params object[] datas)
        {
            var bs = StaticUtils.TryGetBattleScene();
            bs?.DealDamage(5 + user.CurrCraftBook, StaticEnums.AttackType.Magic, user, targets);
        }

        private void C2UseFunction(BaseRole user, List<BaseRole> targets, params object[] datas)
        {
            var bs = StaticUtils.TryGetBattleScene();
            if (bs != null)
            {
                (user as PlayerRole).CurrMBlock += 2 + user.CurrCraftBook;
            }
        }
    }
}
