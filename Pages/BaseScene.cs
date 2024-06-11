using Godot;
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
            base._ExitTree();
        }
    }
}
