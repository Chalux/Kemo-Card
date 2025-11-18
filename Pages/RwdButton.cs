using Godot;

namespace KemoCard.Pages;

public partial class RwdButton : Button
{
    private int _count;

    public override void _Pressed()
    {
        base._Pressed();
        _count++;
        Text = _count.ToString();
    }
}