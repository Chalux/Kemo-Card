using Godot;
using KemoCard.Scripts;

namespace KemoCard.Pages
{
    public partial class BaseScene : Control
    {
        [Export] public bool ShowBlackWhenAdd;
        [Export] public bool ShowBlackWhenRemove;
        [Export] public MouseFilterEnum EnableMouseStopFilter = MouseFilterEnum.Ignore;

        public virtual void OnAdd(params object[] datas)
        {
        }

        public virtual void OnRemove()
        {
        }

        public override void _Ready()
        {
            base._Ready();
            MouseFilter = EnableMouseStopFilter;
        }

        public override void _ExitTree()
        {
            StaticInstance.WindowMgr.RemoveScene(this);
            OnRemove();
            if (this is IEvent eventScene) StaticInstance.EventMgr.UnregisterIEvent(eventScene);
            base._ExitTree();
        }

        public override void _EnterTree()
        {
            base._EnterTree();
            if (this is IEvent eventScene) StaticInstance.EventMgr.RegisterIEvent(eventScene);
        }

        public static void DispatchEvent(string @event, params object[] datas)
        {
            StaticInstance.EventMgr.Dispatch(@event, datas);
        }
    }
}