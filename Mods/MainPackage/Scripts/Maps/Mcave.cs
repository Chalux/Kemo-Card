using Godot;
using KemoCard.Scripts.Map;

namespace KemoCard.Mods.MainPackage.Scripts.Maps
{
    internal partial class Mcave : BaseMapScript
    {
        public override void Init(MapData data)
        {
            data.ShowName = "洞穴";
            data.Cond = new() { { "Map", new() { Variant.From("swamp"), Variant.From("forest") } } };
        }
    }
}
