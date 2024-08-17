using KemoCard.Scripts.Map;

namespace KemoCard.Mods.MainPackage.Scripts.Maps
{
    internal partial class Mtest_map_1 : BaseMapScript
    {
        public override void Init(MapData data)
        {
            data.MinTier = 1;
            data.MaxTier = 3;
            data.ShowName = "森林";
        }
    }
}
