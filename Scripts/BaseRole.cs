using StaticClass;
using System;
using System.Collections.Generic;

namespace KemoCard.Scripts
{
    [Serializable]
    public partial class BaseRole : IEvent
    {
        public BaseRole()
        {
        }
        public BaseRole(double speed, double strength, double effeciency, double mantra, double craftequip, double craftbook, double critical, double dodge, int CurrHealth = 0, int CurrMagic = 0, int OriginHpLimit = 0, int OriginMpLimit = 0)
        {
            int oldCurrHpLimit = CurrHpLimit, oldCurrMpLimit = CurrMpLimit;
            OriginSpeed = speed; OriginStrength = strength; OriginEffeciency = effeciency; OriginMantra = mantra; OriginCraftBook = craftbook; OriginCritical = critical; OriginDodge = dodge; OriginCraftEquip = craftequip;
            UpdateHpAndMp();
            this.CurrHealth = CurrHealth > 0 ? CurrHealth : CurrHpLimit;
            this.CurrMagic = CurrMagic > 0 ? CurrMagic : CurrMpLimit;
            StaticInstance.eventMgr.RegistIEvent(this);
        }

        private void UpdateHpAndMp()
        {
            OriginHpLimit = (int)(Body * 3 + CurrStrength * 3);
            OriginMpLimit = (int)(MagicAbility * 3 + CurrMantra * 3);
        }

        ~BaseRole()
        {
            StaticInstance.eventMgr.UnregistIEvent(this);
        }

        public string name = "未命名";

        #region Properties
        // 固定数据区
        private double _OriginSpeed = 0;
        private double _OriginStrength = 0;
        private double _OriginEffeciency = 0;
        private double _OriginMantra = 0;
        private double _OriginCraftEquip = 0;
        private double _OriginCraftBook = 0;
        private double _OriginCritical = 0;
        private double _OriginDodge = 0;
        private int _OriginHpLimit = 0;
        private int _OriginMpLimit = 0;
        private double _OriginMBlock = 0;
        private double _OriginPBlock = 0;

        public double OriginSpeed
        {
            get => _OriginSpeed;
            set
            {
                var oldValue = _OriginSpeed; _OriginSpeed = value; 
                UpdateHpAndMp();
                OnPropertyChanged(nameof(OriginSpeed), oldValue, _OriginSpeed);
            }
        }
        public double OriginStrength
        {
            get => _OriginStrength;
            set
            {
                var oldValue = _OriginStrength; _OriginStrength = value; 
                UpdateHpAndMp();
                OnPropertyChanged(nameof(OriginStrength), oldValue, _OriginStrength);
            }
        }
        public double OriginEffeciency
        {
            get => _OriginEffeciency;
            set
            {
                var oldValue = _OriginEffeciency; _OriginEffeciency = value; 
                UpdateHpAndMp();
                OnPropertyChanged(nameof(OriginEffeciency), oldValue, _OriginEffeciency);
            }
        }
        public double OriginMantra
        {
            get => _OriginMantra;
            set
            {
                var oldValue = _OriginMantra; _OriginMantra = value;
                UpdateHpAndMp();
                OnPropertyChanged(nameof(OriginMantra), oldValue, _OriginMantra);
            }
        }
        public double OriginCraftEquip
        {
            get => _OriginCraftEquip;
            set
            {
                var oldValue = _OriginCraftEquip; _OriginCraftEquip = value; OnPropertyChanged(nameof(OriginCraftEquip), oldValue, _OriginCraftEquip);
            }
        }
        public double OriginCraftBook
        {
            get => _OriginCraftBook;
            set
            {
                var oldValue = _OriginCraftBook; _OriginCraftBook = value; OnPropertyChanged(nameof(OriginCraftBook), oldValue, _OriginCraftBook);
            }
        }
        public double OriginCritical
        {
            get => _OriginCritical;
            set
            {
                var oldValue = _OriginCritical; _OriginCritical = value; OnPropertyChanged(nameof(OriginCritical), oldValue, _OriginCritical);
            }
        }
        public double OriginDodge
        {
            get => _OriginDodge;
            set
            {
                var oldValue = _OriginDodge; _OriginDodge = value; OnPropertyChanged(nameof(OriginDodge), oldValue, _OriginDodge);
            }
        }
        public int OriginHpLimit
        {
            get => _OriginHpLimit;
            set
            {
                var oldValue = _OriginHpLimit; _OriginHpLimit = value; OnPropertyChanged(nameof(OriginHpLimit), oldValue, _OriginHpLimit);
            }
        }
        public int OriginMpLimit
        {
            get => _OriginMpLimit;
            set
            {
                var oldValue = _OriginMpLimit; _OriginMpLimit = value; OnPropertyChanged(nameof(OriginMpLimit), oldValue, _OriginMpLimit);
            }
        }
        public double OriginMBlock
        {
            get => _OriginMBlock;
            set
            {
                var oldValue = _OriginMBlock; _OriginMBlock = value; OnPropertyChanged(nameof(OriginMBlock), oldValue, _OriginMBlock);
            }
        }
        public double OriginPBlock
        {
            get => _OriginPBlock;
            set
            {
                var oldValue = _OriginPBlock; _OriginPBlock = value; OnPropertyChanged(nameof(OriginPBlock), oldValue, _OriginPBlock);
            }
        }

