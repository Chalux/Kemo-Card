using KemoCard.Scripts;
using KemoCard.Scripts.Enemies;
using StaticClass;
using System;
using System.Collections.Generic;

namespace KemoCard.Mods.MainPackage.Scripts.Enemies
{
    internal partial class Ebat : BaseEnemyScript
    {
        public override void OnEnemyInit(EnemyImplBase e)
        {
            // 参考怪，数值模型为21+3*阶级点总点数,boss为30+3*阶级点总点数
            e.Speed = 9;
            e.Strength = 4;
            e.Dodge = 6;
            e.Critical = 5;
            e.Name = "蝙蝠";
            e.AnimationResourcePath = $"res://Mods/MainPackage/Resources/Animations/Bat.tres";
            e.ActionFunc = new((round, players, enemies, @base) =>
            {
                if (players.Count == 0) return;
                var rand = new Random();
                int idx = rand.Next(players.Count);
                List<BaseRole> list = new() { players[idx] };
                if (StaticUtils.TryGetBattleScene() is BattleScene bs)
                {
                    bs.DealDamage(e.Binder.CurrStrength + e.Binder.Body * 0.5, StaticEnums.AttackType.Physics, e.Binder, list,StaticEnums.AttributeEnum.WIND);
                }
            });
            e.AddEvent("StartBattle", new((datas) =>
            {
                string Intent =
                    $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]造成" +
                    $"{e.Binder.CurrStrength + e.Binder.Body * 0.5:N2}点风属性物理伤害(" +
                    $"{StaticUtils.MakeColorString("力量", StaticInstance.BodyColor, 36)}+0.5*" +
                    $"{StaticUtils.MakeColorString("身体", StaticInstance.BodyColor, 36)})";
                e.ChangeIntent(Intent);
            }));
        }
    }
}
