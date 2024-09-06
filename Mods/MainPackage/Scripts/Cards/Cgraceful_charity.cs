using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;
using System.Collections.Generic;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Cgraceful_charity : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.Cost = 8;
            c.Alias = "天使的施舍";
            c.Desc = "抽3张牌，然后选择2张手牌丢弃";
            c.TargetType = StaticEnums.TargetType.SELF;
            c.CostType = StaticEnums.CostType.MAGIC;
            c.UseFilter = new((user, targets, datas) =>
            {
                return targets != null && targets.Count > 0 && targets[0] is PlayerRole pr && pr.isFriendly == true;
            });
            c.FunctionUse = new((user, targets, datas) =>
            {
                if (StaticInstance.currWindow is BattleScene bs)
                {
                    if (BattleStatic.isFighting)
                    {
                        bs.DrawCard(3, (PlayerRole)targets[0]);
                        bs.SelectCard(2, 2, null, (List<Card> Cards) =>
                        {
                            bs.DisCard(Cards, (PlayerRole)targets[0], BattleScene.DisCardReason.EFFECT, user);
                        });
                    }
                }
            });
        }
    }
}
