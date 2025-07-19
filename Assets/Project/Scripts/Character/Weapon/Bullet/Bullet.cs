using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Custom;
using Project.Scripts.Enums;
using Project.Scripts.Pool;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Scripts.Weapon
{
    public class Bullet : MonoBehaviour
    {
        private BulletGiver _bulletGiver;
        [SerializeField]private float smooth;
        private Vector3 _currentTarget;
        private bool _isShooting;
        private int lastIndex;
        [SerializeField] private Vector3 rotOffset;
        [SerializeField] private float defaultSpeed;
        private float speed;
        [SerializeField] private float bulletSpeedChange;
        public LayerMask enemyLayerMask;
        public BoxCollider enemyTrigger;
        private CancellationTokenSource _cts;
        private Weapon _weapon;
        private Vector3 previousPosition;   
        public void Init(BulletGiver bulletGiver, Weapon weapon)
        {
            speed=defaultSpeed;
            _bulletGiver = bulletGiver;
            _weapon = weapon;
        }

        private void Update()
        {
            Vector3 direction = transform.position - previousPosition;
            float distance = direction.magnitude;

            // Если объект действительно движется
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
                        HpController hpController = other.gameObject.GetComponent<HpController>();
                        if (hpController != null)
                        {
                            Debug.Log("Hp detected");
                            bool death = hpController.TakeDamage(DamageBySpeed(speed, hpController.MaxHp));
                            if (!death)
                            {   
                                if(_cts!=null)_cts.Cancel();
                            }
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
                _currentTarget=points[0];/*
                speed = SpeedShiftByReflectorType(path[_currentTarget]);*/
                transform.LookAt(_currentTarget);
                while (Vector3.Distance(transform.position,points[^1]) > 0.01f&&!_cts.IsCancellationRequested&& Application.isPlaying)
                {
                    transform.position = Vector3.MoveTowards(transform.position, _currentTarget, smooth*CTime.timeScale*speed);

                    if (Vector3.Distance(transform.position, _currentTarget) < 0.01f && lastIndex < points.Count - 1)
                    {
                        speed = SpeedShiftByReflectorType(path[_currentTarget]);
                        lastIndex++;
                        _currentTarget = points[lastIndex];
                        transform.LookAt(_currentTarget);
                    }
                    await Task.Yield();
                } 
                Death();
                _isShooting=false;
                _cts.Cancel();
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
            _bulletGiver.ReturnGameObjectToPool(gameObject);
            _weapon.CheckWin();
        }
    }
}