using System.Threading.Tasks;
using Custom;
using Project.Scripts.Enemy;
using Project.Scripts.Weapon;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Scripts
{
    public class DynamicTimeScale : MonoBehaviour
    {
        private bool _isDynamic;
        [SerializeField] private float timeScaleWithZeroDistance;
        [SerializeField] private float timeScaleWithOneDistance;
        public async Task StartDynamicTimeScaleChange(Bullet bullet)
        {
            _isDynamic = true;
            Vector3 colliderSize = bullet.enemyTrigger.size;

            float maxDistance = Mathf.Sqrt(
                Mathf.Pow(colliderSize.x / 2f, 2f) +
                Mathf.Pow(colliderSize.y / 2f, 2f) +
                Mathf.Pow(colliderSize.z / 2f, 2f)
            );
            while (_isDynamic&& Application.isPlaying )
            {
                float distance = GetDistanceToEnemy(bullet.enemyTrigger, bullet.enemyLayerMask);
                if (distance!=-1)
                { float distanceProgress = Mathf.Clamp01(distance / maxDistance);
                    
                    float interpolatedTimeScale = Mathf.Lerp(timeScaleWithZeroDistance, timeScaleWithOneDistance, distanceProgress);
                    CTime.timeScale = interpolatedTimeScale;
                }
                else
                {
                    CTime.timeScale = timeScaleWithOneDistance;
                }
                await Task.Yield();
            }
        }

        private float GetDistanceToEnemy(BoxCollider bulletCollider, LayerMask layerMask)
        {
            float minimumDistance = -1;
            Collider[] hitColliders = Physics.OverlapBox(bulletCollider.transform.position + bulletCollider.center, 
                bulletCollider.size / 2f, 
                bulletCollider.transform.rotation, 
                layerMask);
            if (hitColliders.Length > 0)
            {
                // Теперь перебираем только тех врагов, которые были найдены OverlapBox
                foreach (Collider hitCollider in hitColliders)
                {
                    // Убедимся, что это действительно враг (можно добавить проверку тега или компонента)
                    // if (hitCollider.CompareTag("Enemy")) 
                    // {
                    float distanceToEnemy = Vector3.Distance(bulletCollider.transform.position, hitCollider.transform.position);

                    if (minimumDistance == -1 || distanceToEnemy < minimumDistance)
                    {
                        minimumDistance = distanceToEnemy;
                    }
                    // }
                } 
            }
            return minimumDistance;
        }
        public void StopDynamicTimeScaleChange()
        {
            _isDynamic = false;
        }
    }
}