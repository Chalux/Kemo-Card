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
    internal partial class Eslime : BaseEnemyScript
    {
        private EnemyImplBase _data;
        public override void OnEnemyInit(EnemyImplBase e)
        {
            _data = e;
            e.Speed = 1;
            e.Strength = 15;
            e.Mantra = 8;
            e.Name = "史莱姆";
            e.AnimationResourcePath = $"res://Mods/MainPackage/Resources/Animations/Bat.tres";
            e.ActionFunc = ActionFunc;
            e.AddEvent("StartBattle", UpdateIntent);
        }

        private void ActionFunc(int round, List<PlayerRole> players, List<EnemyRole> enemies, EnemyImplBase @base)
        {
            if (players.Count == 0) return;
            var rand = new Random();
            int idx = rand.Next(players.Count);
            var target = players[idx];
            List<BaseRole> list = new() { target };
            if (StaticUtils.TryGetBattleScene() is BattleScene bs)
            {
                int value = @base.Binder.CurrMagic / 2;
                int diff = value - target.CurrMagic;
                if (diff > 0)
                {
                    bs.DealDamage(diff, StaticEnums.AttackType.Magic, @base.Binder, list, StaticEnums.AttributeEnum.WATER);
                    target.CurrMagic = 0;
                }
                else
                {
                    target.CurrMagic -= value;
                }
            }
        }

        private void UpdateIntent(dynamic datas)
        {
            string Intent = $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]以随机一名敌人为目标," +
                $"吸取其相当于史莱姆一半法力值上限的法力,溢出的数值将转化为水属性魔法伤害攻击目标。";
            _data.ChangeIntent(Intent);
        }
    }
}
