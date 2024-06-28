using Godot;
using KemoCard.Scripts;

namespace KemoCard.Mods.MainPackage.Scripts.MainPackage
{
    internal partial class ModBoost : BaseModBoost
    {
        public override void OnInit()
        {
            GD.Print("MainPackage装载中");
        }
        public override void OnInitEnd()
        {
            GD.Print("MainPackage已装载");
        }
    }
}
