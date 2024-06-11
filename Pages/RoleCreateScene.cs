using Godot;
using KemoCard.Pages;
using KemoCard.Scripts;
using StaticClass;

public partial class RoleCreateScene : BaseScene
{
    // Called when the node enters the scene tree for the first time.
    public int CurrentPoint = 2;
    public int SpeedPoint = 5;
    public int StrengthPoint = 5;
    public int EfficiencyPoint = 5;
    public int MantraPoint = 5;
    public int CraftEquipPoint = 5;
    public int CraftBookPoint = 5;
    public int CriticalPoint = 5;
    public int DodgePoint = 5;
    [Export] private TextEdit NameInput;
    [Export] private Label SpeedTxt;
    [Export] private Label StrengthTxt;
    [Export] private Label EfficiencyTxt;
    [Export] private Label MantraTxt;
    [Export] private Label EquipTxt;
    [Export] private Label BookTxt;
    [Export] private Label CriticalTxt;
    [Export] private Label DodgeTxt;
    [Export] private Godot.Button SpeedMinus;
    [Export] private Godot.Button StrengthMinus;
    [Export] private Godot.Button EfficiencyMinus;
    [Export] private Godot.Button MantraMinus;
    [Export] private Godot.Button EquipMinus;
    [Export] private Godot.Button BookMinus;
    [Export] private Godot.Button CriticalMinus;
    [Export] private Godot.Button DodgeMinus;
    [Export] private Godot.Button SpeedPlus;
    [Export] private Godot.Button StrengthPlus;
    [Export] private Godot.Button EfficiencyPlus;
    [Export] private Godot.Button MantraPlus;
    [Export] private Godot.Button EquipPlus;
    [Export] private Godot.Button BookPlus;
    [Export] private Godot.Button CriticalPlus;
    [Export] private Godot.Button DodgePlus;
    [Export] private Godot.Button ConfirmBtn;
    [Export] private Label RemainPoint;
    [Export] private TextEdit PresetRoleInput;
    public override void _Ready()
    {
        UpdateView();
        SpeedPlus.Pressed += new(() => TryPlusProperty("SpeedPoint"));
        SpeedMinus.Pressed += new(() => TryMinusProperty("SpeedPoint"));
        StrengthPlus.Pressed += new(() => TryPlusProperty("StrengthPoint"));
        StrengthMinus.Pressed += new(() => TryMinusProperty("StrengthPoint"));
        EfficiencyPlus.Pressed += new(() => TryPlusProperty("EfficiencyPoint"));
        EfficiencyMinus.Pressed += new(() => TryMinusProperty("EfficiencyPoint"));
        MantraPlus.Pressed += new(() => TryPlusProperty("MantraPoint"));
        MantraMinus.Pressed += new(() => TryMinusProperty("MantraPoint"));
        EquipPlus.Pressed += new(() => TryPlusProperty("CraftEquipPoint"));
        EquipMinus.Pressed += new(() => TryMinusProperty("CraftEquipPoint"));
        BookPlus.Pressed += new(() => TryPlusProperty("CraftBookPoint"));
        BookMinus.Pressed += new(() => TryMinusProperty("CraftBookPoint"));
        CriticalPlus.Pressed += new(() => TryPlusProperty("CriticalPoint"));
        CriticalMinus.Pressed += new(() => TryMinusProperty("CriticalPoint"));
        DodgePlus.Pressed += new(() => TryPlusProperty("DodgePoint"));
        DodgeMinus.Pressed += new(() => TryMinusProperty("DodgePoint"));
        ConfirmBtn.Pressed += new(() => DoConfirm());
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
        RemainPoint.Text = "剩余点数：" + CurrentPoint.ToString();
    }

    private void TryPlusProperty(string propertyName)
    {
        if (Get(propertyName).Obj != null && CurrentPoint > 0)
        {
            CurrentPoint--;
            Set(propertyName, (long)Get(propertyName).Obj + 1);
        }
        UpdateView();
    }

    private void TryMinusProperty(string protertyName)
    {
        Variant v = Get(protertyName);
        if (v.Obj != null)
        {
            if (((long)v) > 0)
            {
                Set(protertyName, ((long)v) - 1);
                CurrentPoint++;
            }
        }
        UpdateView();
    }

    void DoConfirm()
    {
        if (PresetRoleInput.Text.ToInt() > 0)
        {
            PlayerRole role = new((uint)PresetRoleInput.Text.ToInt());
            AlertView.PopupAlert($"检测到已填入预设角色id。角色的数据为：\r\n{role.GetRichDesc()}是否确定？", false, new(() => role.StartFunction?.Call()));
        }
        if (CurrentPoint > 0)
        {
            StaticInstance.MainRoot.ShowBanner("还有点数未使用");
            return;
        }
        else if (NameInput.Text == null || NameInput.Text == "")
        {
            AlertView.PopupAlert("未输入名字，是否以默认名字Able继续？", false, new(() =>
            {
                StartGameAsync();
            }));
        }
        else
        {
            StartGameAsync();
        }
    }

    void StartGameAsync()
    {
        BaseRole player = new(SpeedPoint, StrengthPoint, EfficiencyPoint, MantraPoint, CraftEquipPoint, CraftBookPoint, CriticalPoint, DodgePoint)
        {
            name = NameInput.Text == "" ? "Able" : NameInput.Text,
        };

        var majorrole = new PlayerRole(player.OriginSpeed, player.OriginStrength, player.OriginEffeciency, player.OriginMantra, player.OriginCraftEquip, player.OriginCraftBook, player.OriginCritical, player.OriginDodge, player.name);
        majorrole.AddCardIntoDeck(new(10005));
        majorrole.AddCardIntoDeck(new(10005));
        majorrole.AddCardIntoDeck(new(10006));
        majorrole.AddCardIntoDeck(new(10006));
        majorrole.AddCardIntoDeck(new(10006));
        majorrole.AddCardIntoDeck(new(10003));
        majorrole.AddCardIntoDeck(new(10003));
        majorrole.AddCardIntoDeck(new(10004));
        if (Datas.Ins.BuffPool.TryGetValue(10001, out Datas.BuffStruct modInfo))
        {
            FileAccess script = FileAccess.Open("user://Mods/" + modInfo.mod_id + "/buff/B" + modInfo.buff_id + ".lua", FileAccess.ModeFlags.Read);
            if (script != null)
            {
                BuffImplBase data = new()
                {
                    LuaState = StaticUtils.GetOneTempLua()
                };
                data.LuaState["b"] = data;
                data.LuaState.DoString(script.GetAsText());
                majorrole.AddBuff(data);
            }
        }
        majorrole.ActionPoint = 3;

        StaticInstance.playerData.gsd.MajorRole = majorrole;

        MainScene node = (MainScene)ResourceLoader.Load<PackedScene>("res://Pages/MainScene.tscn").Instantiate();
        StaticInstance.windowMgr.ChangeScene(node, new(() =>
        {
            StaticInstance.MainRoot.canPause = true;
        }));
    }

}
