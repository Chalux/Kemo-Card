using Godot;
using Godot.Collections;
using KemoCard.Scripts;
using KemoCard.Scripts.Map;

namespace KemoCard.Mods.MainPackage.Scripts.Maps;

internal partial class Cave : BaseMapScript
{
    public override void Init(MapData data)
    {
        data.ShowName = "洞穴";
        data.Cond = new Dictionary<string, Array<Variant>>
            { { "Map", [Variant.From("swamp"), Variant.From("forest")] } };
    }
}

internal partial class Forest : BaseMapScript
{
    public override void Init(MapData data)
    {
        data.ShowName = "森林";
    }
}

internal partial class Swamp : BaseMapScript
{
    public override void Init(MapData data)
    {
        data.ShowName = "沼泽";
        data.MapStartAction = SwampStartFunction;
    }

    private static void SwampStartFunction()
    {
        //PackedScene packedScene = ResourceLoader.Load<PackedScene>("res://Pages/DialogueScene.tscn");
        //if (packedScene != null)
        //{
        //    DialogueScene dialogueScene = packedScene.Instantiate<DialogueScene>();
        //    StaticInstance.windowMgr.PopScene(dialogueScene);
        //    dialogueScene.ChangeBG("res://Resources/Images/forest.png");
        //    Resource dialogue = ResourceLoader.Load<Resource>("res://Mods/MainPackage/Resources/Dialogues/Dswamp_start.dialogue");
        //    if (dialogue != null)
        //    {
        //        DialogueManager.ShowDialogueBalloon(dialogue);
        //        DialogueManager.DialogueEnded = SwampEndDialog;
        //    }
        //}
        StaticUtils.StartDialogue("res://Mods/MainPackage/Resources/Dialogues/Dswamp_start.dialogue");
    }
}