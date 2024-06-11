using Godot;
using static StaticClass.StaticEnums;

namespace KemoCard.Scripts.Cards
{
    public partial class CardState : Control
    {
        [Export] public State state = State.BASE;

        //[Signal] public delegate void TransitionRequestEventHandler(CardState from, State to);

        public CardObject cardObject;

        public virtual void Enter()
        {

        }

        public virtual void Exit()
        {

        }

        public virtual void OnInput(InputEvent @event)
        {
        }

        public virtual void OnGUIInput(InputEvent @event)
        {
        }

        public virtual void OnMouseEnter()
        {

        }

        public virtual void OnMouseExit()
        {

        }

        public virtual void Process(double delta)
        {

        }
    }
}
