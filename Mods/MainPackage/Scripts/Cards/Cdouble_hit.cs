using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;
using System;
using System.Collections.Generic;
using static BattleScene;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Cdouble_hit : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.UseFilter = new((_, _, _) => false);
            c.DiscardAction = new((BaseRole user, DisCardReason reason, BaseRole from) =>
            {
                if (reason == DisCardReason.EFFECT)
                {
                    if (StaticInstance.currWindow is BattleScene bs)
                    {
                        if (BattleStatic.isFighting)
                        {
                            List<BaseRole> tars = new();
                            Random r = new();
                            tars.Add(bs.currentEnemyRoles[r.Next(bs.currentEnemyRoles.Count)]);
                            bs.DealDamage(14, StaticEnums.AttackType.Physics, user, tars, StaticEnums.AttributeEnum.None);
                        }
                    }
                }
            });
        }
    }
}
