using System;
using System.Collections.Generic;

namespace KemoCard.Scripts
{
    [Serializable]
    public class BaseRole : IEvent
    {
        public BaseRole()
        {
        }

        public BaseRole(double speed, double strength, double effeciency, double mantra, double craftequip,
            double craftbook, double critical, double dodge, int currHealth = 0, int currMagic = 0,
            int originHpLimit = 0, int originMpLimit = 0)
        {
            int oldCurrHpLimit = CurrHpLimit, oldCurrMpLimit = CurrMpLimit;
            OriginSpeed = speed;
            OriginStrength = strength;
            OriginEffeciency = effeciency;
            OriginMantra = mantra;
            OriginCraftBook = craftbook;
            OriginCritical = critical;
            OriginDodge = dodge;
            OriginCraftEquip = craftequip;
            UpdateHpAndMp();
            CurrHealth = currHealth > 0 ? currHealth : CurrHpLimit;
            CurrMagic = currMagic > 0 ? currMagic : CurrMpLimit;
            StaticInstance.EventMgr.RegisterIEvent(this);
        }

        private void UpdateHpAndMp()
        {
            OriginHpLimit = (int)(Body * 3 + CurrStrength * 3);
            OriginMpLimit = (int)(MagicAbility * 3 + CurrMantra * 3);
        }

        ~BaseRole()
        {
            StaticInstance.EventMgr.UnregisterIEvent(this);
        }

        public string Name = "未命名";

        #region Properties

        // 固定数据区
        private double _originSpeed;
        private double _originStrength;
        private double _originEffeciency;
        private double _originMantra;
        private double _originCraftEquip;
        private double _originCraftBook;
        private double _originCritical;
        private double _originDodge;
        private int _originHpLimit;
        private int _originMpLimit;
        private double _originMBlock;
        private double _originPBlock;

        public double OriginSpeed
        {
            get => _originSpeed;
            set
            {
                var oldValue = _originSpeed;
                _originSpeed = value;
                UpdateHpAndMp();
                OnPropertyChanged(nameof(OriginSpeed), oldValue, _originSpeed);
            }
        }

        public double OriginStrength
        {
            get => _originStrength;
            set
            {
                var oldValue = _originStrength;
                _originStrength = value;
                UpdateHpAndMp();
                OnPropertyChanged(nameof(OriginStrength), oldValue, _originStrength);
            }
        }

        public double OriginEffeciency
        {
            get => _originEffeciency;
            set
            {
                var oldValue = _originEffeciency;
                _originEffeciency = value;
                UpdateHpAndMp();
                OnPropertyChanged(nameof(OriginEffeciency), oldValue, _originEffeciency);
            }
        }

        public double OriginMantra
        {
            get => _originMantra;
            set
            {
                var oldValue = _originMantra;
                _originMantra = value;
                UpdateHpAndMp();
                OnPropertyChanged(nameof(OriginMantra), oldValue, _originMantra);
            }
        }

        public double OriginCraftEquip
        {
            get => _originCraftEquip;
            set
            {
                var oldValue = _originCraftEquip;
                _originCraftEquip = value;
                OnPropertyChanged(nameof(OriginCraftEquip), oldValue, _originCraftEquip);
            }
        }

        public double OriginCraftBook
        {
            get => _originCraftBook;
            set
            {
                var oldValue = _originCraftBook;
                _originCraftBook = value;
                OnPropertyChanged(nameof(OriginCraftBook), oldValue, _originCraftBook);
            }
        }

        public double OriginCritical
        {
            get => _originCritical;
            set
            {
                var oldValue = _originCritical;
                _originCritical = value;
                OnPropertyChanged(nameof(OriginCritical), oldValue, _originCritical);
            }
        }

        public double OriginDodge
        {
            get => _originDodge;
            set
            {
                var oldValue = _originDodge;
                _originDodge = value;
                OnPropertyChanged(nameof(OriginDodge), oldValue, _originDodge);
            }
        }

        public int OriginHpLimit
        {
            get => _originHpLimit;
            set
            {
                var oldValue = _originHpLimit;
                _originHpLimit = value;
                OnPropertyChanged(nameof(OriginHpLimit), oldValue, _originHpLimit);
            }
        }

        public int OriginMpLimit
        {
            get => _originMpLimit;
            set
            {
                var oldValue = _originMpLimit;
                _originMpLimit = value;
                OnPropertyChanged(nameof(OriginMpLimit), oldValue, _originMpLimit);
            }
        }

        public double OriginMBlock
        {
            get => _originMBlock;
            set
            {
                var oldValue = _originMBlock;
                _originMBlock = value;
                OnPropertyChanged(nameof(OriginMBlock), oldValue, _originMBlock);
            }
        }

