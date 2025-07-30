using UnityEngine;

namespace Project.Scripts.Enemy
{
    public class EnemyAnimsController : MonoBehaviour
    {
        [SerializeField]private Enemy _enemy;
        private Animator _animator;
        private int attack;
        public bool IsAttaking{get;private set;}
        public bool NeedToAttack{get;private set;}
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            InitAnimsName();
        }
        private void InitAnimsName()
        {
            attack = Animator.StringToHash("Attack");
        }

        public void UnAttack()
        {
           if(_animator!=null) _animator.SetBool(attack, false);
           NeedToAttack=false;
        }
        public void Attack()
        {
            if(_animator!=null)_animator.SetBool(attack, true);
            NeedToAttack=true;
        }

        public void StartAttack(AnimationEvent animationEvent)
        {
            IsAttaking = true;
            _enemy.StartAttack();
        }
        public void EndAttack(AnimationEvent animationEvent)
        {
            if(!NeedToAttack)IsAttaking = false;
        }
        private void Update()
        {
            if(_animator!=null)
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                    _enemy.Attack();
                
        }
    }
}