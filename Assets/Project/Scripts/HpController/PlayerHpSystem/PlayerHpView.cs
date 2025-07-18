using UnityEngine;
using UnityEngine.UI;

namespace StarterAssets.FirstPersonController.Scripts.PlayerHpSystem
{
    public class PlayerHpView
    {
        private readonly Slider _healthSlider;
        public PlayerHpView(Slider healthSlider) {if(healthSlider!=null) _healthSlider = healthSlider; }

        public void UpdateHp(int hp, int maxhp)
        {
            if(_healthSlider!=null) 
                _healthSlider.value = Mathf.Clamp(hp, 0, maxhp)*(_healthSlider.maxValue/maxhp);
            
            
        }
    }
}