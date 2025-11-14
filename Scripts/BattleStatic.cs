using KemoCard.Scripts.Cards;
using System;
using System.Collections.Generic;

namespace KemoCard.Scripts
{
    internal class BattleStatic
    {
        public static CardObject currCard = null;
        public static HashSet<BaseRole> Targets = new();
        public static bool isFighting = false;

        public static List<Card> discard_list = new();
        public static bool isDiscarding = false;
        public static Func<Card, bool> SelectFilterFunc;
        public static Action<List<Card>> ConfirmAction;
        public static List<int> MustList = new();
        public static int MaxSelectCount = 0;
        public static int MinSelectCount = 0;

        public static List<Card> GameUsedCard = new();
        public static List<Card> TurnUsedCard = new();
        public static Dictionary<string, double> GameTags = new();
        public static Dictionary<string, double> TurnTags = new();

        public static void Reset()
        {
            currCard = null;
            Targets?.Clear();
            isFighting = false;
            EndSelect();
            GameUsedCard.Clear();
            TurnUsedCard.Clear();
            GameTags.Clear();
            TurnTags.Clear();
        }

        public static void SelectCard(CardObject obj)
        {
            if (discard_list.IndexOf(obj.card) > -1)
            {
                if (MustList.IndexOf(obj.GetIndex()) > -1)
                {
                    StaticInstance.MainRoot.ShowBanner("选择的卡牌必须被选中");
                }
                else
                {
                    discard_list.Remove(obj.card);
                    obj.EFRect.Visible = false;
                }
            }
            else
            {
                discard_list.Add(obj.card);
                obj.EFRect.Visible = true;
            }
            StaticInstance.EventMgr.Dispatch("SelectCard");
        }

        public static void EndSelect()
        {
            discard_list?.Clear();
            isDiscarding = false;
            MustList?.Clear();
            SelectFilterFunc = null;
            ConfirmAction = null;
        }

        public static void StartSelect()
        {
            discard_list = new();
            isDiscarding = true;
            MustList = new();
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
