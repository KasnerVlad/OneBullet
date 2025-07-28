using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Scripts.Custom;
using UnityEngine;

namespace Project.Scripts.Enemy.CustomAIAgent
{
    public class AsyncPathfinderAI : MonoBehaviour
    {
        [Header("Настройки навигации")]/*
        [SerializeField]private Transform target;*/
        [SerializeField]private LayerMask obstacleMask;
        [SerializeField]private LayerMask walkableMask;
        [SerializeField]private float checkStep = 0.5f;
        [SerializeField]private float checkHeightOffset = 1f;
        [SerializeField]private float offset = 0.1f; // Смещение вверх от верхней границы коллайдера
        [SerializeField]private float jumpDistance = 2f; // Максимальное расстояние для продолжения
        public float speed;
        public bool isStopped;
        private Vector3 spherePos;
        public float maxJumpHeight = 2f; // Максимальная высота, которую персонаж может перепрыгнуть (для Raycast в Pathfinder)
        private Enemy _enemy;
        private CharacterController controller;
        private List<Vector3> currentPath = new();
        private int currentIndex = 0;/*
        private bool isWalking = false;*/
        private bool isCalculating = false;
        
        private Vector3 lastRequestedDestination;
        private Vector3 transformPos;
        [SerializeField]private float destinationUpdateThreshold = 0.2f;
        private float lastPathRequestTime = 0;
        [SerializeField]private float pathUpdateCooldown = 0.5f;
        private Vector3 EvadePoint1;
        private Vector3 EvadePoint2;
        /*private float lastPathRequestTime = 0;
        private float pathUpdateCooldown = 0.5f*//*;*/ // чтобы не обновлять путь слишком часто
    
        void Awake()
        {
            controller = GetComponent<CharacterController>();
            _enemy = GetComponent<Enemy>();
        }
    
        void Update()
        {
            /*if (target != null)
            {
                SetDestination(target.position);
            }*//*
            Debug.Log("updated");
            Debug.Log("current index " + currentIndex + " currentPath.Count: " + currentPath.Count);*/
            if (/*isWalking && */currentPath.Count > 0 && currentIndex < currentPath.Count)
            {
                MoveTo(currentPath[currentIndex]);/*
                Debug.Log("moving");*/
            }
        }
    
        public void SetDestination(Vector3 dest)
        {
            if (isCalculating) return;
    
            if (Time.time - lastPathRequestTime < pathUpdateCooldown) return;
            
            if (Vector3.Distance(dest, lastRequestedDestination) > destinationUpdateThreshold||transform.position == transformPos )
            {
                transformPos = transform.position;
                lastRequestedDestination = dest;
                _ = FindPathAsync(dest);
            }
        }
    
        void MoveTo(Vector3 point)
        {
            if (!isStopped)
            {
                Vector3 currentPosition = transform.position;
                // Используем Vector3.Distance для проверки достижения точки во всех измерениях, или только горизонтально.
                // Я предлагаю сначала проверить горизонтальное расстояние, а затем высоту.
                Vector3 currentPosFlat = new Vector3(currentPosition.x, 0, currentPosition.z);
                Vector3 pointFlat = new Vector3(point.x, 0, point.z);
            
                // Если горизонтальное расстояние до точки пути достаточно мало
                if (Vector3.Distance(currentPosFlat, pointFlat) < 0.15f) // Порог можно настроить
                {
                    float heightDifference = point.y - currentPosition.y;
            
                    // Если точка выше и агент на земле, и разница в высоте допустима для прыжка
                    if (controller.isGrounded && heightDifference > controller.stepOffset + 0.1f && heightDifference <= maxJumpHeight)
                    {
                        if (_enemy != null) { _enemy.Jump(); } // Предполагается, что Jump инициирует движение вверх
                        Debug.Log("Jump condition met for point " + point + ". Height diff: " + heightDifference);
                    }
                    // Если агент достиг точки (и по горизонтали, и по вертикали достаточно близко)
                    if (Mathf.Abs(heightDifference) < 0.2f || currentPosition.y > point.y) // Можно также добавить условие, если агент перепрыгнул точку
                    {
                        currentIndex++;
                        Debug.Log("Reached point " + point + ". Moving to next index: " + currentIndex);
                        return;
                    }
                }
            
                Vector3 horizontalDir = pointFlat - currentPosFlat;
                Vector3 horizontalMovement = Vector3.zero;
            
                if (horizontalDir.magnitude > 0.05f) // Горизонтальное движение, если не слишком близко
                {
                    horizontalMovement = horizontalDir.normalized * (speed * Time.deltaTime);
                }
                
                // Обработка гравитации или вертикального движения, если персонаж не на земле
                // Если CharacterController не обрабатывает это автоматически, вам нужно добавить это
                // Пример:
                /*if (!controller.isGrounded) {
                    verticalVelocity -= gravity * Time.deltaTime; // gravity - ваша константа
                } else {
                    verticalVelocity = -0.5f; // Небольшое отрицательное значение, чтобы CharacterController оставался "приземленным"
                }
                controller.Move(horizontalMovement + Vector3.up * verticalVelocity * Time.deltaTime);*/
            
                controller.Move(horizontalMovement); 
            }
        }

