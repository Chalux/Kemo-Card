using Godot;
using KemoCard.Scripts.Cards;
using KemoCard.Scripts.Roles;
using StaticClass;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;
using static StaticClass.StaticEnums;

namespace KemoCard.Scripts
{
    [Serializable]
    public partial class PlayerRole : BaseRole
    {
        public PlayerRole(double OriginSpeed, double OriginStrength, double OriginEffeciency, double OriginMantra, double OriginCraftEquip, double OriginCraftBook, double OriginCritical, double OriginDodge, string name = "", int CurrHealth = 0, int CurrMagic = 0, int OriginHpLimit = 0, int OriginMpLimit = 0, int ActionPoint = 0) : base(OriginSpeed, OriginStrength, OriginEffeciency, OriginMantra, OriginCraftEquip, OriginCraftBook, OriginCritical, OriginDodge, CurrHealth, CurrMagic, OriginHpLimit, OriginMpLimit)
        {
            SetName(name);
            this.ActionPoint = ActionPoint;
        }

        public PlayerRole()
        {

        }

        public PlayerRole(string id)
        {
            if (!Datas.Ins.RolePool.ContainsKey(id))
            {
                string errorLog = "未在脚本库中找到角色对应id，请检查Mod配置。id:" + id;
                StaticInstance.MainRoot.ShowBanner(errorLog);
                GD.PrintErr(errorLog);
            }
            else
            {
                var cfg = Datas.Ins.RolePool[id];
                Id = cfg.role_id;
                string path = $"res://Mods/{cfg.mod_id}/role/R{id}.cs";
                var res = FileAccess.Open(path, FileAccess.ModeFlags.Read);
                if (res == null)
                {
                    string errorLog = "未找到角色脚本资源,id:" + id;
                    StaticInstance.MainRoot.ShowBanner(errorLog);
                    GD.PrintErr(errorLog);
                }
                else
                {
                    var s = ResourceLoader.Load<CSharpScript>(path).New().As<BaseRoleScript>();
                    s.OnRoleInit(this);
                }
            }
        }

        public string Id { get; set; } = "";

        [JsonIgnore]
        /// <summary>
        /// 在使用这个角色当预设角色开始游戏的时候才会调用这个函数
        /// </summary>
        public Action StartFunction;

        public List<Card> Deck { get; set; } = new();

        public Dictionary<string, Dictionary<uint, Card>> deck_idx_dic = new();

        private int _Gold = 0;
        public int Gold
        {
            get => _Gold; set
            {
                _Gold = Math.Max(0, value);
                StaticInstance.eventMgr.Dispatch("GoldChanged");
            }
        }

        public void AddCardIntoDeck(Card card)
        {
            if (deck_idx_dic.ContainsKey(card.Id))
            {
                var dic = deck_idx_dic[card.Id];
                if (dic != null)
                {
                    foreach (var idx in dic.Keys)
                    {
                        if (!dic.ContainsKey(idx + 1))
                        {
                            dic[idx + 1] = card;
                            card.Idx = idx + 1;
                            break;
                        }
                    }
                }
                else
                {
                    _ = deck_idx_dic[card.Id] = new();
                }
                card.owner = this;
                Deck.Add(card);
            }
            else
            {
                var dic = deck_idx_dic[card.Id] = new();
                dic[0] = card;
                card.Idx = 0;
                card.owner = this;
                Deck.Add(card);
            }
        }

        public void RemoveCardFromDeck(string cardid, uint idx)
        {
            if (deck_idx_dic.ContainsKey(cardid))
            {
                var dic = deck_idx_dic[cardid];
                if (dic.ContainsKey(idx))
                {
                    Deck.Remove(dic[idx]);
                    dic[idx].owner = null;
                    dic.Remove(idx);
                }
            }
        }

