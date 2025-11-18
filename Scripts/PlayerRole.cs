using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Godot;
using KemoCard.Pages;
using KemoCard.Scripts.Cards;
using KemoCard.Scripts.Roles;
using static KemoCard.Scripts.StaticEnums;

namespace KemoCard.Scripts
{
    [Serializable]
    public class PlayerRole : BaseRole
    {
        public PlayerRole(double OriginSpeed, double OriginStrength, double OriginEffeciency, double OriginMantra,
            double OriginCraftEquip, double OriginCraftBook, double OriginCritical, double OriginDodge,
            string name = "", int CurrHealth = 0, int CurrMagic = 0, int OriginHpLimit = 0, int OriginMpLimit = 0,
            int ActionPoint = 0) : base(OriginSpeed, OriginStrength, OriginEffeciency, OriginMantra, OriginCraftEquip,
            OriginCraftBook, OriginCritical, OriginDodge, CurrHealth, CurrMagic, OriginHpLimit, OriginMpLimit)
        {
            SetName(name);
            this.ActionPoint = ActionPoint;
        }

        public PlayerRole()
        {
        }

        public PlayerRole(string id)
        {
            if (!Datas.Ins.RolePool.TryGetValue(id, out var value))
            {
                var errorLog = "未在脚本库中找到角色对应id，请检查Mod配置。id:" + id;
                StaticInstance.MainRoot.ShowBanner(errorLog);
                GD.PrintErr(errorLog);
            }
            else
            {
                Id = value.RoleId;
                var script = RoleFactory.CreateRole(id);
                script?.OnRoleInit(this);
            }
        }

        public string Id { get; set; } = "";

        /// <summary>
        /// 在使用这个角色当预设角色开始游戏的时候才会调用这个函数
        /// </summary>
        [JsonIgnore] public Action StartFunction;

        public List<Card> Deck { get; set; } = [];

        public Dictionary<string, Dictionary<uint, Card>> DeckIdxDic = new();

        private int _gold;

        public int Gold
        {
            get => _gold;
            set
            {
                _gold = Math.Max(0, value);
                StaticInstance.EventMgr.Dispatch("GoldChanged");
            }
        }

        public void AddCardIntoDeck(Card card)
        {
            if (DeckIdxDic.TryGetValue(card.Id, out var value))
            {
                if (value != null)
                {
                    foreach (var idx in value.Keys.Where(idx => !value.ContainsKey(idx + 1)))
                    {
                        value[idx + 1] = card;
                        card.Idx = idx + 1;
                        break;
                    }
                }
                else
                {
                    _ = DeckIdxDic[card.Id] = new Dictionary<uint, Card>();
                }

                card.Owner = this;
                Deck.Add(card);
            }
            else
            {
                var dic = DeckIdxDic[card.Id] = new Dictionary<uint, Card>();
                dic[0] = card;
                card.Idx = 0;
                card.Owner = this;
                Deck.Add(card);
            }
        }

        public void RemoveCardFromDeck(string cardid, uint idx)
        {
            if (!DeckIdxDic.TryGetValue(cardid, out var dic)) return;
            if (!dic.TryGetValue(idx, out var value)) return;
            Deck.Remove(value);
            value.Owner = null;
            dic.Remove(idx);
        }

        /// <summary>
        /// 在读档的时候调用，用来构造deck_idx_dic这个变量的
        /// </summary>
        public void BuildDeckIdxDic()
        {
            foreach (var card in Deck)
            {
                if (DeckIdxDic.TryGetValue(card.Id, out var value))
                {
                    if (value != null)
                    {
                        foreach (var idx in value.Keys.Where(idx => !value.ContainsKey(idx + 1)))
                        {
                            value[idx + 1] = card;
                            card.Idx = idx + 1;
                            break;
                        }
                    }
                    else
                    {
                        _ = DeckIdxDic[card.Id] = new Dictionary<uint, Card>();
                    }

                    card.Owner = this;
                }
                else
                {
                    var dic = DeckIdxDic[card.Id] = new Dictionary<uint, Card>();
                    dic[0] = card;
                    card.Idx = 0;
                    card.Owner = this;
                }
            }
        }

        //private List<uint> DeckIds
        //{
        //    get
        //    {
        //        List<uint> res = new();
        //        deck.ForEach(card =>
        //        {
        //            if (card.equipId == 0) res.Add(card.id);
        //        });
        //        return res;
        //    }
        //    set
        //    {
        //        value.ForEach(ids =>
        //        {
        //            deck.Add(new(ids, this));
        //        });
        //    }
        //}

