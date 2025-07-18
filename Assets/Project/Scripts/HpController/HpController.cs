using UnityEngine;
using StarterAssets.FirstPersonController.Scripts.PlayerHpSystem;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HpController : MonoBehaviour
{
    private PlayerHpModel _playerHpModel;
    private PlayerHpView _playerHpView;
    [SerializeField]private Slider hpScrollbar;
    [SerializeField]private UnityEvent onDeath;
    private void Start()
    {
        _playerHpModel = new PlayerHpModel(onDeath);
        _playerHpView = new PlayerHpView(hpScrollbar);
        _playerHpView.UpdateHp(_playerHpModel.CurrentHealth, _playerHpModel.maxHp);
    }

    public bool TakeDamage(int damage)
    {
        Debug.Log(damage + " damage");
        bool death = _playerHpModel.TakeDamage(damage);
        _playerHpView.UpdateHp(_playerHpModel.CurrentHealth, _playerHpModel.maxHp);
        return death;
    }

    public void Heal(int heal)
    {
        Debug.Log(heal + " heal");
        _playerHpModel.RegenerateHp(heal);
        _playerHpView.UpdateHp(_playerHpModel.CurrentHealth, _playerHpModel.maxHp);
    }
}