        /// <summary>
        /// 在读档的时候调用，用来构造deck_idx_dic这个变量的
        /// </summary>
        public void BuildDeckIdxDic()
        {
            foreach (var card in Deck)
            {
                if (deck_idx_dic.ContainsKey(card.Id))
                {
                    var dic = deck_idx_dic[card.Id];
                    if (dic != null)
                    {
                        foreach (var idx in dic.Keys)
                        {
                            if (!dic.ContainsKey(idx + 1))
                            {
                                dic[idx + 1] = card;
                                card.Idx = idx + 1;
                                break;
                            }
                        }
                    }
                    else
                    {
                        _ = deck_idx_dic[card.Id] = new();
                    }
                    card.owner = this;
                }
                else
                {
                    var dic = deck_idx_dic[card.Id] = new();
                    dic[0] = card;
                    card.Idx = 0;
                    card.owner = this;
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

        private int _actionPoint = 0;
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
            Equip equip = EquipList[bagIndex];
            if (equip == null) return;
            if (equip.EquipType == EquipType.DOUBLE_WEAPON)
            {
                uint amt = GetEquipBagEmptyAmt();
                if (EquipDic[(uint)EquipType.WEAPON1] != null && EquipDic[(uint)EquipType.WEAPON2] != null && EquipDic[(uint)EquipType.WEAPON1].EquipType != EquipType.DOUBLE_WEAPON)
                {
                    if (amt < 1)
                    {
                        StaticInstance.MainRoot.ShowBanner("背包已满");
                        return;
                    }
                    else
                    {
                        EquipList[bagIndex] = null;
                        PutOffEquip((uint)EquipType.WEAPON1);
                        PutOffEquip((uint)EquipType.WEAPON2);
                        EquipDic[(uint)EquipType.WEAPON1] = EquipDic[(uint)EquipType.WEAPON2] = equip;
                    }
                }
                else
                {
                    EquipList[bagIndex] = null;
                    PutOffEquip((uint)EquipType.WEAPON1);
                    PutOffEquip((uint)EquipType.WEAPON2);
                    EquipDic[(uint)EquipType.WEAPON1] = EquipDic[(uint)EquipType.WEAPON2] = equip;
                }
            }
            else if (equip.EquipType == EquipType.OTHER)
            {
                bool tempbool = true;
                for (uint i = (uint)EquipType.OTHER; i < (uint)EquipType.OTHER + 5; i++)
                {
                    if (!EquipDic.ContainsKey(i) || EquipDic[i] == null)
                    {
                        EquipList[bagIndex] = null;
                        PutOffEquip(bagIndex);
                        if (!EquipDic.ContainsKey(i)) EquipDic.Add(i, equip);
                        else if (EquipDic[i] == null) EquipDic[i] = equip;
                        tempbool = false;
                        break;
                    }
                }
                if (tempbool)
                {
                    StaticInstance.MainRoot.ShowBanner("无空闲饰品栏");
                    return;
                }
            }
            else if (equip.EquipType == EquipType.BOTH_WEAPON)
            {
                if (EquipDic.ContainsKey((uint)EquipType.WEAPON1))
                {
                    if (EquipDic.ContainsKey((uint)EquipType.WEAPON2))
                    {
                        EquipList[bagIndex] = null;
                        PutOffEquip((uint)EquipType.WEAPON2);
                        EquipDic[(uint)EquipType.WEAPON2] = equip;
                    }
                    else
                    {
                        EquipList[bagIndex] = null;
                        EquipDic[(uint)EquipType.WEAPON2] = equip;
                    }
                }
                else
                {
                    EquipList[bagIndex] = null;
                    EquipDic[(uint)EquipType.WEAPON1] = equip;
                }
            }
            else
            {
                EquipList[bagIndex] = null;
                PutOffEquip((uint)equip.EquipType);
                EquipDic[(uint)equip.EquipType] = equip;
            }
            equip.owner = this;
            equip.EquipScript.OnPutOn();
            StaticInstance.eventMgr.Dispatch("PlayerEquipUpdated");
        }

        public void PutOffEquip(uint slot)
        {
            if (EquipDic.ContainsKey(slot) && EquipDic[slot] != null)
            {
                uint amt = GetEquipBagEmptyAmt();

                if (amt > 0)
                {
                    Equip equip = EquipDic[slot];
                    if (equip.EquipType == EquipType.DOUBLE_WEAPON && (slot == (uint)EquipType.WEAPON1 || slot == (uint)EquipType.WEAPON2))
                    {
                        AddEquipToBag(equip);
                        EquipDic[(uint)EquipType.WEAPON2] = null;
                        EquipDic[(uint)EquipType.WEAPON1] = null;
                    }
                    else
                    {
                        AddEquipToBag(equip);
                        EquipDic.Remove(slot);
                    }
                    equip.EquipScript.OnPutOff();
                    StaticInstance.eventMgr.Dispatch("PlayerEquipUpdated");
                }
                else
                {
                    StaticInstance.MainRoot.ShowBanner("背包已满");
                }
            }
        }

        public bool AddEquipToBag(Equip equip)
        {
            var idx = GetFirstEmptyBagIndex();
            if (idx > -1)
            {
                EquipList[idx] = equip;
                StaticInstance.eventMgr.Dispatch("PlayerEquipUpdated");
                return true;
            }
            else
            {
                StaticInstance.MainRoot.ShowBanner("背包已满");
                return false;
            }
        }

        public int GetFirstEmptyBagIndex()
        {
            for (int i = 0; i < EquipList.Length; i++)
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
            for (int i = 0; i < EquipList.Length; i++)
            {
                if (EquipList[i] == null)
                {
                    res++;
                }
            }
            return res;
        }

        public override float GetSymbol(string key, float defaultValue = 0f)
        {
            var value = Symbol.GetValueOrDefault(key, defaultValue);
            Buffs.ForEach(buff =>
            {
                value += buff.Symbol.GetValueOrDefault(key, 0);
            });
            foreach (var equip in EquipDic.Values)
            {
                value += equip.Symbol.GetValueOrDefault(key, 0);
            }
            return value;
        }

        [JsonIgnore]
        public Action OnBattleStart;

        private uint _unUsedPoints = 0;
        public uint UnUsedPoints { get => _unUsedPoints; set => _unUsedPoints = Math.Max(0, value); }
        private uint _exp = 0;
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
                var NeedExp = ExpCfg.CalUpgradeNeedExp(Level);
                if (_exp < NeedExp)
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
            set
            {
                _level = Math.Clamp(value, _level, ExpCfg.MaxLevel);
            }
        }

        public void UpgradeLevel()
        {
            Level++;
            UnUsedPoints += ExpCfg.GainPointPerUpgrade;
        }

        private int _CurrPBlock = 0;
        private int _CurrMBlock = 0;

        public int CurrPBlock
        {
            get => _CurrPBlock;
            set
            {
                var oldValue = _CurrPBlock;
                if (value < 0)
                {
                    _CurrPBlock = 0;
                }
                else
                {
                    _CurrPBlock = value;
                }
                object[] param = new object[] { this, "CurrPBlock", oldValue, _CurrPBlock };
                StaticInstance.eventMgr.Dispatch("PropertiesChanged", param);
            }
        }

        public int CurrMBlock
        {
            get => _CurrMBlock;
            set
            {
                var oldValue = _CurrMBlock;
                if (value < 0)
                {
                    _CurrMBlock = 0;
                }
                else
                {
                    _CurrMBlock = value;
                }
                object[] param = new object[] { this, "CurrMBlock", oldValue, _CurrPBlock };
                StaticInstance.eventMgr.Dispatch("PropertiesChanged", param);
            }
        }
        public List<Card> TempDeck { get; set; } = new();
        #region 战斗中的数据，非战斗中时没有意义
        [JsonIgnore]
        public List<Card> InFightDeck = new();
        [JsonIgnore]
        public List<Card> InFightHands = new();
        [JsonIgnore]
        public List<Card> InFightGrave = new();
        [JsonIgnore]
        public List<BuffImplBase> InFightBuffs = new();
        public int HandLimit = 10;
        [JsonIgnore]
        public bool isIFPInited = false;
        [JsonIgnore]
        public bool isDead = false;
        public int InFightDeckCount => InFightDeck.Count;
        public int InFightGraveCount => InFightGrave.Count;
        [JsonIgnore]
        public PlayerRoleObject roleObject;

        public int _turnActionPoint = 0;
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

        public int _currActionPoint = 0;
        public int CurrentActionPoint
        {
            get => _currActionPoint;
            set
            {
                var oldValue = _currActionPoint;
                _currActionPoint = value;
                OnPropertyChanged(nameof(CurrentActionPoint), oldValue, _currActionPoint);
            }
        }

        public void AddCardToDeck(Card card)
        {
            InFightDeck.Add(card);
            if (StaticInstance.currWindow is BattleScene bs)
            {
                if (bs.nowPlayer == this)
                {
                    bs.UpdateCounts();
                }
            }
        }

        public void AddCardToTempDeck(Card card)
        {
            TempDeck.Add(card);
        }

        public void RemoveCardInDeck(Card card)
        {
            InFightDeck.Remove(card);
            if (StaticInstance.currWindow is BattleScene bs)
            {
                if (bs.nowPlayer == this)
                {
                    bs.UpdateCounts();
                }
            }
        }

        public void AddCardToGrave(Card card)
        {
            InFightGrave.Add(card);
            if (StaticInstance.currWindow is BattleScene bs)
            {
                if (bs.nowPlayer == this)
                {
                    bs.UpdateCounts();
                }
            }
        }

        public void RemoveCardInGrave(Card card)
        {
            InFightGrave.Remove(card);
            if (StaticInstance.currWindow is BattleScene bs)
            {
                if (bs.nowPlayer == this)
                {
                    bs.UpdateCounts();
                }
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
                card.owner = this;
            });
            StaticUtils.ShuffleArray(InFightDeck);
            InFightGrave = new();
            isIFPInited = true;
            InitBuff();
        }

