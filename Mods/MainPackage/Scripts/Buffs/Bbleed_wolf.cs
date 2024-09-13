using KemoCard.Scripts;
using KemoCard.Scripts.Buffs;
using StaticClass;

namespace KemoCard.Mods.MainPackage.Scripts.Buffs
{
    internal partial class Bbleed_wolf : BaseBuffScript
    {
        public override void OnBuffInit(BuffImplBase buff)
        {
            buff.AddEvent("NewTurn", new(datas =>
            {
                if (StaticUtils.TryGetBattleScene() is BattleScene bs)
                {
                    if (buff.Creator is BaseRole creator && buff.Binder is BaseRole binder)
                        bs.DealDamage(buff.BuffValue, StaticEnums.AttackType.Physics, creator, new() { binder });
                }
            }));
        }
    }
}
