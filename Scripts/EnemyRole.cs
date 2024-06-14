using Godot;
using KemoCard.Scripts.Enemies;
using StaticClass;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KemoCard.Scripts
{
    public partial class EnemyRole : BaseRole, IEvent
    {
        public EnemyImplBase script;
        public List<BuffImplBase> InFightBuffs = new();
        public EnemyRoleObject roleObject;
        public bool isEnemyInited = false;

        private int _CurrPBlock = 0;
        private int _CurrMBlock = 0;
        public int CurrPBlock
        {
            get => _CurrPBlock;
            set
            {
                if (value < 0)
                {
                    _CurrPBlock = 0;
                }
                else
                {
                    _CurrPBlock = value;
                }
                StaticInstance.eventMgr.Dispatch("PropertiesChanged", null);
            }
        }

        public int CurrMBlock
        {
            get => _CurrMBlock;
            set
            {
                if (value < 0)
                {
                    _CurrMBlock = 0;
                }
                else
                {
                    _CurrMBlock = value;
                }
                StaticInstance.eventMgr.Dispatch("PropertiesChanged", null);
            }
        }
        public EnemyRole(uint id)
        {
            if (Datas.Ins.EnemyPool.ContainsKey(id))
            {
                var data = Datas.Ins.EnemyPool[id];
                isFriendly = false;
                var res = ResourceLoader.Load<CSharpScript>($"res://Mods/{data.mod_id}/Scripts/Enemies/E" + id + ".cs");
                script = new();
                if (res != null)
                {
                    var s = res.New().As<BaseEnemyScript>();
                    s.OnEnemyInit(script);
                }
                else
                {
                    StaticInstance.MainRoot.ShowBanner("未找到敌人脚本文件。id:" + id);
                }

                if (script != null)
                {
                    OriginSpeed = script.Speed;
                    OriginStrength = script.Strength;
                    OriginEffeciency = script.Effeciency;
                    OriginMantra = script.Mantra;
                    OriginCraftBook = script.CraftBook;
                    OriginCraftEquip = script.CraftEquip;
                    OriginCritical = script.Critical;
                    OriginDodge = script.Dodge;
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
                isFriendly = false;
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

                script = new();

                string errorLog = "未找到敌人。id:" + id;
                StaticInstance.MainRoot.ShowBanner(errorLog);
                GD.PrintErr(errorLog);
            }
            name = script.Name;
            script.Binder = this;
            isEnemyInited = true;
        }

        public override void ReceiveEvent(string @event, dynamic datas)
        {
            script?.ReceiveEvent(@event, datas);
        }

        ~EnemyRole()
        {
            if (script != null) script.Binder = null;
            script = null;
            roleObject = null;
        }
    }
}
