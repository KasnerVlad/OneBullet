using System.Collections.Generic;
using Custom;
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
        private BulletGiver bulletGiver;
        private void Awake()
        {
            InputManager.playerInput.Weapon.Shoot.performed += e => Shoot();
            bulletGiver = new BulletGiver();
            bulletGiver.SetNeededPrefab(bulletPrefab);

        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
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
                    bullet.Init(bulletGiver);
                    _=bullet.Shoot(bulletPathPreview.GetPoints());
                }
            }

            if (bulletCount == 0)
            {
                onBulletEnd.Invoke();
            }
        }

        public void OnBulletEnd()
        {
            CTime.timeScale = 0.3f;
        }
        
    }
}