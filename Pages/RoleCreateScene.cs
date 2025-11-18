using Godot;
using KemoCard.Scripts;
using KemoCard.Scripts.Cards;

// ReSharper disable InconsistentNaming

namespace KemoCard.Pages;

public partial class RoleCreateScene : BaseScene
{
    // Called when the node enters the scene tree for the first time.
    private int CurrentPoint = 2;
    private int SpeedPoint = 5;
    private int StrengthPoint = 5;
    private int EfficiencyPoint = 5;
    private int MantraPoint = 5;
    private int CraftEquipPoint = 5;
    private int CraftBookPoint = 5;
    private int CriticalPoint = 5;
    private int DodgePoint = 5;
    [Export] private TextEdit NameInput;
    [Export] private Label SpeedTxt;
    [Export] private Label StrengthTxt;
    [Export] private Label EfficiencyTxt;
    [Export] private Label MantraTxt;
    [Export] private Label EquipTxt;
    [Export] private Label BookTxt;
    [Export] private Label CriticalTxt;
    [Export] private Label DodgeTxt;
    [Export] private Button SpeedMinus;
    [Export] private Button StrengthMinus;
    [Export] private Button EfficiencyMinus;
    [Export] private Button MantraMinus;
    [Export] private Button EquipMinus;
    [Export] private Button BookMinus;
    [Export] private Button CriticalMinus;
    [Export] private Button DodgeMinus;
    [Export] private Button SpeedPlus;
    [Export] private Button StrengthPlus;
    [Export] private Button EfficiencyPlus;
    [Export] private Button MantraPlus;
    [Export] private Button EquipPlus;
    [Export] private Button BookPlus;
    [Export] private Button CriticalPlus;
    [Export] private Button DodgePlus;
    [Export] private Button ConfirmBtn;
    [Export] private Label RemainPoint;
    [Export] private TextEdit PresetRoleInput;

    public override void _Ready()
    {
        UpdateView();
        StaticUtils.StartDialogue("res://Resources/Dialogues/StarterTutorial.dialogue");
        SpeedPlus.Pressed += () => TryPlusProperty("SpeedPoint");
        SpeedMinus.Pressed += () => TryMinusProperty("SpeedPoint");
        StrengthPlus.Pressed += () => TryPlusProperty("StrengthPoint");
        StrengthMinus.Pressed += () => TryMinusProperty("StrengthPoint");
        EfficiencyPlus.Pressed += () => TryPlusProperty("EfficiencyPoint");
        EfficiencyMinus.Pressed += () => TryMinusProperty("EfficiencyPoint");
        MantraPlus.Pressed += () => TryPlusProperty("MantraPoint");
        MantraMinus.Pressed += () => TryMinusProperty("MantraPoint");
        EquipPlus.Pressed += () => TryPlusProperty("CraftEquipPoint");
        EquipMinus.Pressed += () => TryMinusProperty("CraftEquipPoint");
        BookPlus.Pressed += () => TryPlusProperty("CraftBookPoint");
        BookMinus.Pressed += () => TryMinusProperty("CraftBookPoint");
        CriticalPlus.Pressed += () => TryPlusProperty("CriticalPoint");
        CriticalMinus.Pressed += () => TryMinusProperty("CriticalPoint");
        DodgePlus.Pressed += () => TryPlusProperty("DodgePoint");
        DodgeMinus.Pressed += () => TryMinusProperty("DodgePoint");
        ConfirmBtn.Pressed += DoConfirm;
    }

    private void UpdateView()
    {
        SpeedTxt.Text = SpeedPoint.ToString();
        StrengthTxt.Text = StrengthPoint.ToString();
        EfficiencyTxt.Text = EfficiencyPoint.ToString();
        MantraTxt.Text = MantraPoint.ToString();
        EquipTxt.Text = CraftEquipPoint.ToString();
        BookTxt.Text = CraftBookPoint.ToString();
        CriticalTxt.Text = CriticalPoint.ToString();
        DodgeTxt.Text = DodgePoint.ToString();
        RemainPoint.Text = "剩余点数：" + CurrentPoint;
    }

    private void TryPlusProperty(string propertyName)
    {
        var property = Get(propertyName);
        if (property.Obj != null && CurrentPoint > 0)
        {
            CurrentPoint--;
            Set(propertyName, (long)property.Obj + 1);
        }

        UpdateView();
    }

    private void TryMinusProperty(string propertyName)
    {
        var v = Get(propertyName);
        if (v.Obj != null)
        {
            if (((long)v) > 0)
            {
                Set(propertyName, ((long)v) - 1);
                CurrentPoint++;
            }
        }

        UpdateView();
    }

    private void DoConfirm()
    {
        PlayerRole role = null;
        if (PresetRoleInput.Text.Length > 0)
        {
            role = new PlayerRole(PresetRoleInput.Text);
        }

        if (role != null && role.Id != "")
        {
            AlertView.PopupAlert($"检测到已填入预设角色id。角色的数据为：\r\n{role.GetRichDesc()}是否确定？", false, () =>
            {
                role.StartFunction?.Invoke();

                StaticInstance.PlayerData.Gsd.MajorRole = role;

                var node =
                    (MainScene)ResourceLoader.Load<PackedScene>("res://Pages/MainScene.tscn").Instantiate();
                StaticInstance.WindowMgr.ChangeScene(node, _ => { StaticInstance.MainRoot.CanPause = true; });
            });
        }
        else
        {
            if (CurrentPoint > 0)
            {
                StaticInstance.MainRoot.ShowBanner("还有点数未使用");
            }
            else if (string.IsNullOrEmpty(NameInput.Text))
            {
                AlertView.PopupAlert("未输入名字，是否以默认名字Able继续？", false, StartGameAsync);
            }
            else
            {
                StartGameAsync();
            }
        }
    }

    private void StartGameAsync()
    {
        BaseRole player = new(SpeedPoint, StrengthPoint, EfficiencyPoint, MantraPoint, CraftEquipPoint, CraftBookPoint,
            CriticalPoint, DodgePoint)
        {
            Name = NameInput.Text == "" ? "Able" : NameInput.Text,
        };

        var majorRole = new PlayerRole(player.OriginSpeed, player.OriginStrength, player.OriginEffeciency,
            player.OriginMantra, player.OriginCraftEquip, player.OriginCraftBook, player.OriginCritical,
            player.OriginDodge, player.Name)
        {
            Id = "major"
        };
        majorRole.AddCardIntoDeck(new Card("infinite"));
        majorRole.AddCardIntoDeck(new Card("infinite"));
        StaticUtils.CreateBuffAndAddToRole("get_lucky", majorRole, majorRole);
        StaticUtils.CreateEquipAndPutOn("base_attack", majorRole);
        StaticUtils.CreateEquipAndPutOn("base_attack", majorRole);
        StaticUtils.CreateEquipAndPutOn("base_defense", majorRole);
        StaticUtils.CreateEquipAndPutOn("base_defense", majorRole);
        majorRole.ActionPoint = 3;

        StaticInstance.PlayerData.Gsd.MajorRole = majorRole;

        var node = (MainScene)ResourceLoader.Load<PackedScene>("res://Pages/MainScene.tscn").Instantiate();
        StaticInstance.WindowMgr.ChangeScene(node, _ => { StaticInstance.MainRoot.CanPause = true; });
    }
}