        public double OriginPBlock
        {
            get => _originPBlock;
            set
            {
                var oldValue = _originPBlock;
                _originPBlock = value;
                OnPropertyChanged(nameof(OriginPBlock), oldValue, _originPBlock);
            }
        }

        // 实际数据区
        private int _currHealth;
        private int _currMagic;

        public int CurrSpeed =>
            (int)Math.Round((OriginSpeed + GetSymbol("SpeedAddProp") - GetSymbol("SpeedMinusProp")) *
                            (1 + Math.Round(GetSymbol("SpeedAddPerm") - GetSymbol("SpeedMinusPerm"))));

        public int CurrStrength =>
            (int)Math.Round((OriginStrength + GetSymbol("StrengthAddProp") - GetSymbol("StrengthMinusProp")) *
                            (1 + Math.Round(GetSymbol("StrengthAddPerm") - GetSymbol("StrengthMinusPerm"))));

        public int CurrEffeciency =>
            (int)Math.Round(
                (OriginEffeciency + GetSymbol("EffeciencyAddProp") - GetSymbol("EffeciencyMinusProp")) *
                (1 + Math.Round(GetSymbol("EffeciencyAddPerm") - GetSymbol("EffeciencyMinusPerm"))));

        public int CurrMantra =>
            (int)Math.Round((OriginMantra + GetSymbol("MantraAddProp") - GetSymbol("MantraMinusProp")) *
                            (1 + Math.Round(GetSymbol("MantraAddPerm") - GetSymbol("MantraMinusPerm"))));

        public int CurrCraftEquip =>
            (int)Math.Round(
                (OriginCraftEquip + GetSymbol("CraftEquipAddProp") - GetSymbol("CraftEquipMinusProp")) *
                (1 + Math.Round(GetSymbol("CraftEquipAddPerm") - GetSymbol("CraftEquipMinusPerm"))));

        public int CurrCraftBook =>
            (int)Math.Round((OriginCraftBook + GetSymbol("CraftBookAddProp") - GetSymbol("CraftBookMinusProp")) *
                            (1 + Math.Round(GetSymbol("CraftBookAddPerm") - GetSymbol("CraftBookMinusPerm"))));

        public int CurrCritical =>
            (int)Math.Round((OriginCritical + GetSymbol("CriticalAddProp") - GetSymbol("CriticalMinusProp")) *
                            (1 + Math.Round(GetSymbol("CriticalAddPerm") - GetSymbol("CriticalMinusPerm"))));

        public int CurrDodge =>
            (int)Math.Round((OriginDodge + GetSymbol("DodgeAddProp") - GetSymbol("DodgeMinusProp")) *
                            (1 + Math.Round(GetSymbol("DodgeAddPerm") - GetSymbol("DodgeMinusPerm"))));

        public int CurrHpLimit =>
            (int)Math.Round((OriginHpLimit + GetSymbol("HpLimitAddProp") - GetSymbol("HpLimitMinusProp")) *
                            (1 + Math.Round(GetSymbol("HpLimitAddPerm") - GetSymbol("HpLimitMinusPerm"))));

        public int CurrMpLimit =>
            (int)Math.Round((OriginMpLimit + GetSymbol("MpLimitAddProp") - GetSymbol("MpLimitMinusProp")) *
                            (1 + Math.Round(GetSymbol("MpLimitAddPerm") - GetSymbol("MpLimitMinusPerm"))));

        public int CurrHealth
        {
            get => _currHealth;
            set
            {
                var oldValue = _currHealth;
                if (value > CurrHpLimit)
                {
                    _currHealth = CurrHpLimit;
                }
                else if (value < 0)
                {
                    _currHealth = 0;
                }
                else
                {
                    _currHealth = value;
                }

                OnPropertyChanged("CurrHealth", oldValue, _currHealth);
            }
        }

        public int CurrMagic
        {
            get => _currMagic;
            set
            {
                var oldValue = _currMagic;
                if (value > CurrMpLimit)
                {
                    _currMagic = CurrMpLimit;
                }
                else if (value < 0)
                {
                    _currMagic = 0;
                }
                else
                {
                    _currMagic = value;
                }

                OnPropertyChanged("CurrMagic", oldValue, CurrMagic);
            }
        }

        public void RecoverMagic()
        {
            CurrMagic += CurrEffeciency * 3;
        }

        public bool IsFriendly = true;

        // 结合数据区，这里始终取实际数据区的值，所以是一种特殊的实际数据
        public double Body => CurrSpeed + CurrStrength;

