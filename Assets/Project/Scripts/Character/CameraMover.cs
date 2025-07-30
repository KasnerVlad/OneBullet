using Project.Scripts.Enemy;
using UnityEngine;

namespace Project.Scripts.Character.Controller
{
    public class CameraMover : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D _boxCollider;
        [SerializeField] private BoxCollider2D _blockCollider;
        [SerializeField] private bool block;
        [SerializeField] private GameObject target;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float smooth;
        [SerializeField] private bool isNoPlayerChange;
        private Vector3 _velocity;
        public float boundaryOffset = 0.1f;    
    // Окончательные границы мира, за которые камера не должна выходить
    private Vector3 _minWorldCameraLimit;
    private Vector3 _maxWorldCameraLimit;

    private void Start()
    {
        if(!isNoPlayerChange) target = AllEnemyController.Instance.GetPlayerControl();
        if (_boxCollider == null || ( _blockCollider == null&&block) || target == null)
        {
            Debug.LogError("Один или несколько обязательных компонентов (BoxCollider2D камеры, BlockCollider2D, Target) не назначены!", this);
            enabled = false; // Отключаем скрипт, если компоненты не назначены
            return;
        }

        if(block) CalculateWorldCameraLimits();
    }

    private void CalculateWorldCameraLimits()
    {
        // Получаем половину размера коллайдера камеры
        Vector3 cameraHalfSize = _boxCollider.size / 2f;
        Bounds blockBounds = _blockCollider.bounds; // Границы внешнего блока

        // Вычисляем минимальные и максимальные мировые координаты,
        // где центр камеры может находиться, не выходя за _blockCollider.
        _minWorldCameraLimit.x = blockBounds.min.x + cameraHalfSize.x;
        _maxWorldCameraLimit.x = blockBounds.max.x - cameraHalfSize.x;

        _minWorldCameraLimit.y = blockBounds.min.y + cameraHalfSize.y;
        _maxWorldCameraLimit.y = blockBounds.max.y - cameraHalfSize.y;
    }

    private void FixedUpdate()
    {
        if (target == null || _boxCollider == null ||( _blockCollider == null&&block))
        {
            if(!isNoPlayerChange){
                target = AllEnemyController.Instance.GetPlayerControl();/*
                if(target == null) Destroy(gameObject);*/
            
            }
            return;
        }
        
        Vector3 playerPos = target.transform.position;
        Vector3 currentCameraPos = transform.position;
        Bounds cameraBounds = _boxCollider.bounds; // Текущие границы камеры

        Vector3 desiredCameraPos = currentCameraPos; // Изначально желаемая позиция = текущая

        // Проверяем, находится ли игрок за пределами мёртвой зоны камеры по X
        if (playerPos.x < cameraBounds.min.x + boundaryOffset)
        {
            desiredCameraPos.x = playerPos.x - _boxCollider.size.x / 2 + boundaryOffset;
        }
        else if (playerPos.x > cameraBounds.max.x - boundaryOffset)
        {
            desiredCameraPos.x = playerPos.x + _boxCollider.size.x / 2 - boundaryOffset;
        }

        // Проверяем, находится ли игрок за пределами мёртвой зоны камеры по Y
        if (playerPos.y < cameraBounds.min.y + boundaryOffset)
        {
            desiredCameraPos.y = playerPos.y - _boxCollider.size.y / 2 + boundaryOffset;
        }
        else if (playerPos.y > cameraBounds.max.y - boundaryOffset)
        {
            desiredCameraPos.y = playerPos.y + _boxCollider.size.y / 2 - boundaryOffset;
        }

        // --- Главное изменение: Применяем ограничение по _blockCollider ---
        // Обрезаем желаемую позицию камеры, чтобы она не выходила за _blockCollider
        if (block)
        {
            desiredCameraPos.x = Mathf.Clamp(desiredCameraPos.x, _minWorldCameraLimit.x, _maxWorldCameraLimit.x);
            desiredCameraPos.y = Mathf.Clamp(desiredCameraPos.y, _minWorldCameraLimit.y, _maxWorldCameraLimit.y);
            desiredCameraPos.z = transform.position.z; // Сохраняем Z-координату камеры
        }
        // Плавно перемещаем камеру к вычисленной желаемой позиции
        transform.position = Vector3.SmoothDamp(currentCameraPos, desiredCameraPos+offset, ref _velocity, smooth);
    }
    }
}