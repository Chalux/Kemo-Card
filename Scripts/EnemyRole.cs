using Godot;
using KemoCard.Scripts.Enemies;
using System;
using System.Collections.Generic;

namespace KemoCard.Scripts
{
    public class EnemyRole : BaseRole
    {
        public EnemyImplBase Script;
        public List<BuffImplBase> InFightBuffs = [];
        public EnemyRoleObject RoleObject;
        public bool IsEnemyInit;

        private int _currPBlock;
        private int _currMBlock;

        public int CurrPBlock
        {
            get => _currPBlock;
            set
            {
                _currPBlock = value < 0 ? 0 : value;

                StaticInstance.EventMgr.Dispatch("PropertiesChanged", null);
            }
        }

        public int CurrMBlock
        {
            get => _currMBlock;
            set
            {
                _currMBlock = value < 0 ? 0 : value;

                StaticInstance.EventMgr.Dispatch("PropertiesChanged", null);
            }
        }

        public EnemyRole(string id)
        {
            if (Datas.Ins.EnemyPool.ContainsKey(id))
            {
                IsFriendly = false;
                Script = new EnemyImplBase
                {
                    Binder = this
                };
                var baseEnemyScript = EnemyFactory.CreateEnemy(id);
                baseEnemyScript?.OnEnemyInit(Script);

                if (Script != null)
                {
                    OriginSpeed = Script.Speed;
                    OriginStrength = Script.Strength;
                    OriginEffeciency = Script.Effeciency;
                    OriginMantra = Script.Mantra;
                    OriginCraftBook = Script.CraftBook;
                    OriginCraftEquip = Script.CraftEquip;
                    OriginCritical = Script.Critical;
                    OriginDodge = Script.Dodge;
                }
                else
                {
                    OriginSpeed = 1;
                    OriginStrength = 1;
                    OriginEffeciency = 1;
                    OriginMantra = 1;
                    OriginCraftBook = 1;
                    OriginCraftEquip = 1;
                    OriginCritical = 1;
                    OriginDodge = 1;
                }

                OriginHpLimit = (int)(Body * 3 + OriginStrength * 3);
                CurrHealth = CurrHpLimit;
                OriginMpLimit = (int)(MagicAbility * 3 + OriginMantra * 3);
                CurrMagic = CurrMpLimit;
            }
            else
            {
                IsFriendly = false;
                OriginSpeed = 1;
                OriginStrength = 1;
                OriginEffeciency = 1;
                OriginMantra = 1;
                OriginCraftBook = 1;
                OriginCraftEquip = 1;
                OriginCritical = 1;
                OriginDodge = 1;

                OriginHpLimit = (int)(Body * 3 + OriginStrength * 3);
                CurrHealth = CurrHpLimit;
                OriginMpLimit = (int)(MagicAbility * 3 + OriginMantra * 3);
                CurrMagic = CurrMpLimit;

                Script = new EnemyImplBase();

                var errorLog = $"未找到敌人。id:{id}";
                StaticInstance.MainRoot.ShowBanner(errorLog);
                GD.PrintErr(errorLog);
            }

            Name = Script.Name;
            Script.Binder = this;
            IsEnemyInit = true;
        }

        public override void ReceiveEvent(string @event, params object[] datas)
        {
            Script?.ReceiveEvent(@event, datas);
        }

        ~EnemyRole()
        {
            if (Script != null) Script.Binder = null;
            Script = null;
            RoleObject = null;
        }

        public Action<EnemyRoleObject> OnBattleStart;

        public override void AddBuff(BuffImplBase buff)
        {
            InFightBuffs.Add(buff);
            RoleObject?.AddBuff(buff);
            base.AddBuff(buff);
        }
    }
}