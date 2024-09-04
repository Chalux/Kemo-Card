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
    internal partial class Ezombie : BaseEnemyScript
    {
        public override void OnEnemyInit(EnemyImplBase enemy)
        {
            enemy.Speed = 6;
            enemy.Strength = 18;
            enemy.Dodge = 0;
            enemy.Critical = 6;
            enemy.Name = "丧尸";
            enemy.AnimationResourcePath = $"res://Mods/MainPackage/Resources/Animations/Bat.tres";
            enemy.ActionFunc = ZombieAction;
            string Intent = $"造成(1*力量)点无属性物理伤害，将造成的伤害值转化为生命值治疗丧尸";
            enemy.ChangeIntent(Intent);
        }

        private void ZombieAction(int round, List<PlayerRole> players, List<EnemyRole> enemies, EnemyImplBase @base)
        {
            var bs = StaticUtils.TryGetBattleScene();
            if(bs != null)
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
