using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Godot;
using KemoCard.Scripts.Cards;

namespace KemoCard.Scripts
{
    public partial class EquipImplBase : RefCounted, IEvent
    {
        public Equip Binder;
        public string Name { get; set; } = "";
        public Dictionary<string, uint> CardDic { get; set; } = new();

        public string TextureUrl { get; set; } = "";
        [JsonIgnore] public Action<BaseRole> UseFilter;

        private bool _doDefaultPutOn = true;
        [JsonIgnore] public Action<EquipImplBase> CustomPutOn;
        public string Desc = "";

        public void OnPutOn()
        {
            if (Binder != null && _doDefaultPutOn && Binder.Owner is PlayerRole pr)
            {
                var count = 0;
                foreach (var keyValuePair in CardDic)
                {
                    for (uint i = 1; i <= keyValuePair.Value; i++)
                    {
                        Card card = new(keyValuePair.Key)
                        {
                            EquipId = Binder.Uuid,
                        };
                        pr.AddCardIntoDeck(card);
                        count++;
                    }
                }

                GD.Print("角色" + pr.GetName() + "装备了id为" + Binder.Id + ",Uuid为" + Binder.Uuid + "的装备。新增卡牌" + count +
                         "张至卡组中。装备后角色的卡组数量为" + pr.Deck.Count);
            }

            CustomPutOn?.Invoke(this);
        }

        public bool DoDefaultPutOff = true;
        [JsonIgnore] public Action<EquipImplBase> CustomPutOff;

        public void OnPutOff()
        {
            if (Binder != null && DoDefaultPutOff && Binder.Owner is PlayerRole pr)
            {
                var count = 0;
                var tempDeck = pr.Deck.ToList();
                foreach (var card in tempDeck.Where(card =>
                             card.EquipId == Binder.Uuid && CardDic.ContainsKey(card.Id)))
                {
                    pr.RemoveCardFromDeck(card.Id, card.Idx);
                    count++;
                }

                GD.Print("角色" + pr.GetName() + "脱下了id为" + Binder.Id + ",Uuid为" + Binder.Uuid + "的装备。从卡组中删除卡牌" + count +
                         "张。脱下后角色的卡组数量为" + pr.Deck.Count);
            }

            CustomPutOff?.Invoke(this);
        }

        [JsonIgnore] public Action OnCreated;

        [JsonIgnore] private Dictionary<string, List<Action<dynamic>>> EventDic { get; set; } = new();

        public void ReceiveEvent(string @event, params object[] datas)
        {
            if (EventDic.TryGetValue(@event, out var value)) value?.ForEach(function => function?.Invoke(datas));
        }

        public void AddEvent(string @event, Action<dynamic> func)
        {
            if (EventDic.TryGetValue(@event, out var value))
            {
                value.Add(func);
            }
            else
            {
                EventDic[@event] = [func];
            }
        }
    }
}