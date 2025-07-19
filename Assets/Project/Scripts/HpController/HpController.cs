using UnityEngine;
using StarterAssets.FirstPersonController.Scripts.PlayerHpSystem;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HpController : MonoBehaviour
{
    private HpModel _hpModel;
    private HpView _hpView;
    [SerializeField]private Slider hpScrollbar;
    [SerializeField]private int maxHp;
    [SerializeField]private UnityEvent onDeath;
    public int MaxHp=>maxHp;
    private void Start()
    {
        _hpModel = new HpModel(onDeath, maxHp);
        _hpView = new HpView(hpScrollbar);
        _hpView.UpdateHp(_hpModel.CurrentHealth, _hpModel.maxHp);
    }

    public bool TakeDamage(int damage)
    {
        Debug.Log(damage + " damage");
        bool death = _hpModel.TakeDamage(damage);
        _hpView.UpdateHp(_hpModel.CurrentHealth, _hpModel.maxHp);
        return death;
    }

    public void Heal(int heal)
    {
        Debug.Log(heal + " heal");
        _hpModel.RegenerateHp(heal);
        _hpView.UpdateHp(_hpModel.CurrentHealth, _hpModel.maxHp);
    }
}