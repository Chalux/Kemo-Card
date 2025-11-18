using System;
using Godot;

namespace KemoCard.Pages;

public partial class FloatingNum : Label
{
    private Vector2 _velocity;
    private Vector2 _gravity;
    private int _mass;

    public void Init(object text, Color color, Vector2 globalPosition, int fontSize = 72)
    {
        Position = globalPosition;
        Random r = new();
        Text = text.ToString();
        AddThemeFontSizeOverride("font_size", fontSize);
        PivotOffset = Size / 2;
        Modulate = color;
        _velocity = new Vector2(r.Next(-100, 100), -130);
        _gravity = new Vector2(0, 1.5f);
        _mass = 200;
    }

    public override void _Ready()
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(this, "modulate:a", 0, 0.3).SetDelay(1).SetTrans(Tween.TransitionType.Linear)
            .SetEase(Tween.EaseType.Out);
        tween.Parallel().TweenProperty(this, "scale", new Vector2(.4f, .4f), 0.3).SetDelay(1)
            .SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
        tween.Finished += RemoveSelf;
    }

    private void RemoveSelf()
    {
        GetTree().QueueDelete(this);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        _velocity += _gravity * _mass * (float)delta;
        Position += _velocity * (float)delta;
    }
}