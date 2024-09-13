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
    internal partial class Eskeleton : BaseEnemyScript
    {
        private EnemyImplBase _data;
        public override void OnEnemyInit(EnemyImplBase e)
        {
            _data = e;
            e.Speed = 2;
            e.Strength = 5;
            e.Critical = 17;
            e.Name = "骷髅";
            e.AnimationResourcePath = $"res://Mods/MainPackage/Resources/Animations/Bat.tres";
            e.ActionFunc = ActionFunc;
            e.AddEvent("StartBattle", UpdateIntent);
            StaticUtils.CreateBuffAndAddToRole("", e.Binder, e.Binder);
        }

        private void ActionFunc(int round, List<PlayerRole> players, List<EnemyRole> enemies, EnemyImplBase @base)
        {
            if (players.Count == 0) return;
            var rand = new Random();
            int idx = rand.Next(players.Count);
            List<BaseRole> list = new() { players[idx] };
            if (StaticUtils.TryGetBattleScene() is BattleScene bs)
            {
                bs.DealDamage(_data.Binder.CurrStrength + 5, StaticEnums.AttackType.Physics, _data.Binder, list, StaticEnums.AttributeEnum.DARK);
            }
        }

        private void UpdateIntent(dynamic datas)
        {
            string Intent = $"[img=30x30]res://Mods/MainPackage/Resources/Icons/icons_079.png[/img]造成" +
                    $"{5 + _data.Binder.CurrStrength}点暗属性物理伤害(" +
                    $"{StaticUtils.MakeColorString("力量", StaticInstance.BodyColor, 36)}+5)";
            _data.ChangeIntent(Intent);
        }
    }
}