        private int _actionPoint;

        public int ActionPoint
        {
            get => _actionPoint;
            set
            {
                var oldValue = _actionPoint;
                _actionPoint = value;
                OnPropertyChanged(nameof(ActionPoint), oldValue, _actionPoint);
            }
        }

        public void Reset()
        {
            Deck.Clear();
        }

        public Dictionary<uint, Equip> EquipDic { get; set; } = new Dictionary<uint, Equip>();
        public Equip[] EquipList { get; set; } = new Equip[BagCount];

        public void PutOnEquip(uint bagIndex)
        {
            var equip = EquipList[bagIndex];
            if (equip == null) return;
            switch (equip.EquipType)
            {
                case EquipType.DoubleWeapon:
                {
                    var amt = GetEquipBagEmptyAmt();
                    if (EquipDic[(uint)EquipType.Weapon1] != null && EquipDic[(uint)EquipType.Weapon2] != null &&
                        EquipDic[(uint)EquipType.Weapon1].EquipType != EquipType.DoubleWeapon)
                    {
                        if (amt < 1)
                        {
                            StaticInstance.MainRoot.ShowBanner("背包已满");
                            return;
                        }

                        EquipList[bagIndex] = null;
                        PutOffEquip((uint)EquipType.Weapon1);
                        PutOffEquip((uint)EquipType.Weapon2);
                        EquipDic[(uint)EquipType.Weapon1] = EquipDic[(uint)EquipType.Weapon2] = equip;
                    }
                    else
                    {
                        EquipList[bagIndex] = null;
                        PutOffEquip((uint)EquipType.Weapon1);
                        PutOffEquip((uint)EquipType.Weapon2);
                        EquipDic[(uint)EquipType.Weapon1] = EquipDic[(uint)EquipType.Weapon2] = equip;
                    }

                    break;
                }
                case EquipType.Other:
                {
                    var tempBool = true;
                    for (var i = (uint)EquipType.Other; i < (uint)EquipType.Other + 5; i++)
                    {
                        if (EquipDic.TryGetValue(i, out var value) && value != null) continue;
                        EquipList[bagIndex] = null;
                        PutOffEquip(bagIndex);
                        if (EquipDic.TryAdd(i, equip))
                        {
                        }
                        else if (EquipDic[i] == null) EquipDic[i] = equip;

                        tempBool = false;
                        break;
                    }

                    if (tempBool)
                    {
                        StaticInstance.MainRoot.ShowBanner("无空闲饰品栏");
                        return;
                    }

                    break;
                }
                case EquipType.BothWeapon when EquipDic.ContainsKey((uint)EquipType.Weapon1):
                {
                    if (EquipDic.ContainsKey((uint)EquipType.Weapon2))
                    {
                        EquipList[bagIndex] = null;
                        PutOffEquip((uint)EquipType.Weapon2);
                    }
                    else
                    {
                        EquipList[bagIndex] = null;
                    }

                    EquipDic[(uint)EquipType.Weapon2] = equip;

                    break;
                }
                case EquipType.BothWeapon:
                    EquipList[bagIndex] = null;
                    EquipDic[(uint)EquipType.Weapon1] = equip;
                    break;
                default:
                    EquipList[bagIndex] = null;
                    PutOffEquip((uint)equip.EquipType);
                    EquipDic[(uint)equip.EquipType] = equip;
                    break;
            }

            equip.Owner = this;
            equip.EquipScript.OnPutOn();
            StaticInstance.EventMgr.Dispatch("PlayerEquipUpdated");
        }

        public void PutOffEquip(uint slot)
        {
            if (!EquipDic.TryGetValue(slot, out var value) || value == null) return;
            var amt = GetEquipBagEmptyAmt();

            if (amt > 0)
            {
                if (value.EquipType == EquipType.DoubleWeapon &&
                    slot is (uint)EquipType.Weapon1 or (uint)EquipType.Weapon2)
                {
                    AddEquipToBag(value);
                    EquipDic[(uint)EquipType.Weapon2] = null;
                    EquipDic[(uint)EquipType.Weapon1] = null;
                }
                else
                {
                    AddEquipToBag(value);
                    EquipDic.Remove(slot);
                }

                value.EquipScript.OnPutOff();
                StaticInstance.EventMgr.Dispatch("PlayerEquipUpdated");
            }
            else
            {
                StaticInstance.MainRoot.ShowBanner("背包已满");
            }
        }

