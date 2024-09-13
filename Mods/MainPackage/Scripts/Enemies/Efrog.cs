using KemoCard.Scripts;
using KemoCard.Scripts.Enemies;
using StaticClass;
using System;
using System.Collections.Generic;

namespace KemoCard.Mods.MainPackage.Scripts.Enemies
{
    internal partial class Efrog : BaseEnemyScript
    {
        private EnemyImplBase _data;
        public override void OnEnemyInit(EnemyImplBase enemy)
        {
            _data = enemy;
            enemy.Speed = 6;
            enemy.Strength = 3;
            enemy.CraftBook = 0;
            enemy.CraftEquip = 0;
            enemy.Dodge = 10;
            enemy.Critical = 5;
            enemy.Effeciency = 10;
            enemy.Mantra = 5;
            enemy.Name = "青蛙";
            enemy.AnimationResourcePath = $"res://Mods/MainPackage/Resources/Animations/Bat.tres";
            enemy.ActionFunc = EfrogAction;
            SetAction(null);
            enemy.AddEvent("NewTurn", SetAction);
            enemy.AddEvent("StartBattle", SetAction);
            enemy.AddEvent("PropertiesChanged", UpdateIntent);
            StaticUtils.CreateBuffAndAddToRole("water_resis", enemy.Binder, enemy.Binder);
        }

        private void SetAction(dynamic datas)
        {
            Random r = new();
            if (r.Next(0, 10) < 5)
            {
                _data.InGameDict["action"] = 1;
            }
            else
            {
                _data.InGameDict["action"] = 2;
            }
            UpdateIntent();
        }

        private void UpdateIntent(dynamic datas = null)
        {
            if (!_data.InGameDict.ContainsKey("action")) return;
            string Intent = "";
            switch (_data.InGameDict["action"])
            {
                case 1:
                    Intent =
                    $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]造成" +
                    $"{_data.Binder.CurrEffeciency + _data.Binder.MagicAbility * 0.5:N2}点无属性魔法伤害(" +
                    $"{StaticUtils.MakeColorString("效率", StaticInstance.MagicColor, 36)}+0.5*" +
                    $"{StaticUtils.MakeColorString("魔法", StaticInstance.MagicColor, 36)})";
                    _data.ChangeIntent(Intent);
                    break;
                case 2:
                    Intent =
                    $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]消耗一半的当前法力值，造成" +
                    $"等量无属性魔法伤害({_data.Binder.CurrMagic * 0.5:N2}," +
                    $"0.5*{StaticUtils.MakeColorString("法力值", StaticInstance.MagicColor, 36)})";
                    _data.ChangeIntent($"");
                    break;
            }
            _data.ChangeIntent(Intent);
        }

        private void EfrogAction(int round, List<PlayerRole> players, List<EnemyRole> enemies, EnemyImplBase @base)
        {
            if (!@base.InGameDict.ContainsKey("action") || players.Count == 0) return;
            if (StaticUtils.TryGetBattleScene() is BattleScene bs)
            {
                if (players.Count == 0) return;
                Random r = new();
                List<BaseRole> targets = new() { players[r.Next(players.Count)] };
                switch (@base.InGameDict["action"])
                {
                    case 1:
                        bs.DealDamage(_data.Binder.CurrEffeciency + _data.Binder.MagicAbility * 0.5, StaticEnums.AttackType.Magic, _data.Binder, targets);
                        break;
                    case 2:
                        bs.DealDamage(_data.Binder.CurrMagic * 0.5, StaticEnums.AttackType.Magic, _data.Binder, targets);
                        _data.Binder.CurrMagic /= 2;
                        break;
                }
            }
        }
    }
}