        public async Task FindPathAsync(Vector3 destination)
        {
            isCalculating = true;
            Vector3 start = transform.position;
            Vector3 end = destination;
            List<Vector3> resultPath = new();
            Debug.Log("AsyncPathfinder ");
        
            Vector3 current = start;
            resultPath.Add(current);
            
            int maxIterations = 50; // Увеличьте количество итераций, если пути могут быть длинными
            for (int i = 0; i < maxIterations; i++)
            {
                if (Vector3.Distance(new Vector3(current.x, 0, current.z), new Vector3(destination.x, 0, destination.z)) < checkStep)
                {
                    // Достаточно близко к цели по горизонтали
                    // Добавляем саму цель, если она достижима по высоте
                    if (Mathf.Abs(current.y - destination.y) < maxJumpHeight * 2 && CanStand(destination)) 
                    {
                        resultPath.Add(destination);
                        Debug.Log("Reached destination proximity. Final segment added.");
                    }
                    else
                    {
                        Debug.Log("Destination not reachable by height or cannot stand there.");
                    }
                    break;
                }
        
                Vector3 rayOrigin = current + Vector3.up * checkHeightOffset;
                Vector3 rayDirection = (destination - current).normalized;
                float rayDistance = Vector3.Distance(current, destination);
        
                RaycastHit hit;
                if (!Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance, obstacleMask))
                {
                    // Путь свободен, идем прямо к цели (или к ее проекции на землю)
                    resultPath.Add(destination);
                    Debug.Log("No obstacle found. Path completed.");
                    break;
                }
                else
                {
                    Debug.Log("Obstacle found at: " + hit.point + " on collider: " + hit.collider.name);
        
                    // Попытка обойти препятствие по сторонам
                    Vector3 hitToCurrentDir = (new Vector3(current.x, 0, 0) - new Vector3(hit.point.x, 0 ,0)).normalized;
                    Vector3 perpendicularDir = Vector3.Cross(hitToCurrentDir, Vector3.up).normalized;
        
                    Vector3 evadePoint1 = new Vector3(hit.point.x, current.y)  + /*perpendicularDir*/hitToCurrentDir * (controller.radius /* + 0.5f*//* *1.5f*/); // Отступаем немного в сторону
                    Vector3 evadePoint2 = new Vector3(hit.point.x, current.y) - /*perpendicularDir*/hitToCurrentDir * (controller.radius /* + 0.5f*//* *1.5f*/);
                    EvadePoint1=evadePoint1;
                    EvadePoint2=evadePoint2;
                    // Выбираем лучшую точку обхода
                    Vector3 bestEvadePoint = Vector3.zero;
                    bool evadePointFound = false;
        
                    // Проверяем, можно ли дойти до этих точек и стоять на них
                    if (CanStand(evadePoint1) && !Physics.Raycast(current + Vector3.up * checkHeightOffset, (evadePoint1 - current).normalized, Vector3.Distance(current, evadePoint1), obstacleMask))
                    {
                        bestEvadePoint = evadePoint1;
                        evadePointFound = true;
                    }
                    else if (CanStand(evadePoint2) && !Physics.Raycast(current + Vector3.up * checkHeightOffset, (evadePoint2 - current).normalized, Vector3.Distance(current, evadePoint2), obstacleMask))
                    {
                        bestEvadePoint = evadePoint2;
                        evadePointFound = true;
                        
                    }
        
                    if (evadePointFound)
                    {
                        // Добавляем точку обхода
                        resultPath.Add(bestEvadePoint);
                        current = bestEvadePoint;
                        Jump(hit, bestEvadePoint, ref current, ref resultPath, rayDirection, end, out bool slide);
                        Debug.Log("Evading obstacle to: " + bestEvadePoint);
                    }
                    else
                    {
                        Debug.LogWarning("Could not find a suitable evasion point around the obstacle. Pathfinding may be stuck.");
                        break; // Не можем обойти, прекращаем поиск пути
                    }
                }
                await Task.Yield(); // Чтобы не блокировать основной поток
            }
        
