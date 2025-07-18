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
        [SerializeField] private LayerMask enemyLayerMask;
        private CancellationTokenSource _cts;
        public void Init(BulletGiver bulletGiver )
        {
            speed=defaultSpeed;
            _bulletGiver = bulletGiver;
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
                speed = SpeedShiftByReflectorType(path[_currentTarget]);
                transform.LookAt(_currentTarget);
                while (Vector3.Distance(transform.position,points[^1]) > 0.01f&&!_cts.IsCancellationRequested)
                {
                    transform.position = Vector3.MoveTowards(transform.position, _currentTarget, smooth*CTime.timeScale*speed);

                    if (Vector3.Distance(transform.position, _currentTarget) < 0.01f && lastIndex < points.Count - 1)
                    {
                        
                        lastIndex++;
                        _currentTarget = points[lastIndex];
                        speed = SpeedShiftByReflectorType(path[_currentTarget]);
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

        private int DamageBySpeed(float speed)
        {
            if (speed >= defaultSpeed)
            {
                return 100;
            }
            else
            {
                return (int)Mathf.Round(100*(speed/defaultSpeed));
            }
        }
        public void OnTriggerEnter(Collider other)
        {
            Debug.Log("Trigger enter");
            Debug.Log(other.gameObject.name);
            if (LayerMaskComparer.Equals(enemyLayerMask, other.gameObject.layer))
            {
                Debug.Log("Collision with enemy");
                HpController hpController = other.gameObject.GetComponent<HpController>();
                if (hpController != null)
                {
                    Debug.Log("Hp detected");
                    bool death = hpController.TakeDamage(DamageBySpeed(speed));
                    if (!death)
                    {   
                        Debug.Log("Not Death");
                        if(_cts!=null)_cts.Cancel();
                    }
                    Debug.Log("Death");
                }
            }
        }

        /*
        public void OnTriggerExit(Collider other)
        {
            
        }*/
        private void Death()
        {
            _bulletGiver.ReturnGameObjectToPool(gameObject);
        }
    }
}