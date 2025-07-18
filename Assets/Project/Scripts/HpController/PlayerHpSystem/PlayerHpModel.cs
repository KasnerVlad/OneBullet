using UnityEngine;
using UnityEngine.Events;

namespace StarterAssets.FirstPersonController.Scripts.PlayerHpSystem
{
    public class PlayerHpModel
    {
        public readonly int maxHp = 100;
        private readonly int _minHp = 0;
        private readonly UnityEvent _onDeath;
        public int CurrentHealth{get; private set;}
        public PlayerHpModel(UnityEvent onDeath) {CurrentHealth = maxHp; _onDeath = onDeath; }

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