            currentPath = resultPath;
            currentIndex = 0;
            isCalculating = false;
            Debug.Log("Pathfinding finished. Path length: " + currentPath.Count);
        }

        bool Jump(RaycastHit hit, Vector3 evasionPoint, ref Vector3 current, ref List<Vector3> resultPath, Vector3 rayDirection, Vector3 end, out bool slide)
        {
            Collider hitCollider = hit.collider;

            Bounds bounds = hitCollider.bounds;


            var targetPoint = new Vector3(evasionPoint.x, bounds.max.y + offset, evasionPoint.z);


            float distanceToJumpTarget = Vector3.Distance(evasionPoint, targetPoint);

            if (distanceToJumpTarget <= jumpDistance)
            {

                current = targetPoint;

                resultPath.Add(targetPoint);

                Debug.Log("Jumping over obstacle to: " + targetPoint);
                slide = Slide(ref current, ref resultPath, rayDirection, end, targetPoint, bounds);
                return true;
            }
            slide = false;
            return false;
        }

        bool Slide(ref Vector3 current, ref List<Vector3> resultPath, Vector3 rayDirection, Vector3 end, Vector3 targetPoint, Bounds bounds)
        {
            
                Vector3 slideEndPoint = targetPoint;
                
                    if (rayDirection.x > 0)

                    {

                        float targetX = (targetPoint.x < bounds.max.x)
                            ? bounds.max.x + controller.radius
                            : targetPoint.x;

                        slideEndPoint = new Vector3(targetX, targetPoint.y, targetPoint.z);

                    }

                    else

                    {

                        float targetX = (targetPoint.x > bounds.min.x)
                            ? bounds.min.x - controller.radius
                            : targetPoint.x;

                        slideEndPoint = new Vector3(targetX, targetPoint.y, targetPoint.z);

                    }
                if (Vector3.Distance(current, slideEndPoint) < Vector3.Distance(current, end))
                {

                }
                
                Vector3 finalSlidePoint = slideEndPoint;
                

                if ((rayDirection.x > 0 && end.x < finalSlidePoint.x) ||
                    (rayDirection.x < 0 && end.x > finalSlidePoint.x))
                {               
                    // Если цель находится между targetPoint и slideEndPoint по X               
                    if (Mathf.Abs(end.y - targetPoint.y) < 0.5f) // Если цель примерно на той же высоте             
                    {               
                        finalSlidePoint = new Vector3(end.x, targetPoint.y, targetPoint.z);             
                    }               
                }


                Debug.DrawLine(current, finalSlidePoint, Color.blue, 1f); // Синяя линия - путь скольжения

                resultPath.Add(finalSlidePoint);

                current = finalSlidePoint; // Обновляем текущую позицию на конец скольжения

                Debug.Log("Sliding along obstacle to bounds edge: " + finalSlidePoint);
                
                return false;
                
        }

        bool CanJump(Vector3 startPoint, Vector3 point)
        {
            if (startPoint.y < point.y && Vector3.Distance(startPoint, point) <= jumpDistance)
            {
                return true;
            }
            return false;
        }
        bool CanStand(Vector3 point)
        {
            bool hasGround = Physics.Raycast(point + Vector3.up * 0.5f, Vector3.down, out _, 1.5f, walkableMask);
            Debug.DrawLine(point + Vector3.up * 0.5f, Vector3.down*1f+point, Color.green);
            Debug.Log(point);
            return hasGround;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(spherePos, 0.2f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(EvadePoint1, 0.2f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(EvadePoint2, 0.2f);
            if (currentPath != null && currentPath.Count > 1)
            {
                for (int i = 0; i < currentPath.Count - 1; i++)
                {
                    Gizmos.color = Color.red; // Цвет для пути
                    Gizmos.DrawLine(currentPath[i], currentPath[i + 1]);
                    Gizmos.DrawWireSphere(currentPath[i], 0.1f); // Отображение каждой точки
                }
                Gizmos.DrawWireSphere(currentPath[currentPath.Count - 1], 0.1f);
            }
        }
    }
}