using System;

namespace Animation
{
    [Serializable]
    public enum PlayerAnimationState
    {
        Idle = 0,
        Attack = 1,
        Attack2 = 2,
        Run = 3,
        QuickTurn = 4,
        Jump = 5,
        Fall = 6,
        GetHit = 7,
        Death = 8,
    }
}