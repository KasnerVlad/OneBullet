using Custom;
using Project.Scripts.Character.Controller;
using Project.Scripts.Enemy.CustomAIAgent;
using UnityEngine;
using UnityEngine.AI;

namespace Project.Scripts.Enemy
{
    public class Enemy : MonoBehaviour
    {
[Header("Обнаружение цели")]
    [SerializeField] private LayerMask targetLayer;          // Слой, на котором находится цель (например, "Player")
    [SerializeField] private LayerMask obstacleLayer;        // Слои, которые будут блокировать видимость (например, "Default", "Environment")
    [SerializeField] private float fieldOfViewAngle = 90f;   // Угол обзора врага в градусах
    [SerializeField] private float viewDistance = 10f;       // Максимальная дистанция, на которой враг может видеть цель

    // --- Настройки для преследования ---
    [Header("Преследование")]
    [SerializeField] private float stopDistance = 1.5f;      // Дистанция, на которой враг остановится перед целью
    [SerializeField] private float patrolSpeed = 2f;         // Скорость врага в режиме патрулирования/покоя
    [SerializeField] private float chaseSpeed = 4f;          // Скорость врага при преследовании

    [SerializeField] private float maxDistanceSurface;
    // --- Приватные переменные ---
    private AsyncPathfinderAI agent;
    private CharacterController characterController;
    [SerializeField]private GameObject currentTarget;                // Текущая обнаруженная цель
    [SerializeField]private bool isChasing = false;                  // Флаг, указывающий, преследует ли враг цель
    /*private bool isAgentActive = false;          */    // Флаг, указывающий, активирован ли NavMeshAgent и находится ли он на NavMesh

    [Header("EnemyManager")]
    [SerializeField] private EnemyManager _enemyManager;

    // Для движения CharacterController (NavMeshAgent не управляет напрямую)
    private Vector3 velocity; // Текущая скорость для Character Controller
    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f; // Гравитация (отрицательное значение)
    [SerializeField] private float gravityMin = -30f; // Минимальная скорость падения
    [SerializeField] private float gravityMinOnGround = -0.5f; // Небольшая отрицательная скорость, чтобы "прижать" к земле
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 10f;
    private BoxCollider collider;
    private Vector3 jumpingForceVelocity;
    private Vector3 jumpForce;
    [SerializeField]private float JumpFallOff;
    [SerializeField]private float JumpHeight;
    private Vector3 lastPos;
    private Vector3 lastTargetPos;
    private float _playerGravity;
    void Awake()
    {
        agent = GetComponent<AsyncPathfinderAI>();
        characterController = GetComponent<CharacterController>();
        collider = GetComponent<BoxCollider>();
        // Изначально отключаем NavMeshAgent, чтобы персонаж мог упасть
        if (agent != null)
        {
            
            Debug.Log("agent not null ");
            /*agent.enabled = false; */// Отключаем компонент NavMeshAgent
            /*agent.updatePosition = false; // Отключаем автоматическое обновление позиции
            agent.updateRotation = false;*/ // Отключаем автоматическое обновление вращения
        }
    }

