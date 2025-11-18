using Godot;
using static KemoCard.Scripts.StaticEnums;

namespace KemoCard.Scripts.Cards
{
    internal partial class CardBaseState : CardState
    {
        public override void Enter()
        {
            if (!CardObject.IsNodeReady())
            {
                CardObject.Ready += OnEnter;
            }
            else
            {
                OnEnter();
            }
        }

        private void OnEnter()
        {
            CardObject.ReparentToSourceRoot();
            object[] param = [-1];
            StaticInstance.EventMgr.Dispatch("RepositionHand", param);
            StaticInstance.EventMgr.Dispatch("DraggingCard");
            //cardObject.BgRect.Color = new("0000006e");
            //cardObject.PivotOffset = Vector2.Zero;
            CardObject.PivotOffset = new Vector2(CardObject.Size.X / 2, CardObject.Size.Y);
            CardObject.StateLabel.Text = "BASE";
            if (CardObject.SvContainer.Material is ShaderMaterial sm)
            {
                sm.SetShaderParameter("x_rot", 0);
                sm.SetShaderParameter("y_rot", 0);
            }

            if (CardObject.EnterTween != null && CardObject.EnterTween.IsRunning()) CardObject.EnterTween.Kill();
            CardObject.EnterTween = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
            CardObject.EnterTween.TweenProperty(this, "scale", Vector2.One, 0.55f);
            CardObject.EnterTween.Play();
        }

        public override void OnGUIInput(InputEvent @event)
        {
            if (!@event.IsActionPressed("left_mouse")) return;
            if (BattleStatic.IsDiscarding)
            {
                if (BattleStatic.SelectFilterFunc == null || BattleStatic.SelectFilterFunc.Invoke(CardObject.Card))
                {
                    CardObject.Csm.OnTransitionRequest(this, CardStateEnum.Discarding);
                }
            }
            else
            {
                CardObject.PivotOffset = CardObject.GetGlobalMousePosition() - CardObject.GlobalPosition;
                CardObject.Csm.OnTransitionRequest(this, CardStateEnum.Clicked);
            }
        }
    }
}