using KemoCard.Scripts;
using KemoCard.Scripts.Enemies;
using StaticClass;
using System;
using System.Collections.Generic;

namespace KemoCard.Mods.MainPackage.Scripts.Enemies
{
    internal partial class Ezombie : BaseEnemyScript
    {
        public override void OnEnemyInit(EnemyImplBase enemy)
        {
            enemy.Speed = 6;
            enemy.Strength = 24;
            enemy.Critical = 6;
            enemy.Name = "丧尸";
            enemy.AnimationResourcePath = $"res://Mods/MainPackage/Resources/Animations/Bat.tres";
            enemy.ActionFunc = ZombieAction;
            string Intent =
                    $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]造成" +
                    $"{enemy.Binder.CurrStrength:N2}点物理伤害(" +
                    $"{StaticUtils.MakeColorString("力量", StaticInstance.BodyColor, 36)})，将造成的伤害值转化为生命值治疗丧尸";
            enemy.ChangeIntent(Intent);
        }

        private void ZombieAction(int round, List<PlayerRole> players, List<EnemyRole> enemies, EnemyImplBase @base)
        {
            var bs = StaticUtils.TryGetBattleScene();
            if (bs != null)
            {
                if (players.Count == 0) return;
                Random r = new();
                int index = r.Next(players.Count);
                List<BaseRole> list = new() { players[index] };
                var oldHealth = list[0].CurrHealth;
                bs.DealDamage(@base.Binder.CurrStrength, StaticEnums.AttackType.Physics, @base.Binder, list);
                if (oldHealth > list[0].CurrHealth)
                {
                    @base.Binder.CurrHealth += oldHealth - list[0].CurrHealth;
                }
            }
        }
    }
}
