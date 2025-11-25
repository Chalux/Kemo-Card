using Godot;

namespace KemoCard.Scripts.Roles
{
    public partial class BaseRoleScript : RefCounted
    {
        public virtual void OnRoleInit(PlayerRole r)
        {
        }

        /// <summary>
        /// 在使用这个角色当预设角色开始游戏的时候才会调用这个函数
        /// </summary>
        public virtual void OnGameStart(PlayerRole r)
        {
        }

        public virtual void OnBattleStart(PlayerRole r)
        {
        }
    }
}