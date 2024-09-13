using Godot;
using System;

public partial class FloatingNum : Label
{
    Vector2 velocity = new();
    Vector2 gravity = new();
    int mass = 0;
    public void Init(object text, Color color, Vector2 GlobalPosition, int fontSize = 72)
    {
        Position = GlobalPosition;
        Random r = new();
        Text = text.ToString();
        AddThemeFontSizeOverride("font_size", fontSize);
        PivotOffset = Size / 2;
        Modulate = color;
        velocity = new(r.Next(-100, 100), -130);
        gravity = new(0, 1.5f);
        mass = 200;
    }

    public override void _Ready()
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(this, "modulate:a", 0, 0.3).SetDelay(1).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
        tween.Parallel().TweenProperty(this, "scale", new Vector2(.4f, .4f), 0.3).SetDelay(1).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out);
        tween.Finished += RemoveSelf;
    }

    private void RemoveSelf()
    {
        GetTree().QueueDelete(this);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        velocity += gravity * mass * (float)delta;
        Position += velocity * (float)delta;
    }
}
