using Godot;
using KeraLua;
using NLua;
using StaticClass;
using System;
using System.Collections.Generic;
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

        public PlayerRole(uint id)
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
                var res = FileAccess.Open("user://Mods/" + cfg.mod_id + "/role/R" + id + ".lua", FileAccess.ModeFlags.Read);
                if (res == null)
                {
                    string errorLog = "未找到角色脚本资源,id:" + id;
                    StaticInstance.MainRoot.ShowBanner(errorLog);
                    GD.PrintErr(errorLog);
                }
                else
                {
                    NLua.Lua templua = StaticUtils.GetOneTempLua();
                    templua["r"] = this;
                    templua.DoString(res.GetAsText());
                }
            }
        }

        /// <summary>
        /// 在创建角色的时候才会调用这个函数，在lua脚本中定义好函数内容，然后创角后会检测有没有对应的脚本，有的话就调用lua脚本写好的函数
        /// </summary>
        public NLua.LuaFunction StartFunction;

        public List<Card> Deck { get; set; } = new();

        public Dictionary<uint, Dictionary<uint, Card>> deck_idx_dic = new();

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

        public void RemoveCardFromDeck(uint cardid, uint idx)
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
            else
            {
                EquipList[bagIndex] = null;
                PutOffEquip(bagIndex);
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
                    var equip = EquipDic[slot];
                    if (equip.EquipType == EquipType.DOUBLE_WEAPON && (slot == (uint)EquipType.WEAPON1 || slot == (uint)EquipType.WEAPON2))
                    {
                        AddEquipToBag(equip);
                        EquipDic[(uint)EquipType.WEAPON2] = null;
                        EquipDic[(uint)EquipType.WEAPON1] = null;
                    }
                    else
                    {
                        AddEquipToBag(equip);
                        EquipDic[slot] = null;
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
            buffs.ForEach(buff =>
            {
                value += buff.Symbol.GetValueOrDefault(key, 0);
            });
            foreach (var equip in EquipDic.Values)
            {
                value += equip.Symbol.GetValueOrDefault(key, 0);
            }
            return value;
        }

    }
}
