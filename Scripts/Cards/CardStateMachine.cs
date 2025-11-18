using Godot;
using Godot.Collections;
using KemoCard.Pages;
using static KemoCard.Scripts.StaticEnums;

namespace KemoCard.Scripts.Cards;

public partial class CardStateMachine : Control
{
    [Export] public CardState InitialState;
    public CardState CurrentState;
    private Dictionary<CardStateEnum, CardState> _states = new();

    public void Init(CardObject card)
    {
        foreach (var child in GetChildren())
        {
            if (child is not CardState c) continue;
            _states[c.State] = c;
            //c.TransitionRequest += OnTransitionRequest;
            c.CardObject = card;
        }

        if (InitialState == null) return;
        InitialState.Enter();
        CurrentState = InitialState;
    }

    public void OnInput(InputEvent @event)
    {
        CurrentState?.OnInput(@event);
    }

    public void OnGUIInput(InputEvent @event)
    {
        CurrentState?.OnGUIInput(@event);
    }

    public void OnMouseEnter()
    {
        CurrentState?.OnMouseEnter();
    }

    public void OnMouseExit()
    {
        CurrentState?.OnMouseExit();
    }

    public void OnTransitionRequest(CardState from, CardStateEnum to)
    {
        if (from != CurrentState)
        {
            return;
        }

        var newState = _states[to];
        if (newState == null)
        {
            return;
        }

        CurrentState?.Exit();
        CurrentState = newState;
        newState.Enter();
    }

    public void Process(double delta)
    {
        CurrentState?.Process(delta);
    }

    public void ReceiveEvent(string @event, params object[] datas)
    {
        CurrentState.ReceiveEvent(@event, datas);
    }
}