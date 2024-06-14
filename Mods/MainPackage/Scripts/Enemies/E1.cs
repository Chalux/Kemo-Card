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
                e.Binder.roleObject.IntentRichLabel.Text = StaticUtils.MakeBBCodeString(string.Format(format: "[img=30x30]res://Resources/Images/SkillFrame.png[/img]造成{0:N2}点伤害({1}+0.5*{2})"
                    , arg0: e.Binder.CurrStrength + e.Binder.Body * 0.5
                    , arg1: StaticUtils.MakeBBCodeString("力量", "center", 36, StaticInstance.BodyColor)
                    , arg2: StaticUtils.MakeBBCodeString("身体", "center", 36, StaticInstance.BodyColor)));
            }));
        }
    }
}
