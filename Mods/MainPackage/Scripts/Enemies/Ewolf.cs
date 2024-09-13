using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Enemies;
using StaticClass;
using System;
using System.Collections.Generic;

namespace KemoCard.Mods.MainPackage.Scripts.Enemies
{
    internal partial class Ewolf : BaseEnemyScript
    {
        private EnemyImplBase _data;
        public override void OnEnemyInit(EnemyImplBase enemy)
        {
            _data = enemy;
            enemy.Speed = 12;
            enemy.Strength = 12;
            enemy.CraftBook = 0;
            enemy.CraftEquip = 0;
            enemy.Dodge = 5;
            enemy.Critical = 7;
            enemy.Mantra = 0;
            enemy.Effeciency = 0;
            enemy.Name = "狼";
            enemy.AnimationResourcePath = $"res://Mods/MainPackage/Resources/Animations/Bat.tres";
            enemy.ActionFunc = ActionFunction;
            if (BattleStatic.isFighting)
            {
                SetAction(null);
            }
            enemy.AddEvent("NewTurn", SetAction);
            enemy.AddEvent("StartBattle", SetAction);
        }

        private void SetAction(dynamic datas)
        {
            Random r = new();
            if (!BattleStatic.GameTags.ContainsKey("wolf_call_helper") && r.Next(0, 100) < 25)
            {
                _data.InGameDict["action"] = 1;
            }
            else
            {
                if (r.Next(0, 100) < 50)
                {
                    _data.InGameDict["action"] = 2;
                }
                else
                {
                    _data.InGameDict["action"] = 3;
                }
            }
            UpdateIntent();
        }

        private void UpdateIntent()
        {
            string Intent = "";
            switch (_data.InGameDict["action"])
            {
                case 1:
                    Intent = $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_176.png[/img]召唤一匹狼加入战斗";
                    break;
                case 2:
                    Intent = $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]攻击一名随机玩家。造成{_data.Binder.CurrStrength}点无属性物理伤害(1*{StaticUtils.MakeColorString("力量", StaticInstance.BodyColor)})。如果此伤害暴击，则给目标施加一层流血效果(每回合开始时受到0.5*附加此效果时的力量点无属性物理伤害)。";
                    break;
                case 3:
                    Intent = $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]攻击一名随机玩家。造成{_data.Binder.CurrStrength}点无属性物理伤害(1*{StaticUtils.MakeColorString("力量", StaticInstance.BodyColor)})。目标每有1点护甲增加1点伤害，最多增加10点。";
                    break;
            }
            _data.ChangeIntent(Intent);
        }

        private void ActionFunction(int round, List<PlayerRole> players, List<EnemyRole> enemies, EnemyImplBase @base)
        {
            if (StaticUtils.TryGetBattleScene() is BattleScene bs)
            {
                Random r = new();
                switch (@base.InGameDict["action"])
                {
                    case 0:
                        break;
                    case 1:
                        BattleStatic.GameTags.TryAdd("wolf_call_helper", 1);
                        EnemyRole helper = new("wolf");
                        PackedScene res = ResourceLoader.Load<PackedScene>("res://Pages/EnemyRoleObject.tscn");
                        if (res != null)
                        {
                            bs.currentEnemyRoles.Add(helper);
                            EnemyRoleObject ero = res.Instantiate<EnemyRoleObject>();
                            ero.Init(helper);
                            helper.script.InGameDict["action"] = 0;
                            bs.enemyContainer.AddChild(ero);
                        }
                        break;
                    case 2:
                        List<BaseRole> targets = new()
                        {
                            players[r.Next(0, players.Count)]
                        };
                        bs.DealDamage(@base.Strength, StaticEnums.AttackType.Physics, @base.Binder, targets, StaticEnums.AttributeEnum.None, 1, 0, 0, null, CriticalAction);
                        break;
                    case 3:
                        var t = players[r.Next(0, players.Count)];
                        List<BaseRole> targets2 = new()
                        {
                            t
                        };
                        bs.DealDamage(@base.Strength + (t.CurrPBlock > 10 ? 10 : t.CurrPBlock), StaticEnums.AttackType.Physics, @base.Binder, targets2);
                        break;
                }
            }
        }

        private void CriticalAction(Damage damage, BaseRole currTarget)
        {
            BuffImplBase buff = new("bleed_wolf")
            {
                Creator = damage.from,
                BuffValue = (int)Math.Floor(damage.from.CurrStrength * 0.5)
            };
            buff.Desc = $"每回合开始时，受到{buff.BuffValue}点无属性物理伤害";
            currTarget.AddBuff(buff);
            if (StaticUtils.TryGetBattleScene() is BattleScene bs)
            {
                if (damage.from is EnemyRole wolf)
                {
                    if (bs.FindEnemyObjectByRole(wolf) is EnemyRoleObject ero)
                    {
                        ero.AddBuff(buff);
                    }
                }
            }
        }
    }
}
