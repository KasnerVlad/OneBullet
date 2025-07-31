using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Custom;
using Project.Scripts.Character.Controller;
using Project.Scripts.Enemy;
using Project.Scripts.Enums;
using Project.Scripts.Pool;
using Project.Scripts.SaveSystem.SaveSystemLogic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Scripts.Weapon
{
    public class Bullet : MonoBehaviour
    {
        private BulletGiver _bulletGiver;
        [SerializeField]private float smooth;
        private Vector3 _currentTarget;
        public bool _isShooting{get;private set;}
        private int lastIndex;
        [SerializeField] private Vector3 rotOffset;
        [SerializeField] private float defaultSpeed;
        [SerializeField] private GameObject hitEffect;
        private float speed;
        [SerializeField] private float bulletSpeedChange;
        public LayerMask enemyLayerMask;
        public BoxCollider enemyTrigger;
        private CancellationTokenSource _cts;
        private Weapon _weapon;
        private EnemyManager _enemyManager;
        private Vector3 previousPosition;
        private bool hited;
        public void Init(BulletGiver bulletGiver, Weapon weapon, EnemyManager enemyManager)
        {
            speed=defaultSpeed;
            _bulletGiver = bulletGiver;
            _weapon = weapon;
            _enemyManager = enemyManager;
            hited=false;
        }

        private void Update()
        {
            Vector3 direction = transform.position - previousPosition;
            float distance = direction.magnitude;
            if (distance > 0)
            {
                RaycastHit hit;
                // Делаем Raycast от предыдущей позиции в направлении текущей
                if (Physics.Raycast(previousPosition, direction.normalized, out hit, distance))
                {
                    Collider other = hit.collider;
                    Debug.Log(other.gameObject.name);
                    if (LayerMaskComparer.Equals(enemyLayerMask, other.gameObject.layer))
                    {
                        if (other.gameObject!=_enemyManager.gameObject)
                        {
                            HpController hpController = other.gameObject.GetComponent<HpController>();
                            EnemyManager enemyManager = other.gameObject.GetComponent<EnemyManager>();
                            if (hpController != null)
                            { 
                                hpController.TakeDamage(hpController.MaxHp-1);
                            }
                            if (enemyManager != null)
                            {
                                AllEnemyController.Instance.SetEnemyManager(enemyManager);
                                enemyManager.mesh.GetComponent<SkinnedMeshRenderer>().material = enemyManager.infectedMaterial;
                                enemyManager.hitSound.Play();
                                
                                if (SaveManager.Instance != null) SaveManager.Instance.AddMoney(enemyManager.moneyAmount + SaveManager.Instance.GetMoneys());
                                _weapon.OnHit();
                                hited=true;
                            }
                            _cts.Cancel();
                        }
                    }
                }
            }
            previousPosition = transform.position;
        } 
        public virtual async Task Shoot(Dictionary<Vector3, ReflectorType> path)
        {
            if (!_isShooting)
            {
                _cts = new CancellationTokenSource();
                _isShooting = true;
                List<Vector3> points = new List<Vector3>();
                foreach (var pair in path)
                {
                    points.Add(pair.Key);
                }
                
                lastIndex = 0;
                _currentTarget=points[0];
                transform.LookAt(_currentTarget);
                while (Vector3.Distance(transform.position,points[^1]) > 0.01f&&!_cts.IsCancellationRequested&& Application.isPlaying)
                {
                    transform.position = Vector3.MoveTowards(transform.position, _currentTarget, smooth*CTime.timeScale*speed);

                    if (Vector3.Distance(transform.position, _currentTarget) < 0.01f && lastIndex < points.Count - 1)
                    {
                        Instantiate(hitEffect, transform.position, Quaternion.identity);
                        speed = SpeedShiftByReflectorType(path[_currentTarget]);
                        lastIndex++;
                        _currentTarget = points[lastIndex];
                        transform.LookAt(_currentTarget);
                    }
                    await Task.Yield();
                } 
                _isShooting=false;
                _cts.Cancel();
                if (!hited)
                {
                    _weapon.OnNotHit();
                }
                Death();
            }

        }

        private float SpeedShiftByReflectorType(ReflectorType reflectorType)
        {
            switch (reflectorType)
            {
                case ReflectorType.None:
                    return speed;
                case ReflectorType.DownSpeed:
                    return speed-bulletSpeedChange;
                case ReflectorType.UpSpeed:
                    return speed+bulletSpeedChange;
            }
            return speed;
        }

        protected virtual int DamageBySpeed(float speed, int maxHp)
        {
            if (speed >= defaultSpeed)
            {
                return maxHp;
            }
            else
            {
                return (int)Mathf.Round(maxHp*(speed/defaultSpeed));
            }
        }

        /*
        public void OnTriggerExit(Collider other)
        {
            
        }*/
        private void Death()
        {
            _bulletGiver.ReturnGameObjectToPool(gameObject);/*
            _weapon.CheckWin();*/
        }
    }
}