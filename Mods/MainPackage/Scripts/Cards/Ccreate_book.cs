using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;
using System.Collections.Generic;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Ccreate_book : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.GlobalDict.Add("Exhaust", 1);
            c.FunctionUse = new(UseFunction);
            c.HintCardIds = new() { "create_book_attack", "create_book_armor" };
        }

        private void UseFunction(BaseRole user, List<BaseRole> targets, params object[] datas)
        {
            Card c1 = new("create_book_attack");
            Card c2 = new("create_book_armor");
            List<Card> cs = new() { c1, c2 };
            var bs = StaticUtils.TryGetBattleScene();
            if (bs != null)
            {
                PackedScene res = ResourceLoader.Load<PackedScene>("res://Pages/ChoseCardScene.tscn");
                if (res != null)
                {
                    ChoseCardScene scene = res.Instantiate<ChoseCardScene>();
                    StaticInstance.windowMgr.AddScene(scene, cs, 1, 1);
                }
            }
        }
    }
}
