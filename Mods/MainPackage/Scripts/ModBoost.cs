using Godot;
using KemoCard.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
