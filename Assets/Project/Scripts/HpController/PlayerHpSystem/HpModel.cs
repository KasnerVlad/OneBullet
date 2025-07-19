using UnityEngine;
using UnityEngine.Events;

namespace StarterAssets.FirstPersonController.Scripts.PlayerHpSystem
{
    public class HpModel
    {
        public readonly int maxHp;
        private readonly int _minHp = 0;
        private readonly UnityEvent _onDeath;
        public int CurrentHealth{get; private set;}
        public HpModel(UnityEvent onDeath, int maxHp) {this.maxHp=maxHp; CurrentHealth = maxHp; _onDeath = onDeath; }

        public bool TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, _minHp, maxHp);
            if (CurrentHealth <= 0&&_onDeath != null)
            {
                Debug.Log("On Death");
                
                _onDeath.Invoke();
                return true;
            }
            return false;
        }

        public void RegenerateHp(int amount)
        {
            /*CurrentHealth = maxHp;*/
            CurrentHealth += amount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, _minHp, maxHp);
        }
    }
}
