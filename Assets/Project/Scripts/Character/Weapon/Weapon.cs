using System.Collections.Generic;
using Custom;
using Project.Scripts.Character;
using Project.Scripts.Enemy;
using Project.Scripts.Enums;
using Project.Scripts.Pool;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Weapon
{
    public class Weapon : MonoBehaviour
    {
        private List<Vector3> path;
        [SerializeField]private int bulletCount = 1;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private UnityEvent onBulletEnd;
        [SerializeField] private BulletPathPreview bulletPathPreview;
        [SerializeField] private Transform firePoint;
        [SerializeField] private DynamicTimeScale dynamicTimeScale;
        [SerializeField] private CamManager camManager;
        private BulletGiver bulletGiver;
        private Bullet lastBullet;
        
        private void Awake()
        {
            InputManager.playerInput.Weapon.Shoot.performed += e =>
            {
                if(ModeManager.Instance.nowMode==Mode.ShootMode&&!InputFieldFocusChecker.InputFieldFocused) Shoot();
            };
            bulletGiver = new BulletGiver();
            bulletGiver.SetNeededPrefab(bulletPrefab);
            InputManager.playerInput.Enable();
        }
        private void Shoot()
        {
            Debug.Log("Shoot");
            if (bulletCount > 0)
            {
                bulletCount--;
                bulletGiver.SetNeededPrefab(bulletPrefab);
                bulletGiver.SetPosition(firePoint.position);
                bulletGiver.SetRotation(firePoint.rotation);
                GameObject bulletGameObject = bulletGiver.GetGameObjectFromPool();
                
                Bullet bullet = bulletGameObject.GetComponent<Bullet>();
                if (bullet != null)
                {
                    bullet.Init(bulletGiver, this);
                    _=bullet.Shoot(bulletPathPreview.GetPoints());
                    lastBullet=bullet;
                }
            }

            if (bulletCount == 0)
            {
                onBulletEnd.Invoke();
            }
        }

        public void OnBulletEnd()
        {
            _=dynamicTimeScale.StartDynamicTimeScaleChange(lastBullet);
            _=camManager.FocusOnBullet(lastBullet);
        }

        public void CheckWin()
        {
            AllEnemyController.Instance.CheckWin();
        }
        
    }
}