    void FixedUpdate()
    {
        if (!_enemyManager.IsPlayerControl)
        {
            CheckTriggers();

            // Логика AI только если NavMeshAgent активен и на NavMesh
            if (agent != null)
            {
                if (isChasing && currentTarget != null)
                {
                    /*if (CanSeeTarget(currentTarget.transform))
                    {*/
                        agent.speed = chaseSpeed;

                        // Если дистанция до цели достаточно большая, чтобы двигаться
                        if (Vector3.Distance(transform.position, currentTarget.transform.position) > stopDistance)
                        {
                            
                            if (lastPos == transform.position || lastTargetPos != currentTarget.transform.position)
                            {
                                lastPos = transform.position;
                                lastTargetPos = currentTarget.transform.position;
                                agent.isStopped = false; // Убеждаемся, что агент может двигаться
                                agent.SetDestination(currentTarget.transform.position);
                            }
                        }
                        else // Если мы достаточно близко к цели
                        {
                            agent.isStopped = true;
                            // Останавливаем NavMeshAgent
                            // Возможно, здесь можно сбросить путь агента, чтобы он не пытался дергаться на месте
                            // agent.ResetPath(); 
                        }/*
                    }*/
                    /*else
                    {
                        // Цель потеряна из виду, прекращаем преследование
                        Debug.Log("Цель " + currentTarget.name + " потеряна из виду!");
                        StopChasing();
                    }*/
                }                
                /*else // Враг не преследует или нет текущей цели
                {
                    /*agent.speed = patrolSpeed;
                    if (agent.isOnNavMesh && (!agent.hasPath || agent.remainingDistance < 0.1f)) // Добавлена проверка agent.isOnNavMesh
                    {
                        agent.isStopped = true;
                    }
                    else if (!agent.isStopped) // Если агент должен двигаться по патрулю, но еще не достиг
                    {
                        // Можно добавить логику патрулирования сюда
                    }#1#
                }*/

            }
            HandleMovementAndSync();
        }
    }
    void StartChasing(GameObject target)
    {
        // Начинаем преследование только если агент активен и на NavMesh
        if (target != null && !isChasing && agent != null)
        {
            Debug.Log("Начато преследование цели: " + target.name);
            currentTarget = target;
            isChasing = true;
            agent.isStopped = false;
        }
    }

    void StopChasing()
    {
        if (isChasing)
        {
            Debug.Log("Преследование прекращено.");
            isChasing = false;
            currentTarget = null;
        
            if (agent != null)
            {
                agent.isStopped = true;
            }
        }
    }

    public void Jump()
    {
        if (characterController.isGrounded)
        {
            jumpForce = Vector3.up * JumpHeight;
            _playerGravity = 0; 
        }
    }
    private void CheckTriggers()
    {
        if (currentTarget == null)
        {
            Debug.Log("target is null");
            StopChasing();
        }
        
        if (AllEnemyController.Instance != null && AllEnemyController.Instance.PlayerControllerEnemy != null)
        {
            
            Collider[] colliders = Physics.OverlapBox(transform.position,Vector3.Scale(collider.bounds.extents, transform.lossyScale) ,transform.rotation);
            Debug.Log("Player control not null");
            bool isPlayer = false;
            foreach (Collider other in colliders)
            {
                if (other.gameObject == AllEnemyController.Instance.PlayerControllerEnemy)
                {
                    
                    isPlayer=true;
                    if (!isChasing)
                    {
                        /*if (CanSeeTarget(other.transform))
                        {*/
                        StartChasing(AllEnemyController.Instance.PlayerControllerEnemy);
                        break;
                        /*
                    }*/
                    }
                }

            }
            if (isChasing && currentTarget != null && !isPlayer)
            {
                Debug.Log("Цель " + currentTarget.name + " вышла из зоны обнаружения.");

                StopChasing();
            }
        }
        else
        {
            Debug.Log("Player control is null");
        }
    }

    void HandleMovementAndSync()
    {
        if (characterController == null) return;

        
        Vector3 movementSpeedVector = Vector3.zero;
        jumpForce = Vector3.SmoothDamp(jumpForce, Vector3.zero, ref jumpingForceVelocity, JumpFallOff);
        if (characterController.isGrounded)
        {
            _playerGravity = gravityMinOnGround;
        }
        if (_playerGravity > gravityMin)
        {
            _playerGravity-=gravity*Time.deltaTime;
        }

        if (_playerGravity < gravityMinOnGround && (characterController.isGrounded))
        {
            _playerGravity = gravityMinOnGround;
        }

        
        movementSpeedVector.y += _playerGravity*CTime.timeScale;
        movementSpeedVector += jumpForce *Time.deltaTime;
        
        velocity.y *= CTime.timeScale;
        
        characterController.Move(movementSpeedVector);
        
    }
        public void OnDeath()
        {
            Destroy(gameObject);
        }
    }
}