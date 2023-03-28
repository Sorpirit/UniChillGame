using System;

public partial class CharacterFightingController
{
    [Serializable]
    public struct AttackInfo
    {
        public PlayerAnimationState State;
        public float rechargTime;
        public float damageAmount;

        public bool Equals(AttackInfo other)
        {
            return State == other.State && rechargTime.Equals(other.rechargTime) && damageAmount.Equals(other.damageAmount);
        }

        public override bool Equals(object obj)
        {
            return obj is AttackInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int) State, rechargTime, damageAmount);
        }
    }
}