using System.Linq;
using Godot;
using KemoCard.Scripts;

namespace KemoCard.Pages;

public partial class DeckEditScene : BaseScene
{
    [Export] MenuButton flagFilter;
    [Export] TextEdit IDFilter;
    [Export] TextEdit NameFilter;
    [Export] Godot.Button clearBtn;
    private int _flags;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var popup = flagFilter.GetPopup();
        foreach (var flag in StaticEnums.CardFlagDic.Values)
        {
            popup.AddCheckItem(flag);
        }

        popup.IndexPressed += (index) =>
        {
            // var text = popup.GetItemText((int)index);
            var value = StaticEnums.CardFlagDic.ElementAt((int)index).Key;
            if ((_flags & value) > 0)
            {
                _flags &= ~value;
            }
            else
            {
                _flags |= value;
            }

            ReDrawMenu();
        };
        clearBtn.Pressed += () =>
        {
            _flags = 0;
            ReDrawMenu();
        };
    }

    private void ReDrawMenu()
    {
        if (flagFilter == null) return;
        for (var i = 0; i < flagFilter.GetPopup().ItemCount; i++)
        {
            flagFilter.GetPopup().SetItemChecked(i, (_flags & StaticEnums.CardFlagDic.ElementAt(i).Key) > 0);
        }
    }
}