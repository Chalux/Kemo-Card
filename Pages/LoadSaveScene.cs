using Godot;
using KemoCard.Pages;
using StaticClass;

public partial class LoadSaveScene : BaseScene
{
    [Export] Godot.Button exitBtn;
    [Export] HFlowContainer SaveList;

    // Called when the node enters the scene tree for the first time.
    public override void OnAdd(params object[] datas)
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
                        var index = container.GetIndex();
                        GD.Print("Control" + index + " has been clicked");
                        StaticInstance.playerData.Load((uint)index + 1);
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
                    var img = Image.LoadFromFile(imgpath);
                    (container.GetChild(0) as TextureRect).Texture = ImageTexture.CreateFromImage(img);
                }
            }
            SaveList.AddChild(container);
        }
        exitBtn.Pressed += new(() =>
        {
            StaticInstance.windowMgr.ChangeScene(ResourceLoader.Load<PackedScene>("res://Pages/menu_scene.tscn").Instantiate());
            StaticInstance.MainRoot.HideRichHint();
        });
    }
}
