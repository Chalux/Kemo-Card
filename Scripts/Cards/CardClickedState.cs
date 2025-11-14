using Godot;
using static KemoCard.Scripts.StaticEnums;

namespace KemoCard.Scripts.Cards
{
    partial class CardClickedState : CardState
    {
        public override void Enter()
        {
            OnEnter();
        }

        void OnEnter()
        {
            //cardObject.BgRect.Color = Colors.Orange;
            cardObject.PivotOffset = Vector2.Zero;
            cardObject.dropPointDetector.Monitoring = true;
            cardObject.stateLabel.Text = "CLICKED";
        }

        public override void OnInput(InputEvent @event)
        {
            if (@event is InputEventMouseMotion)
            {
                var type = cardObject.card.TargetType;
                if (type == TargetType.AllSingle || type == TargetType.EnemySingle || type == TargetType.TeamSingle)
                    cardObject.csm.OnTransitionRequest(this, CardStateEnum.TargetDragging);
                else
                    cardObject.csm.OnTransitionRequest(this, CardStateEnum.AllOrSelfDragging);
            }
        }
    }
}