        // 实际数据区
        private int _CurrHealth = 0;
        private int _CurrMagic = 0;
        public int CurrSpeed
        {
            get => (int)Math.Round((OriginSpeed + GetSymbol("SpeedAddProp") - GetSymbol("SpeedMinusProp")) * (1 + Math.Round(GetSymbol("SpeedAddPerm") - GetSymbol("SpeedMinusPerm"))));
        }
        public int CurrStrength
        {
            get => (int)Math.Round((OriginStrength + GetSymbol("StrengthAddProp") - GetSymbol("StrengthMinusProp")) * (1 + Math.Round(GetSymbol("StrengthAddPerm") - GetSymbol("StrengthMinusPerm"))));
        }
        public int CurrEffeciency
        {
            get => (int)Math.Round((OriginEffeciency + GetSymbol("EffeciencyAddProp") - GetSymbol("EffeciencyMinusProp")) * (1 + Math.Round(GetSymbol("EffeciencyAddPerm") - GetSymbol("EffeciencyMinusPerm"))));
        }
        public int CurrMantra
        {
            get => (int)Math.Round((OriginMantra + GetSymbol("MantraAddProp") - GetSymbol("MantraMinusProp")) * (1 + Math.Round(GetSymbol("MantraAddPerm") - GetSymbol("MantraMinusPerm"))));
        }
        public int CurrCraftEquip
        {
            get => (int)Math.Round((OriginCraftEquip + GetSymbol("CraftEquipAddProp") - GetSymbol("CraftEquipMinusProp")) * (1 + Math.Round(GetSymbol("CraftEquipAddPerm") - GetSymbol("CraftEquipMinusPerm"))));
        }
        public int CurrCraftBook
        {
            get => (int)Math.Round((OriginCraftBook + GetSymbol("CraftBookAddProp") - GetSymbol("CraftBookMinusProp")) * (1 + Math.Round(GetSymbol("CraftBookAddPerm") - GetSymbol("CraftBookMinusPerm"))));
        }
        public int CurrCritical
        {
            get => (int)Math.Round((OriginCritical + GetSymbol("CriticalAddProp") - GetSymbol("CriticalMinusProp")) * (1 + Math.Round(GetSymbol("CriticalAddPerm") - GetSymbol("CriticalMinusPerm"))));
        }
        public int CurrDodge
        {
            get => (int)Math.Round((OriginDodge + GetSymbol("DodgeAddProp") - GetSymbol("DodgeMinusProp")) * (1 + Math.Round(GetSymbol("DodgeAddPerm") - GetSymbol("DodgeMinusPerm"))));
        }
        public int CurrHpLimit
        {
            get => (int)Math.Round((OriginHpLimit + GetSymbol("HpLimitAddProp") - GetSymbol("HpLimitMinusProp")) * (1 + Math.Round(GetSymbol("HpLimitAddPerm") - GetSymbol("HpLimitMinusPerm"))));
        }
        public int CurrMpLimit
        {
            get => (int)Math.Round((OriginMpLimit + GetSymbol("MpLimitAddProp") - GetSymbol("MpLimitMinusProp")) * (1 + Math.Round(GetSymbol("MpLimitAddPerm") - GetSymbol("MpLimitMinusPerm"))));
        }
        public int CurrHealth
        {
            get => _CurrHealth;
            set
            {
                var oldValue = _CurrHealth;
                if (value > CurrHpLimit)
                {
                    _CurrHealth = CurrHpLimit;
                }
                else if (value < 0)
                {
                    _CurrHealth = 0;
                }
                else
                {
                    _CurrHealth = value;
                }
                OnPropertyChanged("CurrHealth", oldValue, _CurrHealth);
            }
        }

        public int CurrMagic
        {
            get => _CurrMagic;
            set
            {
                var oldValue = _CurrMagic;
                if (value > CurrMpLimit)
                {
                    _CurrMagic = CurrMpLimit;
                }
                else if (value < 0)
                {
                    _CurrMagic = 0;
                }
                else
                {
                    _CurrMagic = value;
                }
                OnPropertyChanged("CurrMagic", oldValue, CurrMagic);
            }
        }

        public void RecoverMagic()
        {
            CurrMagic += CurrEffeciency * 3;
        }

        public bool isFriendly = true;

        // 结合数据区，这里始终取实际数据区的值，所以是一种特殊的实际数据
        public double Body
        {
            get
            {
                return CurrSpeed + CurrStrength;
            }
        }

        public double MagicAbility
        {
            get
            {
                return CurrEffeciency + CurrMantra;
            }
        }

        public double Knowlegde
        {
            get
            {
                return CurrCraftEquip + CurrCraftBook;
            }
        }

        public double Technique
        {
            get
            {
                return CurrCritical + CurrDodge;
            }
        }

