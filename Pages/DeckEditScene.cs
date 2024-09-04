using Godot;
using KemoCard.Pages;
using StaticClass;
using System.Linq;

public partial class DeckEditScene : BaseScene
{
    [Export] MenuButton flagFilter;
    [Export] TextEdit IDFilter;
    [Export] TextEdit NameFilter;
    [Export] Godot.Button clearBtn;
    private int flags;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var popup = flagFilter.GetPopup();
        foreach (var flag in StaticEnums.CardFlagDic.Values)
        {
            popup.AddCheckItem(flag);
        }
        popup.IndexPressed += new((index) =>
        {
            var text = popup.GetItemText((int)index);
            int value = StaticEnums.CardFlagDic.ElementAt((int)index).Key;
            if ((flags & value) > 0)
            {
                flags &= ~value;
            }
            else
            {
                flags |= value;
            }
            ReDrawMenu();
        });
        clearBtn.Pressed += new(() =>
        {
            flags = 0;
            ReDrawMenu();
        });
    }

    void ReDrawMenu()
    {
        if (flagFilter != null)
        {
            for (int i = 0; i < flagFilter.GetPopup().ItemCount; i++)
            {
                flagFilter.GetPopup().SetItemChecked(i, (flags & StaticEnums.CardFlagDic.ElementAt(i).Key) > 0);
            }
        }
    }
}
