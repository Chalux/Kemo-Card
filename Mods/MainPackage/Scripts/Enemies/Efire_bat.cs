using KemoCard.Scripts;
using KemoCard.Scripts.Enemies;
using StaticClass;
using System;
using System.Collections.Generic;

namespace KemoCard.Mods.MainPackage.Scripts.Enemies
{
    internal partial class Efire_bat : BaseEnemyScript
    {
        public override void OnEnemyInit(EnemyImplBase e)
        {
            e.Speed = 9;
            e.CraftBook = 0;
            e.CraftEquip = 0;
            e.Strength = 4;
            e.Dodge = 6;
            e.Critical = 5;
            e.Mantra = 0;
            e.Effeciency = 0;
            e.Name = "火蝙蝠";
            e.AnimationResourcePath = $"res://Mods/MainPackage/Resources/Animations/Bat.tres";
            e.ActionFunc = new((round, players, enemies, @base) =>
            {
                if (players.Count == 0) return;
                var rand = new Random();
                int idx = rand.Next(players.Count);
                List<BaseRole> list = new() { players[idx] };
                if (StaticInstance.currWindow is BattleScene bs)
                {
                    bs.DealDamage(e.Binder.CurrStrength + e.Binder.Body * 0.5, StaticEnums.AttackType.Physics, e.Binder, list, StaticEnums.AttributeEnum.FIRE);
                }
            });
            e.AddEvent("StartBattle", new((datas) =>
            {
                string Intent =
                    $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]造成" +
                    $"{e.Binder.CurrStrength + e.Binder.Body * 0.5:N2}点火属性物理伤害(" +
                    $"{StaticUtils.MakeColorString("力量", StaticInstance.BodyColor, 36)}+0.5*" +
                    $"{StaticUtils.MakeColorString("身体", StaticInstance.BodyColor, 36)})";
                e.ChangeIntent(Intent);
            }));
            StaticUtils.CreateBuffAndAddToRole("water_injury", e.Binder, e.Binder);
            StaticUtils.CreateBuffAndAddToRole("fire_resis", e.Binder, e.Binder);
        }
    }
}
