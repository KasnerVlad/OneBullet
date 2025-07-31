using System.Collections.Generic;
using Custom;
using Project.Scripts.Character;
using Project.Scripts.Character.Controller;
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
        /*[SerializeField] private CamManager camManager;*/
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private HpController enemyHpController;
        private Bullet lastBullet;
        
        private void Start()
        {
            InputManager.playerInput.Weapon.Shoot.performed += e =>
            {
                if(ModeManager.Instance.nowMode==Mode.ShootMode&&
                   !InputFieldFocusChecker.InputFieldFocused&&enemyManager.IsPlayerControl)
                    Shoot();
            };
            AllBulletGiver.bulletGiver.SetNeededPrefab(bulletPrefab);
            InputManager.playerInput.Enable();
        }

        private void Update()
        {
            bulletPathPreview.Preview(BulletType.Normal);
        }
        private void Shoot()
        {
            Debug.Log("Shoot");
            GetComponent<AudioSource>().Play();
            if (bulletCount > 0 && bulletPathPreview.GetPathCount() > 1)
            {
                bulletCount--;
                AllBulletGiver.bulletGiver.SetNeededPrefab(bulletPrefab);
                AllBulletGiver.bulletGiver.SetPosition(firePoint.position);
                AllBulletGiver.bulletGiver.SetRotation(firePoint.rotation);
                GameObject bulletGameObject = AllBulletGiver.bulletGiver.GetGameObjectFromPool();

                Bullet bullet = bulletGameObject.GetComponent<Bullet>();
                if (bullet != null)
                {
                    bullet.Init(AllBulletGiver.bulletGiver, this, enemyManager);
                    _ = bullet.Shoot(bulletPathPreview.GetPoints());
                    lastBullet = bullet;
                }
            }

            if (bulletCount == 0)
            {
                onBulletEnd.Invoke();
            }
        }

        public void OnBulletEnd()
        {
            _=DynamicTimeScale.Instance.StartDynamicTimeScaleChange(lastBullet);/*
            _=camManager.FocusOnBullet(lastBullet);*/
        }

        public void OnNotHit()
        {
            enemyManager.SetPlayerControl(false);
            enemyHpController.Death();
        }
        public void OnHit()
        {
            DynamicTimeScale.Instance.StopDynamicTimeScaleChange();
            enemyManager.SetPlayerControl(false);
            enemyHpController.Death();
            _=CustomInvoke.Invoke(()=>AllEnemyController.Instance.CheckWin(), 100) ;
            /*
            camManager.UnFocus();*/
        }
        
    }
}