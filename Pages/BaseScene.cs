using Godot;
using KemoCard.Scripts;
using StaticClass;

namespace KemoCard.Pages
{
    public partial class BaseScene : Node2D
    {
        [Export] public bool ShowBlackWhenAdd = false;
        [Export] public bool ShowBlackWhenRemove = false;

        public virtual void OnAdd(dynamic datas = null)
        {
        }

        public virtual void OnRemove()
        {
        }

        public override void _Ready()
        {
            base._Ready();
        }

        public override void _ExitTree()
        {
            StaticInstance.windowMgr.RemoveScene(this);
            OnRemove();
            if (this is IEvent eventScene) StaticInstance.eventMgr.UnregistIEvent(eventScene);
            base._ExitTree();
        }

        public override void _EnterTree()
        {
            base._EnterTree();
            if (this is IEvent eventScene) StaticInstance.eventMgr.RegistIEvent(eventScene);
        }

        public static void DispatchEvent(string @event, params object[] datas)
        {
            StaticInstance.eventMgr.Dispatch(@event, datas);
        }
    }
}
