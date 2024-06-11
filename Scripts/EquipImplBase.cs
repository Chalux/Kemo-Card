using Godot;
using KeraLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace KemoCard.Scripts
{
    public partial class EquipImplBase : GodotObject, IEvent
    {
        public Equip Binder;
        public Dictionary<uint, uint> CardDic { get; set; } = new();

        public string TextureUrl { get; set; } = "";
        [JsonIgnore]
        public Action<BaseRole> UseFilter;

        public bool DoDefaultPutOn = true;
        [JsonIgnore]
        public Action CustomPutOn;
        public void OnPutOn()
        {
            if (DoDefaultPutOn && Binder.owner is PlayerRole pr)
            {
                foreach (var keyValuePair in CardDic)
                {
                    Card card = new(keyValuePair.Key)
                    {
                        EquipId = Binder.Id,
                        EquipIdx = keyValuePair.Value,
                    };
                    pr.AddCardIntoDeck(card);
                }
                GD.Print("角色" + pr.GetName() + "装备了id为" + Binder.Id + "的装备。新增卡牌" + CardDic.Count + "张至卡组中。装备后角色的卡组数量为" + pr.Deck.Count);
            }
            CustomPutOn?.Invoke();
        }

        public bool DoDefaultPutOff = true;
        [JsonIgnore]
        public Action CustomPutOff;
        public void OnPutOff()
        {
            if (DoDefaultPutOff && Binder.owner is PlayerRole pr)
            {
                var tempDeck = pr.Deck.ToList();
                foreach (var card in tempDeck)
                {
                    if (card.EquipId == Binder.Id && CardDic.ContainsKey(card.Id) && card.EquipIdx == CardDic.GetValueOrDefault(card.Id))
                        pr.RemoveCardFromDeck(card.Id, card.Idx);
                }
                GD.Print("角色" + pr.GetName() + "脱下了id为" + Binder.Id + "的装备。从卡组中删除卡牌" + CardDic.Count + "张。脱下后角色的卡组数量为" + pr.Deck.Count);
            }
            CustomPutOff?.Invoke();
        }

        [JsonIgnore]
        public Action OnCreated;

        [JsonIgnore]
        public Dictionary<string, List<LuaFunction>> EventDic { get; set; } = new();
        public void ReceiveEvent(string @event, dynamic datas)
        {
            if (EventDic.ContainsKey(@event)) EventDic[@event]?.ForEach(function => function?.Invoke(datas));
        }

        public void AddEvent(string @event, LuaFunction func)
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
