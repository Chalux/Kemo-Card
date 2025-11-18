using Godot;
using static KemoCard.Scripts.StaticEnums;

namespace KemoCard.Scripts.Cards
{
    internal partial class CardDiscardingState : CardState
    {
        public override void Enter()
        {
            //cardObject.EFRect.Visible = true;
            //BattleStatic.discard_list.Add(cardObject.card);
            BattleStatic.SelectCard(CardObject);
            StaticInstance.EventMgr.Dispatch("SelectCard");
        }

        public override void OnGUIInput(InputEvent @event)
        {
            if (!@event.IsActionPressed("left_mouse")) return;
            if (!BattleStatic.IsDiscarding) return;
            BattleStatic.SelectCard(CardObject);
            CardObject.Csm.OnTransitionRequest(this, CardStateEnum.Base);
        }

        public override void ReceiveEvent(string @event, params object[] datas)
        {
            if (@event == "SelectConfirm")
            {
                CardObject.EfRect.Visible = false;
            }
        }
    }
}