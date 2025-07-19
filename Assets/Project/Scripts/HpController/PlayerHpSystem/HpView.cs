using UnityEngine;
using UnityEngine.UI;

namespace StarterAssets.FirstPersonController.Scripts.PlayerHpSystem
{
    public class HpView
    {
        private readonly Slider _healthSlider;
        public HpView(Slider healthSlider) {if(healthSlider!=null) _healthSlider = healthSlider; }

        public void UpdateHp(int hp, int maxHp)
        {
            if(_healthSlider!=null) 
                _healthSlider.value = Mathf.Clamp(hp, 0, maxHp)*(_healthSlider.maxValue/maxHp);
            
            
        }
    }
}