using Godot;
using KemoCard.Pages;
using KemoCard.Scripts;
using StaticClass;

public partial class EquipObject : Control
{
    [Export] TextureRect textureRect;
    public Equip Data { get; set; }
    private int delegateTag = 0;

    public void Init(Equip equip)
    {
        Data = equip;
        OnInited();
    }

    public void Clear()
    {
        textureRect.Texture = null;
        Data = null;
    }

    private void OnInited()
    {
        if (Data is null)
        {
            textureRect.Texture = null;
        }
        else
        {
            var res = ResourceLoader.Load<CompressedTexture2D>(Data.EquipScript.TextureUrl);
            //if (res != null) textureRect.Texture = ImageTexture.CreateFromImage(res);
            if (res != null) textureRect.Texture = res;
        }
    }

    public void AddBagDelegate(int type = 1)
    {
        delegateTag = type;
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb)
        {
            switch (delegateTag)
            {
                case 1:
                    if (mb.Pressed && Data != null)
                    {
                        if (StaticInstance.currWindow is BagScene bs)
                        {
                            bs.popup.SetItemText(0, "装备");
                            bs.ShowPopMenu(Data, (uint)GetIndex(), GetGlobalMousePosition(), BagOpType.PUT_ON);
                        }
                    }
                    break;
                case 2:
                    if (mb.Pressed && Data != null)
                    {
                        if (StaticInstance.currWindow is BagScene bs)
                        {
                            bs.popup.SetItemText(0, "脱下");
                            bs.ShowPopMenu(Data, (uint)GetIndex(), GetGlobalMousePosition(), BagOpType.PUT_OFF);
                        }
                    }
                    break;
            }
        }
    }

    public override void _Ready()
    {
        MouseEntered += new(() =>
        {
            if (Data != null) StaticInstance.MainRoot.ShowRichHint(Data.ToString());
        });
        MouseExited += new(() =>
        {
            StaticInstance.MainRoot.HideRichHint();
        });
    }
}
