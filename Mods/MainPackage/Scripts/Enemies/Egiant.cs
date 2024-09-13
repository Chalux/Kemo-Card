using KemoCard.Scripts;
using KemoCard.Scripts.Enemies;
using StaticClass;
using System;
using System.Collections.Generic;

namespace KemoCard.Mods.MainPackage.Scripts.Enemies
{
    internal partial class Egiant : BaseEnemyScript
    {
        private EnemyImplBase _data;
        public override void OnEnemyInit(EnemyImplBase e)
        {
            _data = e;
            e.Speed = 3;
            e.Strength = 20;
            e.Critical = 13;
            e.Binder.CurrPBlock = 20;
            e.Binder.CurrMBlock = 20;
            e.Name = "巨人";
            e.AnimationResourcePath = $"res://Mods/MainPackage/Resources/Animations/Bat.tres";
            e.ActionFunc = ActionFunc;
            e.AddEvent("StartBattle", UpdateIntent);
        }

        private void UpdateIntent(dynamic dynamic)
        {
            string Intent = $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]行动时," +
                $"若护甲低于魔甲，则将护甲补充至20，然后对随机一名敌人造成10点无属性物理伤害。" +
                $"否则，将魔甲补充至20，然后对随机一名敌人造成10点无属性魔法伤害";
            _data.ChangeIntent(Intent);
        }

        private void ActionFunc(int round, List<PlayerRole> players, List<EnemyRole> enemies, EnemyImplBase @base)
        {
            if (players.Count == 0) return;
            var rand = new Random();
            int idx = rand.Next(players.Count);
            List<BaseRole> list = new() { players[idx] };
            if (StaticUtils.TryGetBattleScene() is BattleScene bs)
            {
                if (@base.Binder.CurrPBlock < @base.Binder.CurrMBlock)
                {
                    @base.Binder.CurrPBlock = 20;
                    bs.DealDamage(10, StaticEnums.AttackType.Physics, @base.Binder, list);
                }
                else
                {
                    @base.Binder.CurrMBlock = 20;
                    bs.DealDamage(10, StaticEnums.AttackType.Magic, @base.Binder, list);
                }
            }
        }
    }
}
