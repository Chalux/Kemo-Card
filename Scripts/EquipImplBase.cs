using Godot;
using KemoCard.Scripts.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace KemoCard.Scripts
{
    public partial class EquipImplBase : RefCounted, IEvent
    {
        public Equip Binder;
        public string Name { get; set; }
        public Dictionary<string, uint> CardDic { get; set; } = new();

        public string TextureUrl { get; set; } = "";
        [JsonIgnore]
        public Action<BaseRole> UseFilter;

        public bool DoDefaultPutOn = true;
        [JsonIgnore]
        public Action<EquipImplBase> CustomPutOn;
        public string Desc = "";
        public void OnPutOn()
        {
            if (Binder != null && DoDefaultPutOn && Binder.owner is PlayerRole pr)
            {
                int Count = 0;
                foreach (var keyValuePair in CardDic)
                {
                    Card card = new(keyValuePair.Key)
                    {
                        EquipId = Binder.Uuid,
                        EquipIdx = keyValuePair.Value,
                    };
                    pr.AddCardIntoDeck(card);
                    Count++;
                }
                GD.Print("角色" + pr.GetName() + "装备了id为" + Binder.Id + ",Uuid为" + Binder.Uuid + "的装备。新增卡牌" + Count + "张至卡组中。装备后角色的卡组数量为" + pr.Deck.Count);
            }
            CustomPutOn?.Invoke(this);
        }

        public bool DoDefaultPutOff = true;
        [JsonIgnore]
        public Action<EquipImplBase> CustomPutOff;
        public void OnPutOff()
        {
            if (Binder != null && DoDefaultPutOff && Binder.owner is PlayerRole pr)
            {
                int Count = 0;
                var tempDeck = pr.Deck.ToList();
                foreach (var card in tempDeck)
                {
                    if (card.EquipId == Binder.Uuid && CardDic.ContainsKey(card.Id) && card.EquipIdx == CardDic.GetValueOrDefault(card.Id))
                    {
                        pr.RemoveCardFromDeck(card.Id, card.Idx);
                        Count++;
                    }
                }
                GD.Print("角色" + pr.GetName() + "脱下了id为" + Binder.Id + ",Uuid为" + Binder.Uuid + "的装备。从卡组中删除卡牌" + Count + "张。脱下后角色的卡组数量为" + pr.Deck.Count);
            }
            CustomPutOff?.Invoke(this);
        }

        [JsonIgnore]
        public Action OnCreated;

        [JsonIgnore]
        public Dictionary<string, List<Action<dynamic>>> EventDic { get; set; } = new();
        public void ReceiveEvent(string @event, params object[] datas)
        {
            if (EventDic.ContainsKey(@event)) EventDic[@event]?.ForEach(function => function?.Invoke(datas));
        }

        public void AddEvent(string @event, Action<dynamic> func)
        {
            if (EventDic.ContainsKey(@event))
            {
                EventDic[@event].Add(func);
            }
            else
            {
                EventDic[@event] = new()
                {
                    func
                };
            }
        }
    }
}
