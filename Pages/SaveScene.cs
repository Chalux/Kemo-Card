using Godot;
using KemoCard.Scripts;

namespace KemoCard.Pages;

public partial class SaveScene : BaseScene
{
    [Export] private Button _exitBtn;
    [Export] private HFlowContainer _saveList;

    // Called when the node enters the scene tree for the first time.
    public override void OnAdd(params object[] datas)
    {
        var saveListItem = ResourceLoader.Load<PackedScene>("res://Pages/save_list_item.tscn");
        saveListItem.ResourceLocalToScene = true;
        for (var i = 1; i <= 100; i++)
        {
            var container = (Control)saveListItem.Instantiate();
            container.MouseFilter = MouseFilterEnum.Pass;
            container.GuiInput += @event =>
            {
                if (@event is not InputEventMouseButton mb) return;
                if (!mb.DoubleClick || !mb.IsPressed()) return;
                var index = (uint)container.GetIndex();
                GD.Print("Control" + container.GetIndex() + " has been clicked");
                AlertView.PopupAlert("是否在此存档位存档？该操作会覆盖已有的存档。", false, () =>
                {
                    //GD.Print(imgPath);
                    StaticInstance.PlayerData.Save(index + 1);
                    //if (!DirAccess.DirExistsAbsolute(imgPath))
                    //{
                    //    var error = DirAccess.MakeDirRecursiveAbsolute(imgPath);
                    //    GD.Print(error);
                    //    return;
                    //}
                    var img = StaticInstance.PlayerData.ScreenSnapshot;
                    if (img.GetFormat() == Image.Format.Rgb8)
                    {
                        img.Convert(Image.Format.Rgba8);
                    }

                    if (container.GetChild(0) is TextureRect tr)
                        tr.Texture = ImageTexture.CreateFromImage(img);
                });
            };
            if (container.GetChild(1) is Label lab) lab.Text = "s" + i + (i == 1 ? "(自动存档)" : "");
            var path = StaticUtils.GetSavePath((uint)i);
            if (FileAccess.FileExists(path))
            {
                var imgPath = StaticUtils.GetSaveImgPath((uint)i);
                if (FileAccess.FileExists(imgPath))
                {
                    var img = Image.LoadFromFile(imgPath);
                    if (img.GetFormat() == Image.Format.Rgb8)
                    {
                        img.Convert(Image.Format.Rgba8);
                    }

                    if (container.GetChild(0) is TextureRect tr)
                        tr.Texture =
                            ImageTexture.CreateFromImage(img);
                }
            }

            _saveList.AddChild(container);
        }

        _exitBtn.Pressed += () => { StaticInstance.WindowMgr.RemoveScene(this); };
    }
}