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
    internal partial class E1 : BaseEnemyScript
    {
        public override void OnEnemyInit(EnemyImplBase e)
        {
            e.Speed = 4;
            e.Name = "蝙蝠";
            e.AnimationResourcePath = $"res://Mods/MainPackage/Resources/Animations/Bat.tres";
            e.ActionFunc = new((round, players, enemies) =>
            {
                if (players.Count == 0) return;
                var rand = new Random();
                int idx = rand.Next(players.Count);
                List<BaseRole> list = new() { players[idx] };
                if (StaticInstance.currWindow is BattleScene bs)
                {
                    bs.DealDamage(e.Binder.CurrStrength + e.Binder.Body * 0.5, StaticEnums.AttackType.Physics, e.Binder, list);
                }
            });
            e.AddEvent("StartBattle", new((datas) =>
            {
                string Intent = StaticUtils.MakeBBCodeString(
                    $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_78.png[/img]造成" +
                    $"{e.Binder.CurrStrength + e.Binder.Body * 0.5:N2}点伤害(" +
                    $"{StaticUtils.MakeColorString("力量", StaticInstance.BodyColor, 36)}+0.5*" +
                    $"{StaticUtils.MakeColorString("身体", StaticInstance.BodyColor, 36)})");
                e.ChangeIntent(Intent);
            }));
            StaticUtils.CreateBuffAndAddToRole(10002, e.Binder);
            StaticUtils.CreateBuffAndAddToRole(10001, e.Binder);
        }
    }
}
