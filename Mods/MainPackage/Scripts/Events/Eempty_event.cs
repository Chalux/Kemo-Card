using KemoCard.Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KemoCard.Mods.MainPackage.Scripts.Events
{
    internal partial class Eempty_event : BaseEventScript
    {
        public override void Init(Event e)
        {
            e.EventTitle = "似乎什么都没遇到……真的吗？";
            e.AddEvent("继续前进", "res://Mods/MainPackage/Resources/Icons/icons_064.png", null);
        }
    }
}
