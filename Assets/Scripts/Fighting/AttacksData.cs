using UnityEngine;
using UnityEngine.Serialization;

namespace Fighting
{
    [CreateAssetMenu]
    public class AttacksData : ScriptableObject
    {
        [SerializeField] private AttackInfo[] attacks;
        [SerializeField] private float rechargeTimerMultiplayer;
        [SerializeField] private float attackBuffer;

        public AttackInfo[] Attacks => attacks;

        public float RechargeTimerMultiplayer => rechargeTimerMultiplayer;

        public float AttackBuffer => attackBuffer;
    }
}