        public double MagicAbility => CurrEffeciency + CurrMantra;

        public double Knowlegde => CurrCraftEquip + CurrCraftBook;

        public double Technique => CurrCritical + CurrDodge;

        public string GetName()
        {
            return Name;
        }

        public void SetName(string value)
        {
            Name = value;
        }

        public Dictionary<string, float> Symbol = new();
        public Dictionary<string, float> RunSymbol = new();
        public Dictionary<string, float> FightSymbol = new();

        public void AddSymbolValue(string key, float value, int symbolType = 1)
        {
            float oldValue = 0;
            var s = symbolType switch
            {
                1 => Symbol,
                2 => RunSymbol,
                3 => FightSymbol,
                _ => Symbol
            };

            if (!s.TryAdd(key, value))
            {
                oldValue = Symbol[key];
                s[key] += value;
            }

            OnPropertyChanged(key, oldValue, value);
        }

        public virtual float GetSymbol(string key, float defaultValue = 0f)
        {
            var value = Symbol.GetValueOrDefault(key, defaultValue);
            value += RunSymbol.GetValueOrDefault(key, 0);
            value += FightSymbol.GetValueOrDefault(key, 0);
            Buffs.ForEach(buff => { value += buff.Symbol.GetValueOrDefault(key, 0); });
            return value;
        }

        public string RichName() => $"[color={StaticInstance.NameColor}]{GetName()}[/color]";

        public string RichBody() =>
            $"[color={StaticInstance.BodyColor}]{Body}[/color]";

        public string RichSpeed() => $"[color={StaticInstance.BodyColor}]{CurrSpeed}[/color]";

        public string RichStrength() => $"[color={StaticInstance.BodyColor}]{CurrStrength}[/color]";

        public string RichMagic() => $"[color={StaticInstance.MagicColor}]{MagicAbility}[/color]";

        public string RichEfficiency() => $"[color={StaticInstance.MagicColor}]{CurrEffeciency}[/color]";

        public string RichMantra() => $"[color={StaticInstance.MagicColor}]{CurrMantra}[/color]";

        public string RichKnowledge() => $"[color={StaticInstance.KnowledgeColor}]{Knowlegde}[/color]";

        public string RichCraftEquip() => $"[color={StaticInstance.KnowledgeColor}]{CurrCraftEquip}[/color]";

        public string RichCraftBook() => $"[color={StaticInstance.KnowledgeColor}]{CurrCraftBook}[/color]";

        public string RichTechnique() => $"[color={StaticInstance.TechniqueColor}]{Technique}[/color]";

        public string RichCritical() => $"[color={StaticInstance.TechniqueColor}]{CurrCritical}[/color]";

        public string RichDodge() => $"[color={StaticInstance.TechniqueColor}]{CurrDodge}[/color]";

        public virtual void ReceiveEvent(string @event, params object[] datas)
        {
        }

        #endregion

        public void OnPropertyChanged(string propName, dynamic oldValue, dynamic newValue)
        {
            object[] objects = [this, propName, oldValue, newValue];
            StaticInstance.EventMgr.Dispatch("PropertiesChanged", objects);
        }

        public List<BuffImplBase> Buffs { get; set; } = [];

        public virtual void AddBuff(BuffImplBase buff)
        {
            Buffs.Add(buff);
            StaticInstance.EventMgr.Dispatch("BeforeBuffChange", this, buff, "Add");
            buff.OnBuffAdded?.Invoke(buff);
            StaticInstance.EventMgr.Dispatch("BuffChanged", this);
        }

        public void RemoveBuff(BuffImplBase buff)
        {
            Buffs.Remove(buff);
            StaticInstance.EventMgr.Dispatch("BeforeBuffChange", this, buff, "Remove");
            buff.OnBuffRemoved?.Invoke(buff);
            StaticInstance.EventMgr.Dispatch("BuffChanged", this);
        }

        public string GetDesc()
        {
            return $"名字：{GetName()}\r\n";
        }

        public virtual string GetRichDesc()
        {
            var str =
                $"名字：{RichName()}\r\n" +
                $"身体：{RichBody()}(速度：{RichSpeed()}力量：{RichStrength()})\r\n" +
                $"魔法：{RichMagic()}(效率：{RichEfficiency()}咒言：{RichMantra()})\r\n" +
                $"知识：{RichKnowledge()}(工艺：{RichCraftEquip()}书写：{RichCraftBook()})\r\n" +
                $"技术：{RichTechnique()}(会心：{RichCritical()}敏捷：{RichDodge()})\r\n";
            return str;
        }
    }
}