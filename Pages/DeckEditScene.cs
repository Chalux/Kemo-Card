using System.Linq;
using Godot;
using KemoCard.Scripts;

namespace KemoCard.Pages;

public partial class DeckEditScene : BaseScene
{
    [Export] private MenuButton _flagFilter;
    [Export] private TextEdit _idFilter;
    [Export] private TextEdit _nameFilter;
    [Export] private Button _clearBtn;
    private int _flags;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var popup = _flagFilter.GetPopup();
        foreach (var flag in StaticEnums.CardFlagDic.Values)
        {
            popup.AddCheckItem(flag);
        }

        popup.IndexPressed += index =>
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
        _clearBtn.Pressed += () =>
        {
            _flags = 0;
            ReDrawMenu();
        };
    }

    private void ReDrawMenu()
    {
        if (_flagFilter == null) return;
        for (var i = 0; i < _flagFilter.GetPopup().ItemCount; i++)
        {
            _flagFilter.GetPopup().SetItemChecked(i, (_flags & StaticEnums.CardFlagDic.ElementAt(i).Key) > 0);
        }
    }
}