using Godot;
using static StaticClass.StaticEnums;

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
                if (type == TargetType.ALL_SINGLE || type == TargetType.ENEMY_SINGLE || type == TargetType.TEAM_SINGLE)
                    cardObject.csm.OnTransitionRequest(this, State.TARGET_DRAGGING);
                else
                    cardObject.csm.OnTransitionRequest(this, State.ALL_OR_SELF_DRAGGING);
            }
        }
    }
}
