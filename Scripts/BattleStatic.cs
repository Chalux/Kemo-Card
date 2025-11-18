using System;
using System.Collections.Generic;
using KemoCard.Pages;
using KemoCard.Scripts.Cards;

namespace KemoCard.Scripts
{
    internal static class BattleStatic
    {
        public static CardObject CurrCard;
        public static readonly HashSet<BaseRole> Targets = [];
        public static bool IsFighting;

        public static List<Card> DiscardList = [];
        public static bool IsDiscarding;
        public static Func<Card, bool> SelectFilterFunc;
        public static Action<List<Card>> ConfirmAction;
        public static List<int> MustList = [];
        public static int MaxSelectCount = 0;
        public static int MinSelectCount = 0;

        public static readonly List<Card> GameUsedCard = [];
        public static readonly List<Card> TurnUsedCard = [];
        public static readonly Dictionary<string, double> GameTags = new();
        public static readonly Dictionary<string, double> TurnTags = new();

        public static void Reset()
        {
            CurrCard = null;
            Targets?.Clear();
            IsFighting = false;
            EndSelect();
            GameUsedCard.Clear();
            TurnUsedCard.Clear();
            GameTags.Clear();
            TurnTags.Clear();
        }

        public static void SelectCard(CardObject obj)
        {
            if (DiscardList.IndexOf(obj.Card) > -1)
            {
                if (MustList.IndexOf(obj.GetIndex()) > -1)
                {
                    StaticInstance.MainRoot.ShowBanner("选择的卡牌必须被选中");
                }
                else
                {
                    DiscardList.Remove(obj.Card);
                    obj.EfRect.Visible = false;
                }
            }
            else
            {
                DiscardList.Add(obj.Card);
                obj.EfRect.Visible = true;
            }

            StaticInstance.EventMgr.Dispatch("SelectCard");
        }

        public static void EndSelect()
        {
            DiscardList?.Clear();
            IsDiscarding = false;
            MustList?.Clear();
            SelectFilterFunc = null;
            ConfirmAction = null;
        }

        public static void StartSelect()
        {
            DiscardList = [];
            IsDiscarding = true;
            MustList = [];
            SelectFilterFunc = null;
            ConfirmAction = null;
        }

        public static void AddUsedCard(Card card)
        {
            GameUsedCard.Add(card);
            TurnUsedCard.Add(card);
        }
    }
}