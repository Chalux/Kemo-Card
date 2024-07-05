using KemoCard.Scripts;
using KemoCard.Scripts.Enemies;
using StaticClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Mods.MainPackage.Scripts.Enemies
{
    internal partial class E4 : BaseEnemyScript
    {
        public override void OnEnemyInit(EnemyImplBase e)
        {
            e.Strength = 15;
            e.CraftBook = 0;
            e.CraftEquip = 0;
            e.Strength = 2;
            e.Dodge = 2;
            e.Critical = 25;
            e.Speed = 3;
            e.Name = "巨魔";
            e.AnimationResourcePath = $"res://Mods/MainPackage/Resources/Animations/Bat.tres";
            e.ActionFunc = new((round, players, enemies) =>
            {
                if (e.InGameDict.TryGetValue("action", out var tag))
                {
                    if (tag == 2)
                    {
                        if (players.Count == 0) return;
                        var rand = new Random();
                        int idx = rand.Next(players.Count);
                        List<BaseRole> list = new() { players[idx] };
                        if (StaticInstance.currWindow is BattleScene bs)
                        {
                            bs.DealDamage(e.Binder.CurrStrength + e.Binder.Body * 0.5 + StaticInstance.playerData.gsd.MajorRole.InFightHands.Count * 3, StaticEnums.AttackType.Physics, e.Binder, list);
                        }
                    }
                }
            });
            e.AddEvent("RepositionHand", new((datas) =>
            {
                if (StaticInstance.currWindow is BattleScene bs)
                {
                    if (bs.turns <= 1) return;
                }
                string Intent =
                    $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]造成" +
                    $"{e.Binder.CurrStrength + e.Binder.Body * 0.5 + StaticInstance.playerData.gsd.MajorRole.InFightHands.Count * 3:N2}点物理伤害(" +
                    $"{StaticUtils.MakeColorString("力量", StaticInstance.BodyColor, 36)}+0.5*" +
                    $"{StaticUtils.MakeColorString("身体", StaticInstance.BodyColor, 36)}+3*{StaticUtils.MakeColorString("手牌", "#f70101", 36)})";
                e.ChangeIntent(Intent);
                e.InGameDict.Add("action", 2);
            }));
            e.AddEvent("BattleStart", new((datas) =>
            {
                string Intent =
                    $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_031.png[/img]获得Buff-愤怒(每回合提高自身3点{StaticUtils.MakeColorString("力量", StaticInstance.BodyColor, 36)})";
                e.ChangeIntent(Intent);
                StaticUtils.CreateBuffAndAddToRole(10006, e.Binder);
                e.InGameDict.Add("action", 1);
            }));
            StaticUtils.CreateBuffAndAddToRole(10004, e.Binder);
            StaticUtils.CreateBuffAndAddToRole(10005, e.Binder);
        }
    }
}
