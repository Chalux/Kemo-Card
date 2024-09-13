using DialogueManagerRuntime;
using Godot;
using KemoCard.Pages;
using StaticClass;
using System.Threading.Tasks;

public partial class DialogueScene : BaseScene
{
    [Export] TextureRect Background;
    [Export] ColorRect BlackMask;
    private Tween switchTween;
    private int id = 0;
    [Signal]
    public delegate void ChangeBackgroundEventHandler(string url, float duration = 1.0f);
    public override void _Ready()
    {
        //ChangeBackground += ChangeBG;
    }
    public async Task ChangeBG(string url, float duration = 1.0f)
    {
        if (FileAccess.FileExists(url))
        {
            CompressedTexture2D image = ResourceLoader.Load<CompressedTexture2D>(url);
            var toTexture = image;
            switchTween?.Kill();
            switchTween = GetTree().CreateTween();
            switchTween.SetParallel(false);
            switchTween.TweenProperty(BlackMask, "modulate", Colors.Black, duration / 2);
            switchTween.TweenCallback(Callable.From(() => Background.Texture = toTexture));
            switchTween.TweenProperty(BlackMask, "modulate", Color.Color8(0, 0, 0, 0), duration / 2);
            switchTween.Play();
            await ToSignal(switchTween, Tween.SignalName.Finished);
        }
        else
        {
            StaticInstance.MainRoot.ShowBanner($"未找到图片资源{url},请检查mod或资源路径");
        }
        //return Task.CompletedTask;
    }

    public void RunDialogue(string dialog_url)
    {
        Resource dialogue = ResourceLoader.Load(dialog_url);
        if (dialogue == null)
        {
            string error = $"对话文件不存在，路径为{dialog_url}";
            GD.Print(error);
            StaticInstance.MainRoot.ShowBanner(error);
            return;
        }
        DialogueManager.DialogueEnded += DialogueEnded;
        DialogueManager.ShowDialogueBalloon(dialogue, null, new() { this });
    }

    private void DialogueEnded(Resource dialogueResource)
    {
        StaticInstance.windowMgr.RemoveSceneByName("DialogueScene");
    }
}