        public bool AddEquipToBag(Equip equip)
        {
            var idx = GetFirstEmptyBagIndex();
            if (idx > -1)
            {
                EquipList[idx] = equip;
                StaticInstance.EventMgr.Dispatch("PlayerEquipUpdated");
                return true;
            }

            StaticInstance.MainRoot.ShowBanner("背包已满");
            return false;
        }

        public int GetFirstEmptyBagIndex()
        {
            for (var i = 0; i < EquipList.Length; i++)
            {
                if (EquipList[i] == null)
                {
                    return i;
                }
            }

            return -1;
        }

        public uint GetEquipBagEmptyAmt()
        {
            uint res = 0;
            foreach (var t in EquipList)
            {
                if (t == null)
                {
                    res++;
                }
            }

            return res;
        }

        public override float GetSymbol(string key, float defaultValue = 0f)
        {
            var value = Symbol.GetValueOrDefault(key, defaultValue);
            Buffs.ForEach(buff => { value += buff.Symbol.GetValueOrDefault(key, 0); });
            value += EquipDic.Values.Sum(equip => equip.Symbol.GetValueOrDefault(key, 0));

            return value;
        }

        [JsonIgnore] public Action OnBattleStart;

        private uint _unUsedPoints;

        public uint UnUsedPoints
        {
            get => _unUsedPoints;
            set => _unUsedPoints = Math.Max(0, value);
        }

        private uint _exp;

        public uint Exp
        {
            get => _exp;
            set
            {
                if (Level >= ExpCfg.MaxLevel)
                {
                    _exp = 0;
                    return;
                }

                _exp += value;
                var needExp = ExpCfg.CalUpgradeNeedExp(Level);
                if (_exp < needExp)
                    _exp = Math.Max(0, _exp);
                else
                {
                    while (_exp >= ExpCfg.CalUpgradeNeedExp(Level))
                    {
                        _exp -= ExpCfg.CalUpgradeNeedExp(Level);
                        UpgradeLevel();
                    }
                }
            }
        }

        private uint _level = 1;

        public uint Level
        {
            get => _level;
            set => _level = Math.Clamp(value, _level, ExpCfg.MaxLevel);
        }

        public void UpgradeLevel()
        {
            Level++;
            UnUsedPoints += ExpCfg.GainPointPerUpgrade;
        }

        private int _currPBlock;
        private int _currMBlock;

        public int CurrPBlock
        {
            get => _currPBlock;
            set
            {
                var oldValue = _currPBlock;
                _currPBlock = value < 0 ? 0 : value;

                var param = new object[] { this, "CurrPBlock", oldValue, _currPBlock };
                StaticInstance.EventMgr.Dispatch("PropertiesChanged", param);
            }
        }

        public int CurrMBlock
        {
            get => _currMBlock;
            set
            {
                var oldValue = _currMBlock;
                _currMBlock = value < 0 ? 0 : value;

                var param = new object[] { this, "CurrMBlock", oldValue, _currPBlock };
                StaticInstance.EventMgr.Dispatch("PropertiesChanged", param);
            }
        }

        public List<Card> TempDeck { get; set; } = [];

        #region 战斗中的数据，非战斗中时没有意义

        [JsonIgnore] public List<Card> InFightDeck = [];
        [JsonIgnore] public List<Card> InFightHands = [];
        [JsonIgnore] public List<Card> InFightGrave = [];
        [JsonIgnore] public List<BuffImplBase> InFightBuffs = [];
        public int HandLimit = 10;
        [JsonIgnore] public bool IsIfpInit;
        [JsonIgnore] public bool IsDead = false;
        public int InFightDeckCount => InFightDeck.Count;
        public int InFightGraveCount => InFightGrave.Count;
        [JsonIgnore] public PlayerRoleObject RoleObject;

        private int _turnActionPoint;

        public int TurnActionPoint
        {
            get => _turnActionPoint;
            set
            {
                var oldValue = _turnActionPoint;
                _turnActionPoint = value;
                OnPropertyChanged(nameof(TurnActionPoint), oldValue, _turnActionPoint);
            }
        }

        public int CurrActionPoint;

        public int CurrentActionPoint
        {
            get => CurrActionPoint;
            set
            {
                var oldValue = CurrActionPoint;
                CurrActionPoint = value;
                OnPropertyChanged(nameof(CurrentActionPoint), oldValue, CurrActionPoint);
            }
        }

