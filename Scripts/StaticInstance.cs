using Godot;
using KemoCard.Scripts;

namespace StaticClass
{
    public static class StaticInstance
    {
        // 全局唯一变量
        [Export] public static main_root MainRoot;
        public static WindowMgr windowMgr;
        //public static BattleCore battleCore;
        public static EventMgr eventMgr;
        [Export] public static PlayerData playerData;
        [Export] public static Node currWindow;

        // 一些颜色的配置什么的
        public static readonly string NameColor = "#" + Colors.LightYellow.ToHtml();
        public static readonly string BodyColor = '#' + Colors.Orange.ToHtml();
        public static readonly string MagicColor = '#' + Colors.MediumPurple.ToHtml();
        public static readonly string KnowlegdeColor = '#' + Colors.Orchid.ToHtml();
        public static readonly string TechniqueColor = '#' + Colors.Blue.ToHtml();
    }
}
