using Godot;
using static KemoCard.Scripts.StaticEnums;

namespace KemoCard.Scripts.Cards
{
    internal partial class CardClickedState : CardState
    {
        public override void Enter()
        {
            OnEnter();
        }

        private void OnEnter()
        {
            //cardObject.BgRect.Color = Colors.Orange;
            CardObject.PivotOffset = Vector2.Zero;
            CardObject.DropPointDetector.Monitoring = true;
            CardObject.StateLabel.Text = "CLICKED";
        }

        public override void OnInput(InputEvent @event)
        {
            if (@event is not InputEventMouseMotion) return;
            var type = CardObject.Card.TargetType;
            CardObject.Csm.OnTransitionRequest(this,
                type is TargetType.AllSingle or TargetType.EnemySingle or TargetType.TeamSingle
                    ? CardStateEnum.TargetDragging
                    : CardStateEnum.AllOrSelfDragging);
        }
    }
}
