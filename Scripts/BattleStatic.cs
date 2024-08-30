using KemoCard.Scripts.Cards;
using StaticClass;
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

        public static void Reset()
        {
            currCard = null;
            Targets?.Clear();
            isFighting = false;
            EndSelect();
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
            StaticInstance.eventMgr.Dispatch("SelectCard");
        }

        public static void EndSelect()
        {
            discard_list?.Clear();
            isDiscarding = false;
            MustList?.Clear();
            SelectFilterFunc = null;
            ConfirmAction = null;
        }
    }
}
