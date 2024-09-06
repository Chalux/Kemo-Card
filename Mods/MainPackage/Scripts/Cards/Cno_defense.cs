using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;
using System.Collections.Generic;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Cno_defense : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.UseFilter = new((user, targets, datas) =>
            {
                return targets != null && targets.Count > 0 && targets[0] is EnemyRole er && er.isFriendly == false;
            });
            c.FunctionUse = new((user, targets, datas) =>
            {
                if (StaticInstance.currWindow is BattleScene bs)
                {
                    if (BattleStatic.isFighting)
                    {
                        var pr = user as PlayerRole;
                        List<Card> cards = new();
                        foreach (var card in pr.InFightHands)
                        {
                            if ((card.FilterFlags & (ulong)StaticEnums.CardFlag.ARMOR) != 0)
                            {
                                cards.Add(card);
                            }
                        }
                        bs.DisCard(cards, pr, BattleScene.DisCardReason.EFFECT, user);
                        bs.DealDamage(4, StaticEnums.AttackType.Physics, user, targets, StaticEnums.AttributeEnum.None, cards.Count);
                    }
                }
            });
        }
    }
}
