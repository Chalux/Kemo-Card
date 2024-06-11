public partial class Button : Godot.Button
{
    int count = 0;
    public override void _Pressed()
    {
        base._Pressed();
        count++;
        Text = count.ToString();
    }
}
