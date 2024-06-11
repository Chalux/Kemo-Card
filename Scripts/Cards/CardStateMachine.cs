using Godot;
using Godot.Collections;
using KemoCard.Scripts.Cards;
using static StaticClass.StaticEnums;

public partial class CardStateMachine : Control
{
    [Export] public CardState initialState;
    public CardState currentState;
    public Dictionary<State, CardState> states = new();

    public void Init(CardObject card)
    {
        foreach (var child in GetChildren())
        {
            if (child is CardState @c)
            {
                states[c.state] = c;
                //c.TransitionRequest += OnTransitionRequest;
                c.cardObject = card;
            }
        }
        if (initialState != null)
        {
            initialState.Enter();
            currentState = initialState;
        }
    }

    public void OnInput(InputEvent @event)
    {
        currentState?.OnInput(@event);
    }

    public void OnGUIInput(InputEvent @event)
    {
        currentState?.OnGUIInput(@event);
    }

    public void OnMouseEnter()
    {
        currentState?.OnMouseEnter();
    }

    public void OnMouseExit()
    {
        currentState?.OnMouseExit();
    }

    public void OnTransitionRequest(CardState from, State to)
    {
        if (from != currentState)
        {
            return;
        }
        CardState newState = states[to];
        if (newState == null)
        {
            return;
        }
        currentState?.Exit();
        currentState = newState;
        newState.Enter();
    }

    public void Process(double delta)
    {
        currentState?.Process(delta);
    }
}
