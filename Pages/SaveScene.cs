using Godot;
using KemoCard.Pages;
using StaticClass;

public partial class SaveScene : BaseScene
{
    [Export] Godot.Button exitBtn;
    [Export] HFlowContainer SaveList;

    // Called when the node enters the scene tree for the first time.
    public override void OnAdd(dynamic datas = null)
    {
        PackedScene save_list_item = ResourceLoader.Load<PackedScene>("res://Pages/save_list_item.tscn");
        save_list_item.ResourceLocalToScene = true;
        for (int i = 1; i <= 100; i++)
        {
            Control container = (Control)save_list_item.Instantiate();
            container.MouseFilter = Control.MouseFilterEnum.Pass;
            container.GuiInput += new((@event) =>
            {
                if (@event is InputEventMouseButton mb)
                {
                    if (mb.DoubleClick && mb.IsPressed())
                    {
                        var index = (uint)container.GetIndex();
                        GD.Print("Control" + container.GetIndex() + " has been clicked");
                        AlertView.PopupAlert("是否在此存档位存档？该操作会覆盖已有的存档。", false, new(() =>
                        {
                            string imgPath = StaticUtils.GetSaveImgPath(index + 1);
                            //GD.Print(imgPath);
                            StaticInstance.playerData.Save(index + 1);
                            //if (!DirAccess.DirExistsAbsolute(imgPath))
                            //{
                            //    var error = DirAccess.MakeDirRecursiveAbsolute(imgPath);
                            //    GD.Print(error);
                            //    return;
                            //}
                            StaticInstance.playerData.screen_snapshot.SaveJpg(imgPath);
                            (container.GetChild(0) as TextureRect).Texture = ImageTexture.CreateFromImage(StaticInstance.playerData.screen_snapshot);
                        }));
                    }
                }
            });
            (container.GetChild(1) as Label).Text = "s" + i + (i == 1 ? "(自动存档)" : "");
            string path = StaticUtils.GetSavePath((uint)i);
            if (FileAccess.FileExists(path))
            {
                string imgpath = StaticUtils.GetSaveImgPath((uint)i);
                if (FileAccess.FileExists(imgpath))
                {
                    (container.GetChild(0) as TextureRect).Texture = ImageTexture.CreateFromImage(Image.LoadFromFile(imgpath));
                }
            }
            SaveList.AddChild(container);
        }
        exitBtn.Pressed += new(() =>
        {
            StaticInstance.windowMgr.RemoveScene(this);
        });
    }
}
