using Godot;
using KemoCard.Scripts;

namespace KemoCard.Pages;

public partial class EquipObject : Control
{
    [Export] private TextureRect _textureRect;
    public Equip Data { get; set; }
    private int _delegateTag;

    public void Init(Equip equip)
    {
        Data = equip;
        OnInit();
    }

    public void Clear()
    {
        _textureRect.Texture = null;
        Data = null;
    }

    private void OnInit()
    {
        if (Data is null)
        {
            _textureRect.Texture = null;
        }
        else
        {
            var res = ResourceLoader.Load<CompressedTexture2D>(Data.EquipScript.TextureUrl);
            //if (res != null) textureRect.Texture = ImageTexture.CreateFromImage(res);
            if (res != null) _textureRect.Texture = res;
        }
    }

    public void AddBagDelegate(int type = 1)
    {
        _delegateTag = type;
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton mb) return;
        switch (_delegateTag)
        {
            case 1:
                if (mb.Pressed && Data != null)
                {
                    if (StaticInstance.CurrWindow is BagScene bs)
                    {
                        bs.Popup.SetItemText(0, "装备");
                        bs.ShowPopMenu(Data, (uint)GetIndex(), GetGlobalMousePosition(), BagOpType.PutOn);
                    }
                }

                break;
            case 2:
                if (mb.Pressed && Data != null)
                {
                    if (StaticInstance.CurrWindow is BagScene bs)
                    {
                        bs.Popup.SetItemText(0, "脱下");
                        bs.ShowPopMenu(Data, (uint)GetIndex(), GetGlobalMousePosition(), BagOpType.PutOff);
                    }
                }

                break;
        }
    }

    public override void _Ready()
    {
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }

    private static void OnMouseExited()
    {
        StaticInstance.MainRoot.HideRichHint();
    }

    private void OnMouseEntered()
    {
        if (Data != null) StaticInstance.MainRoot.ShowRichHint(Data.ToString());
    }
}