using Godot;
using StaticClass;
using static StaticClass.StaticEnums;

namespace KemoCard.Scripts.Cards
{
    partial class CardBaseState : CardState
    {
        public override void Enter()
        {
            if (!cardObject.IsNodeReady())
            {
                cardObject.Ready += new(() =>
                {
                    OnEnter();
                });
            }
            else
            {
                OnEnter();
            }
        }

        void OnEnter()
        {
            cardObject.ReparentToSourceRoot();
            object[] param = { -1 };
            StaticInstance.eventMgr.Dispatch("RepositionHand", param);
            StaticInstance.eventMgr.Dispatch("DraggingCard");
            cardObject.colorRect.Color = new("0000006e");
            //cardObject.PivotOffset = Vector2.Zero;
            cardObject.PivotOffset = new(cardObject.Size.X / 2, cardObject.Size.Y);
            cardObject.stateLabel.Text = "BASE";
            (cardObject.SVContainer.Material as ShaderMaterial).SetShaderParameter("x_rot", 0);
            (cardObject.SVContainer.Material as ShaderMaterial).SetShaderParameter("y_rot", 0);
            if (cardObject.EnterTween != null && cardObject.EnterTween.IsRunning()) cardObject.EnterTween.Kill();
            cardObject.EnterTween = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
            cardObject.EnterTween.TweenProperty(this, "scale", Vector2.One, 0.55f);
            cardObject.EnterTween.Play();
        }

        public override void OnGUIInput(InputEvent @event)
        {
            if (@event.IsActionPressed("left_mouse"))
            {
                cardObject.PivotOffset = cardObject.GetGlobalMousePosition() - cardObject.GlobalPosition;
                cardObject.csm.OnTransitionRequest(this, State.CLICKED);
            }
        }
    }
}
