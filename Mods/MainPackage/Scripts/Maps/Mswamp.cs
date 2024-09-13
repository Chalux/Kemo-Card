using KemoCard.Scripts.Map;
using StaticClass;

namespace KemoCard.Mods.MainPackage.Scripts.Maps
{
    internal partial class Mswamp : BaseMapScript
    {
        public override void Init(MapData data)
        {
            data.ShowName = "沼泽";
            data.MapStartAction = SwampStartFunction;
        }

        public void SwampStartFunction()
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
}
