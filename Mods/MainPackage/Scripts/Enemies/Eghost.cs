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
    internal partial class Eghost : BaseEnemyScript
    {
        private EnemyImplBase _data;
        public override void OnEnemyInit(EnemyImplBase e)
        {
            _data = e;
            e.Speed = 6;
            e.Strength = 1;
            e.Dodge = 10;
            e.Effeciency = 6;
            e.Mantra = 4;
            e.Binder.CurrMBlock = 15;
            e.Name = "鬼魂";
            e.ActionFunc = ActionFunc;
            e.AddEvent("StartBattle", UpdateIntent);
            StaticUtils.CreateBuffAndAddToRole("ghost_body", e.Binder, e.Binder);
        }

        private void ActionFunc(int round, List<PlayerRole> players, List<EnemyRole> enemies, EnemyImplBase @base)
        {
            if (players.Count == 0) return;
            var rand = new Random();
            int idx = rand.Next(players.Count);
            List<BaseRole> list = new() { players[idx] };
            if (StaticUtils.TryGetBattleScene() is BattleScene bs)
            {
                bs.DealDamage(12, StaticEnums.AttackType.Magic, _data.Binder, list, StaticEnums.AttributeEnum.DARK);
            }
        }

        private void UpdateIntent(dynamic dynamic)
        {
            string Intent =
                $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]造成" +
                $"12点暗属性魔法伤害";
            _data.ChangeIntent(Intent);
        }
    }
}