        public string GetName()
        {
            return name;
        }

        public void SetName(string value)
        {
            name = value;
        }

        public Dictionary<string, float> Symbol = new();
        public Dictionary<string, float> RunSymbol = new();
        public Dictionary<string, float> FightSymbol = new();
        public void AddSymbolValue(string key, float value, int symbolType = 1)
        {
            float oldValue = 0;
            Dictionary<string, float> s = null;
            switch (symbolType)
            {
                case 1: s = Symbol; break;
                case 2: s = RunSymbol; break;
                case 3: s = FightSymbol; break;
            }
            if (s.ContainsKey(key))
            {
                oldValue = Symbol[key];
                s[key] += value;
            }
            else s.Add(key, value);
            OnPropertyChanged(key, oldValue, value);
        }

        public virtual float GetSymbol(string key, float defaultValue = 0f)
        {
            var value = Symbol.GetValueOrDefault(key, defaultValue);
            value += RunSymbol.GetValueOrDefault(key, 0);
            value += FightSymbol.GetValueOrDefault(key, 0);
            Buffs.ForEach(buff =>
            {
                value += buff.Symbol.GetValueOrDefault(key, 0);
            });
            return value;
        }

        public string RichName() => string.Format(format: "[color={0}]{1}[/color]", arg0: StaticInstance.NameColor, arg1: GetName());

        public string RichBody() => string.Format(format: "[color={0}]{1}[/color]", arg0: StaticInstance.BodyColor, arg1: Body);

        public string RichSpeed() => string.Format(format: "[color={0}]{1}[/color]", arg0: StaticInstance.BodyColor, arg1: CurrSpeed);

        public string RichStrength() => string.Format(format: "[color={0}]{1}[/color]", arg0: StaticInstance.BodyColor, arg1: CurrStrength);

        public string RichMagic() => string.Format(format: "[color={0}]{1}[/color]", arg0: StaticInstance.MagicColor, arg1: MagicAbility);

        public string RichEfficiency() => string.Format(format: "[color={0}]{1}[/color]", arg0: StaticInstance.MagicColor, arg1: CurrEffeciency);

        public string RichMantra() => string.Format(format: "[color={0}]{1}[/color]", arg0: StaticInstance.MagicColor, arg1: CurrMantra);

        public string RichKnowledge() => string.Format(format: "[color={0}]{1}[/color]", arg0: StaticInstance.KnowlegdeColor, arg1: Knowlegde);

        public string RichCraftEquip() => string.Format(format: "[color={0}]{1}[/color]", arg0: StaticInstance.KnowlegdeColor, arg1: CurrCraftEquip);

        public string RichCraftBook() => string.Format(format: "[color={0}]{1}[/color]", arg0: StaticInstance.KnowlegdeColor, arg1: CurrCraftBook);

        public string RichTechnique() => string.Format(format: "[color={0}]{1}[/color]", arg0: StaticInstance.TechniqueColor, arg1: Technique);

        public string RichCritical() => string.Format(format: "[color={0}]{1}[/color]", arg0: StaticInstance.TechniqueColor, arg1: CurrCritical);

        public string RichDodge() => string.Format(format: "[color={0}]{1}[/color]", arg0: StaticInstance.TechniqueColor, arg1: CurrDodge);

        public virtual void ReceiveEvent(string @event, params object[] datas) { }
        #endregion

        public void OnPropertyChanged(string propName, dynamic oldValue, dynamic newValue)
        {
            object[] objects = new object[] { this, propName, oldValue, newValue };
            StaticInstance.eventMgr.Dispatch("PropertiesChanged", objects);
        }

        public List<BuffImplBase> Buffs { get; set; } = new();

        public virtual void AddBuff(BuffImplBase buff)
        {
            Buffs.Add(buff);
            StaticInstance.eventMgr.Dispatch("BeforeBuffChange", this, buff, "Add");
            buff.OnBuffAdded?.Invoke(buff);
            StaticInstance.eventMgr.Dispatch("BuffChanged", this);
        }

        public void RemoveBuff(BuffImplBase buff)
        {
            Buffs.Remove(buff);
            StaticInstance.eventMgr.Dispatch("BeforeBuffChange", this, buff, "Remove");
            buff.OnBuffRemoved?.Invoke(buff);
            StaticInstance.eventMgr.Dispatch("BuffChanged", this);
        }

        public string GetDesc()
        {
            return "";
        }

        public string GetRichDesc()
        {
            string str =
             $"名字：{RichName()}\r\n" +
             $"身体：{RichBody()}(速度：{RichSpeed()}力量：{RichStrength()})\r\n" +
             $"魔法：{RichMagic()}(效率：{RichEfficiency()}咒言：{RichMantra()})\r\n" +
             $"知识：{RichKnowledge()}(工艺：{RichCraftEquip()}书写：{RichCraftBook()})\r\n" +
             $"技术：{RichTechnique()}(会心：{RichCritical()}敏捷：{RichDodge()})\r\n";
            return str;
        }
    }
}
