using Godot;
using KemoCard.Pages;

namespace KemoCard.Scripts
{
    public static class StaticInstance
    {
        // 全局唯一变量
        [Export] public static MainRoot MainRoot;

        public static WindowMgr WindowMgr;

        //public static BattleCore battleCore;
        public static EventMgr EventMgr;
        [Export] public static PlayerData PlayerData;
        [Export] public static Node CurrWindow;

        // 一些颜色的配置什么的
        public static readonly string NameColor = "#" + Colors.LightYellow.ToHtml();
        public static readonly string BodyColor = '#' + Colors.Orange.ToHtml();
        public static readonly string MagicColor = '#' + Colors.MediumPurple.ToHtml();
        public static readonly string KnowledgeColor = '#' + Colors.Orchid.ToHtml();
        public static readonly string TechniqueColor = '#' + Colors.Blue.ToHtml();
    }
}