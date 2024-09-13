using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;
using System.Collections.Generic;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Cno_attack : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.FunctionUse = new((user, targets, datas) =>
            {
                if (StaticUtils.TryGetBattleScene() is BattleScene bs)
                {
                    if (BattleStatic.isFighting)
                    {
                        var pr = user as PlayerRole;
                        List<Card> cards = new();
                        foreach (var card in pr.InFightHands)
                        {
                            if ((card.FilterFlags & (ulong)StaticEnums.CardFlag.ATTACK) != 0)
                            {
                                cards.Add(card);
                            }
                        }
                        bs.DisCard(cards, pr, BattleScene.DisCardReason.EFFECT, user);
                        pr.CurrPBlock += cards.Count * 3;
                        pr.CurrMBlock += cards.Count * 3;
                    }
                }
            });
        }
    }
}
