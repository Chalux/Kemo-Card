using System.Threading.Tasks;
using DialogueManagerRuntime;
using Godot;
using KemoCard.Scripts;

namespace KemoCard.Pages;

public partial class DialogueScene : BaseScene
{
    [Export] private TextureRect _background;
    [Export] private ColorRect _blackMask;
    private Tween _switchTween;
    private int _id;

    [Signal]
    public delegate void ChangeBackgroundEventHandler(string url, float duration = 1.0f);

    public override void _Ready()
    {
        //ChangeBackground += ChangeBG;
    }

    public async Task ChangeBg(string url, float duration = 1.0f)
    {
        var image = Image.LoadFromFile(url);
        if (image != null)
        {
            if (image.GetFormat() == Image.Format.Rgb8)
            {
                image.Convert(Image.Format.Rgba8);
            }

            var toTexture = ImageTexture.CreateFromImage(image);
            _switchTween?.Kill();
            _switchTween = GetTree().CreateTween();
            _switchTween.SetParallel(false);
            _switchTween.TweenProperty(_blackMask, "modulate", Colors.Black, duration / 2);
            _switchTween.TweenCallback(Callable.From(() => _background.Texture = toTexture));
            _switchTween.TweenProperty(_blackMask, "modulate", Color.Color8(0, 0, 0, 0), duration / 2);
            _switchTween.Play();
            await ToSignal(_switchTween, Tween.SignalName.Finished);
        }
        else
        {
            StaticInstance.MainRoot.ShowBanner($"未找到图片资源{url},请检查mod或资源路径");
        }
        //return Task.CompletedTask;
    }

    public void RunDialogue(string dialogUrl)
    {
        var dialogue = ResourceLoader.Load(dialogUrl);
        if (dialogue == null)
        {
            var error = $"对话文件不存在，路径为{dialogUrl}";
            GD.Print(error);
            StaticInstance.MainRoot.ShowBanner(error);
            return;
        }

        DialogueManager.DialogueEnded += DialogueEnded;
        DialogueManager.ShowDialogueBalloon(dialogue, "", [this]);
    }

    private static void DialogueEnded(Resource dialogueResource)
    {
        StaticInstance.WindowMgr.RemoveSceneByName("DialogueScene");
    }
}