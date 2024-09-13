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
    internal partial class Egoblin : BaseEnemyScript
    {
        private EnemyImplBase _data;
        public override void OnEnemyInit(EnemyImplBase e)
        {
            _data = e;
            e.Speed = 3;
            e.Strength = 7;
            e.CraftBook = 7;
            e.CraftEquip = 7;
            e.Dodge = 0;
            e.Critical = 0;
            e.Mantra = 0;
            e.Effeciency = 0;
            e.Name = "哥布林";
            e.AnimationResourcePath = $"res://Mods/MainPackage/Resources/Animations/Bat.tres";
            e.ActionFunc = ActionFunc;
            e.AddEvent("NewTurn", UpdateIntent);
        }

        private void UpdateIntent(dynamic dynamic)
        {
            if (dynamic != null && dynamic[0] is int && _data != null)
            {
                string Intent;
                if (dynamic[0] % 2 == 0)
                {
                    Intent = $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]造成" +
                    $"9 + (哥布林与目标的工艺属性的差值)点土属性物理伤害";
                }
                else
                {
                    Intent = $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]造成" +
                    $"9 + (哥布林与目标的书写属性的差值)点土属性魔法伤害";
                }
                _data.ChangeIntent(Intent);
            }
        }

        private void ActionFunc(int round, List<PlayerRole> players, List<EnemyRole> enemies, EnemyImplBase @base)
        {
            if (players.Count == 0) return;
            Random r = new();
            int idx = r.Next(players.Count);
            var target = players[idx];
            List<BaseRole> targets = new() { target };
            if (StaticUtils.TryGetBattleScene() is BattleScene bs)
            {
                if (round % 2 == 0)
                {
                    bs.DealDamage(9 + @base.Binder.CurrCraftEquip - target.CurrCraftEquip, StaticEnums.AttackType.Physics, @base.Binder, targets, StaticEnums.AttributeEnum.EARTH);
                }
                else
                {
                    bs.DealDamage(9 + @base.Binder.CurrCraftBook - target.CurrCraftBook, StaticEnums.AttackType.Magic, @base.Binder, targets, StaticEnums.AttributeEnum.EARTH);
                }
            }
        }
    }
}
