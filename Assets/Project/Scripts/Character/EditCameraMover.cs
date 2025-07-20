using UnityEngine;

namespace Project.Scripts.Character.Controller
{
    public class EditCameraMover : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D _boxCollider;
        [SerializeField] private GameObject target;
        [SerializeField] private float smooth;
        private Vector3 _velocity;
        public float boundaryOffset = 0.1f;    
        private Vector3 minBounds;
        private Vector3 maxBounds;
        private void Awake()
        {
            
            minBounds = _boxCollider.bounds.min;
            maxBounds = _boxCollider.bounds.max;
        }
        private void FixedUpdate()
        {
            if (target != null && _boxCollider != null)
            {
                Vector3 playerPos = target.transform.position;

                // Проверяем и корректируем границы
                if (playerPos.x < minBounds.x + boundaryOffset)
                {
                    minBounds.x = playerPos.x - boundaryOffset;
                    maxBounds.x = minBounds.x + (_boxCollider.bounds.size.x);
                }
                if (playerPos.x > maxBounds.x - boundaryOffset)
                {
                    maxBounds.x = playerPos.x + boundaryOffset;
                    minBounds.x = maxBounds.x - (_boxCollider.bounds.size.x);
                }
                if (playerPos.y < minBounds.y + boundaryOffset)
                {
                    minBounds.y = playerPos.y - boundaryOffset;
                    maxBounds.y = minBounds.y + (_boxCollider.bounds.size.y);
                }
                if (playerPos.y > maxBounds.y - boundaryOffset)
                {
                    maxBounds.y = playerPos.y + boundaryOffset;
                    minBounds.y = maxBounds.y - (_boxCollider.bounds.size.y);
                }

                // Плавно следуем за игроком
                Vector3 targetPosition = new Vector3(
                    Mathf.Clamp(playerPos.x, minBounds.x + (_boxCollider.bounds.size.x / 2), maxBounds.x - (_boxCollider.bounds.size.x / 2)),
                    Mathf.Clamp(playerPos.y, minBounds.y + (_boxCollider.bounds.size.y / 2), maxBounds.y - (_boxCollider.bounds.size.y / 2)),
                    transform.position.z
                );

                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, smooth);
            }
                
        }
    }
}