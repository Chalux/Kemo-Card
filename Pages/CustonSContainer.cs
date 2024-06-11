using Godot;

public partial class CustonSContainer : Control
{
    [Export] ScrollContainer scrollContainer;
    bool isDrag = false;
    float startPos = 0;
    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton)
        {
            if (@event.IsPressed())
            {
                isDrag = true;
            }
            else
            {
                isDrag = false;
            }
        }
        if (isDrag && @event is InputEventMouseMotion mm)
        {
            scrollContainer.ScrollVertical -= (int)mm.Relative.Y;
        }
    }
}
