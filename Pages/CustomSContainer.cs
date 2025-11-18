using Godot;

namespace KemoCard.Pages;

public partial class CustomSContainer : Control
{
    [Export] private ScrollContainer _scrollContainer;
    private bool _isDrag;
    private float _startPos;

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton)
        {
            _isDrag = @event.IsPressed();
        }

        if (_isDrag && @event is InputEventMouseMotion mm)
        {
            _scrollContainer.ScrollVertical -= (int)mm.Relative.Y;
        }
    }
}