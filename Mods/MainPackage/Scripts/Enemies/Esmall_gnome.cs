using KemoCard.Scripts;
using KemoCard.Scripts.Enemies;
using StaticClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.Projection;

namespace KemoCard.Mods.MainPackage.Scripts.Enemies
{
    internal partial class Esmall_gnome : BaseEnemyScript
    {
        private EnemyImplBase _data;
        public override void OnEnemyInit(EnemyImplBase e)
        {
            _data = e;
            e.Effeciency = 10;
            e.Mantra = 5;
            e.Speed = 3;
            e.Strength = 6;
            e.CraftBook = 3;
            e.ActionFunc = ActionFunc;
            e.AddEvent("StartBattle", UpdateIntent);
        }

        private void UpdateIntent(dynamic dynamic)
        {
            string Intent = $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]造成" +
                    $"{_data.Binder.CurrEffeciency + _data.Binder.MagicAbility * 0.5:N2}点无属性魔法伤害(" +
                    $"{StaticUtils.MakeColorString("效率", StaticInstance.MagicColor, 36)}+0.5*" +
                    $"{StaticUtils.MakeColorString("魔法", StaticInstance.MagicColor, 36)})";
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
                bs.DealDamage(_data.Binder.CurrEffeciency + _data.Binder.MagicAbility * 0.5, StaticEnums.AttackType.Magic, _data.Binder, list);
            }
        }
    }
}
