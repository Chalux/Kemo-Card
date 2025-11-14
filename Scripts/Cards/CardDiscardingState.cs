using Godot;
using static KemoCard.Scripts.StaticEnums;

namespace KemoCard.Scripts.Cards
{
    partial class CardDiscardingState : CardState
    {
        public override void Enter()
        {
            //cardObject.EFRect.Visible = true;
            //BattleStatic.discard_list.Add(cardObject.card);
            BattleStatic.SelectCard(cardObject);
            StaticInstance.EventMgr.Dispatch("SelectCard");
        }

        public override void OnGUIInput(InputEvent @event)
        {
            if (@event.IsActionPressed("left_mouse"))
            {
                if (BattleStatic.isDiscarding)
                {
                    BattleStatic.SelectCard(cardObject);
                    cardObject.csm.OnTransitionRequest(this, CardStateEnum.Base);
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
