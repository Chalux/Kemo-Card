using Godot;

namespace KemoCard.Pages;

public partial class SelectCardItem : Control
{
    [Export] public CardShowObject ShowObject;
    [Export] public TextureRect ShaderRect;
}