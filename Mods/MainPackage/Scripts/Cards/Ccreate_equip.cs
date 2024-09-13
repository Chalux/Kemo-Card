using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;
using StaticClass;
using System.Collections.Generic;

namespace KemoCard.Mods.MainPackage.Scripts.Cards
{
    internal partial class Ccreate_equip : BaseCardScript
    {
        public override void OnCardScriptInit(Card c)
        {
            c.GlobalDict.Add("Exhaust", 1);
            c.HintCardIds = new() { "create_equip_attack", "create_equip_armor" };
            c.FunctionUse = UseFunction;
        }

        private void UseFunction(BaseRole user, List<BaseRole> targets, dynamic[] datas)
        {
            Card c1 = new("create_equip_attack");
            Card c2 = new("create_equip_armor");
            List<Card> list = new() { c1, c2 };
            var bs = StaticUtils.TryGetBattleScene();
            if (bs != null)
            {
                PackedScene res = ResourceLoader.Load<PackedScene>("res://Pages/ChoseCardScene.tscn");
                if (res != null)
                {
                    ChoseCardScene scene = res.Instantiate<ChoseCardScene>();
                    StaticInstance.windowMgr.AddScene(scene, list, 1, 1);
                }
            }
        }
    }
}