        public void AddCardToDeck(Card card)
        {
            InFightDeck.Add(card);
            if (StaticInstance.CurrWindow is not BattleScene bs) return;
            if (bs.NowPlayer == this)
            {
                bs.UpdateCounts();
            }
        }

        public void AddCardToTempDeck(Card card)
        {
            TempDeck.Add(card);
        }

        public void RemoveCardInDeck(Card card)
        {
            InFightDeck.Remove(card);
            if (StaticInstance.CurrWindow is not BattleScene bs) return;
            if (bs.NowPlayer == this)
            {
                bs.UpdateCounts();
            }
        }

        public void AddCardToGrave(Card card)
        {
            InFightGrave.Add(card);
            if (StaticInstance.CurrWindow is not BattleScene bs) return;
            if (bs.NowPlayer == this)
            {
                bs.UpdateCounts();
            }
        }

        public void RemoveCardInGrave(Card card)
        {
            InFightGrave.Remove(card);
            if (StaticInstance.CurrWindow is not BattleScene bs) return;
            if (bs.NowPlayer == this)
            {
                bs.UpdateCounts();
            }
        }

        public void ShuffleGrave()
        {
            StaticUtils.ShuffleArray(InFightGrave);
        }

        public void ShuffleDeck()
        {
            StaticUtils.ShuffleArray(InFightDeck);
        }

        /// <summary>
        /// 赛局开始时调用,初始化赛局数据
        /// </summary>
        public void InitFighter()
        {
            InFightDeck = Deck.Concat(TempDeck).ToList();
            InFightDeck.ForEach(card =>
            {
                card.InGameDict.Clear();
                card.Owner = this;
            });
            StaticUtils.ShuffleArray(InFightDeck);
            InFightGrave = [];
            IsIfpInit = true;
            InitBuff();
        }

        public void EndFight()
        {
            InFightDeck?.Clear();
            InFightHands?.Clear();
            InFightGrave?.Clear();
            //InFightBuffs?.ToList().ForEach((BuffImplBase buff) => { buff.RemoveThisFromBinder(); });
            InFightBuffs?.Clear();
            IsIfpInit = false;
            CurrPBlock = CurrMBlock = 0;
        }

        public override void AddBuff(BuffImplBase buff)
        {
            if (RoleObject != null)
            {
                buff.Binder = this;
                RoleObject.AddBuff(buff);
            }

            base.AddBuff(buff);
        }

        public void InitBuff()
        {
            InFightBuffs = [];
            Buffs.ForEach(buff =>
            {
                //var obj = StaticUtils.TransExp<BuffImplBase, BuffImplBase>.Trans(buff);
                InFightBuffs.Add(buff);
            });
            InFightBuffs.ForEach(buff =>
            {
                if (RoleObject == null) return;
                buff.Binder = this;
                RoleObject.AddBuff(buff);
            });
        }

        public void AddInfightBuff(BuffImplBase buff)
        {
            InFightBuffs.Add(buff);
            if (RoleObject == null) return;
            buff.Binder = this;
            RoleObject.AddBuff(buff);
        }

        /// <summary>
        /// 只有在战斗中才有用
        /// </summary>
        /// <param name="card">添加到手牌的card数据</param>
        public void AddCardIntoInfightHand(Card card)
        {
            if (StaticInstance.WindowMgr.GetSceneByName("BattleScene") is not BattleScene bs) return;
            card.Owner = this;
            InFightHands.Add(card);
            var res = ResourceLoader.Load<PackedScene>("res://Pages/CardObject.tscn");
            if (res == null) return;
            var obj = res.Instantiate<CardObject>();
            obj.InitData(card);
            if (bs.NowPlayer == this)
            {
                bs.HandControl.AddChild(obj);
            }
        }

        #endregion

        public List<Card> GetDeck()
        {
            return Deck.Concat(TempDeck).ToList();
        }

        public override string GetRichDesc()
        {
            var str =
                $"名字：{RichName()} 等级：{Level}\r\n" +
                $"身体：{RichBody()}(速度：{RichSpeed()}力量：{RichStrength()})\r\n" +
                $"魔法：{RichMagic()}(效率：{RichEfficiency()}咒言：{RichMantra()})\r\n" +
                $"知识：{RichKnowledge()}(工艺：{RichCraftEquip()}书写：{RichCraftBook()})\r\n" +
                $"技术：{RichTechnique()}(会心：{RichCritical()}敏捷：{RichDodge()})\r\n";
            return str;
        }
    }
}