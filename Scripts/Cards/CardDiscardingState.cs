using Godot;
using StaticClass;
using static StaticClass.StaticEnums;

namespace KemoCard.Scripts.Cards
{
    partial class CardDiscardingState : CardState
    {
        public override void Enter()
        {
            //cardObject.EFRect.Visible = true;
            //BattleStatic.discard_list.Add(cardObject.card);
            BattleStatic.SelectCard(cardObject);
            StaticInstance.eventMgr.Dispatch("SelectCard");
        }

        public override void OnGUIInput(InputEvent @event)
        {
            if (@event.IsActionPressed("left_mouse"))
            {
                if (BattleStatic.isDiscarding)
                {
                    BattleStatic.SelectCard(cardObject);
                    cardObject.csm.OnTransitionRequest(this, State.BASE);
                }
            }
        }

        public override void ReceiveEvent(string @event, params object[] datas)
        {
            if (@event == "SelectConfirm")
            {
                cardObject.EFRect.Visible = false;
            }
        }
    }
}
