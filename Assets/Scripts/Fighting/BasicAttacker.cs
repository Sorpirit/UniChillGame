using UnityEngine;

namespace Fighting
{
    public class BasicAttacker
    {
        private float _attackRechargeTimer;

        public bool IsAbleToAttack => _attackRechargeTimer <= 0;
        
        public void UpdateRechargeTimer(AttacksData data)
        {
            if (_attackRechargeTimer > 0)
            {
                _attackRechargeTimer -= data.RechargeTimerMultiplayer * Time.deltaTime;
                return;
            }
        }
        
        public void Attack(int index, AttacksData data)
        {
            if(!IsAbleToAttack)
                return;
            
            _attackRechargeTimer = data.Attacks[index].rechargeTime;
        }
    }
}