using System;
[Serializable]
public struct AttackInfo
{
    public PlayerAnimationState animationState;
    public float rechargeTime;
    public float damageAmount;

    public bool Equals(AttackInfo other)
    {
        return animationState == other.animationState && rechargeTime.Equals(other.rechargeTime) && damageAmount.Equals(other.damageAmount);
    }

    public override bool Equals(object obj)
    {
        return obj is AttackInfo other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int) animationState, rechargeTime, damageAmount);
    }
}