        public void EndFight()
        {
            InFightDeck?.Clear();
            InFightHands?.Clear();
            InFightGrave?.Clear();
            //InFightBuffs?.ToList().ForEach((BuffImplBase buff) => { buff.RemoveThisFromBinder(); });
            InFightBuffs?.Clear();
            isIFPInited = false;
            CurrPBlock = CurrMBlock = 0;
        }

        public override void AddBuff(BuffImplBase buff)
        {
            if (roleObject != null)
            {
                buff.Binder = this;
                roleObject.AddBuff(buff);
            }
            base.AddBuff(buff);
        }

        public void InitBuff()
        {
            InFightBuffs = new();
            Buffs.ForEach(buff =>
            {
                //var obj = StaticUtils.TransExp<BuffImplBase, BuffImplBase>.Trans(buff);
                InFightBuffs.Add(buff);
            });
            InFightBuffs.ForEach(buff =>
            {
                if (roleObject != null)
                {
                    buff.Binder = this;
                    roleObject.AddBuff(buff);
                }
            });
        }

        public void AddInfightBuff(BuffImplBase buff)
        {
            InFightBuffs.Add(buff);
            if (roleObject != null)
            {
                buff.Binder = this;
                roleObject.AddBuff(buff);
            }
        }

        /// <summary>
        /// 只有在战斗中才有用
        /// </summary>
        /// <param name="card">添加到手牌的card数据</param>
        public void AddCardIntoInfightHand(Card card)
        {
            if (StaticInstance.windowMgr.GetSceneByName("BattleScene") is not BattleScene bs) return;
            card.owner = this;
            InFightHands.Add(card);
            PackedScene res = ResourceLoader.Load<PackedScene>("res://Pages/CardObject.tscn");
            if (res != null)
            {
                var obj = res.Instantiate<CardObject>();
                obj.InitData(card);
                if (bs.nowPlayer == this)
                {
                    bs.HandControl.AddChild(obj);
                }
            }
        }
        #endregion

        public List<Card> GetDeck()
        {
            return Deck.Concat(TempDeck).ToList();
        }

        public override string GetRichDesc()
        {
            string str =
             $"名字：{RichName()} 等级：{Level}\r\n" +
             $"身体：{RichBody()}(速度：{RichSpeed()}力量：{RichStrength()})\r\n" +
             $"魔法：{RichMagic()}(效率：{RichEfficiency()}咒言：{RichMantra()})\r\n" +
             $"知识：{RichKnowledge()}(工艺：{RichCraftEquip()}书写：{RichCraftBook()})\r\n" +
             $"技术：{RichTechnique()}(会心：{RichCritical()}敏捷：{RichDodge()})\r\n";
            return str;
        }
    }
}
