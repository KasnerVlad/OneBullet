using System.Collections.Generic;
using Custom;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Scripts.Reflector
{
    public class ReflectorMover : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private GameObject _reflector;
        private Quaternion startOffset;
        private bool init;

        private BoxCollider _childBoxCollider; 
        private Transform _childTransform; 

        [SerializeField] private LayerMask ignoreLayer; 
        [SerializeField] private LayerMask reflectorLayer;
        private Vector3 _lastValidPosition;
        private Camera _editCamera;
        
        private Vector3 _lastMousePosition;
        private Vector3 ReflectorCenterPosition=>_reflector.transform.position;
        private StringEffect _effect;
        private ReflectorMoveSurfaces _surfaces;
        private SphereCollider mySphereCollider;
        public void Initialize(GameObject reflector, Camera editCamera, StringEffect stringEffect)
        {
            _reflector = reflector;
            _editCamera = editCamera;
            _effect = stringEffect;
            _surfaces = _reflector.GetComponent<ReflectorMoveSurfaces>();
            mySphereCollider = _reflector.GetComponent<SphereCollider>();
            init = true;
            if (_reflector.transform.childCount > 0)
            {
                _childTransform = _reflector.transform.GetChild(0);
                _childBoxCollider = _childTransform.GetComponent<BoxCollider>();
                if (_childBoxCollider == null)
                {
                    Debug.LogError("First child of _reflector must have a BoxCollider for collision blocking to work!");
                    enabled = false;
                    return;
                }
            }
            else
            {
                Debug.LogError("_reflector has no children. Rotation blocking won't work!");
                enabled = false;
                return;
            }
            
            _lastValidPosition = _reflector.transform.localPosition; 
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(!init){return;}
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = transform.position.z - _editCamera.transform.position.z;
            Vector3 mouseWorldPosition = _editCamera.ScreenToWorldPoint(mouseScreenPosition);
            _lastMousePosition = mouseWorldPosition;
            _lastValidPosition = _reflector.transform.position;
            _effect.SetAll(ReflectorCenterPosition, _lastMousePosition, _reflector.transform);
            _effect.SetShow(true);
        }


        public void OnDrag(PointerEventData eventData)
        {
            if(!init){return;}
            
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = transform.position.z - _editCamera.transform.position.z;
            Vector3 mouseWorldPosition = _editCamera.ScreenToWorldPoint(mouseScreenPosition);
            if (CanMoveTo(mouseWorldPosition))
            {
                _reflector.transform.position = mouseWorldPosition;
                transform.position = mouseWorldPosition;
            }
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            if(!init){return;}
            _effect.SetShow(false);
        }
        private bool CanMoveTo(Vector3 targetPos)
        {
            if (_childBoxCollider == null) return true;/*
            return true;*/
            Vector3 originalReflectorLocalPosition = _reflector.transform.localPosition;

            _reflector.transform.position = targetPos;
            
            Vector3 worldHalfExtents = Vector3.Scale(_childBoxCollider.size * 0.5f, _childTransform.lossyScale);
            
            float colliderRadius = mySphereCollider.radius;
            float maxScale = Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            float actualWorldRadius = colliderRadius * maxScale*Mathf.PI;
            
            Collider[] hitColliders = Physics.OverlapBox(_childTransform.position, worldHalfExtents, _childTransform.rotation, ignoreLayer);
            Collider[] rootHitColliders = Physics.OverlapSphere(_reflector.transform.position, actualWorldRadius, ignoreLayer);
            // Возвращаем _reflector и _childTransform в исходное состояние
            _reflector.transform.position = originalReflectorLocalPosition;

            foreach (Collider hitCollider in hitColliders)
            {
                Debug.Log(hitCollider.gameObject.name);
                if (hitCollider.gameObject != _reflector&&hitCollider.gameObject != _childTransform.gameObject) 
                {
                    return false;
                }
            }
            HashSet<GameObject> surfaceGameObjects = new HashSet<GameObject>();
            foreach (var obj in _surfaces.Surfaces)
            {
                if (obj != null)
                {
                    surfaceGameObjects.Add(obj.gameObject);
                }
            }

            foreach (Collider hitCollider in rootHitColliders)
            {
                if (hitCollider == null) continue;

                Debug.Log(hitCollider.gameObject.name);
                if (LayerMaskComparer.Equals2(hitCollider.gameObject.layer, reflectorLayer))
                {
                    return false;
                }
                if (surfaceGameObjects.Contains(hitCollider.gameObject))
                {
                    return true;
                }
            }

            return false;
        }
            // Структура для хранения информации о ближайшей точке
        private Vector3 GetMousePos()
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = transform.position.z - _editCamera.transform.position.z;
            Vector3 mouseWorldPosition = _editCamera.ScreenToWorldPoint(mouseScreenPosition);
            return mouseWorldPosition;
        }
        public struct ClosestPointInfo
        {
            public Vector3 point;       // Сама ближайшая точка в мировых координатах
            public float distance;      // Расстояние до этой точки
            public BoxCollider collider; // Коллайдер, на котором найдена точка
        }
    
        /// <summary>
        /// Находит ближайшую точку к текущей позиции мыши среди всех BoxCollider2D в _surfaces.
        /// </summary>
        /// <returns>Информация о ближайшей точке, или null, если коллайдеры не найдены.</returns>
        public ClosestPointInfo? FindClosestPointOnSurfacesToMouse()
        {
            if (_surfaces == null || _surfaces.Surfaces == null || _editCamera == null)
            {
                Debug.LogError("Ошибка: Отсутствует ссылка на Surfaces, _editCamera или коллекция Surfaces пуста.", this);
                return null;
            }
    
            // Получаем мировые координаты мыши
            Vector3 mouseWorldPosition = GetMousePos();
    
            ClosestPointInfo? closestInfo = null;
            float minDistanceSqr = float.MaxValue; // Используем квадрат расстояния для оптимизации (нет Math.Sqrt)
    
            // Перебираем все BoxCollider2D в коллекции
            foreach (BoxCollider surfaceCollider in _surfaces.Surfaces)
            {
                if (surfaceCollider == null) continue; // Пропускаем null-ссылки
    
                // Получаем ближайшую точку на КОЛЛАЙДЕРЕ к позиции мыши
                // (Не на Transform, а на границах коллайдера)
                Vector2 closestPointOnCollider = surfaceCollider.ClosestPoint(mouseWorldPosition);
    
                // Вычисляем квадрат расстояния от мыши до этой точки
                float currentDistanceSqr = (mouseWorldPosition - (Vector3)closestPointOnCollider).sqrMagnitude;
    
                // Если это расстояние меньше, чем текущее минимальное
                if (currentDistanceSqr < minDistanceSqr)
                {
                    minDistanceSqr = currentDistanceSqr; // Обновляем минимальное расстояние
                    
                    // Обновляем информацию о ближайшей точке
                    closestInfo = new ClosestPointInfo
                    {
                        point = closestPointOnCollider,
                        distance = Mathf.Sqrt(minDistanceSqr), // Вычисляем реальное расстояние только для результата
                        collider = surfaceCollider
                    };
                }
            }
    
            return closestInfo; // Возвращаем информацию о самой близкой точке (или null, если ничего не найдено)
        }
        private void OnDrawGizmos()
        {
            /*Color originalGizmoColor = Gizmos.color;
            Matrix4x4 originalGizmoMatrix = Gizmos.matrix;

            /*
            Vector3 worldHalfExtents = Vector3.Scale(_childBoxCollider.size * 0.5f, _childTransform.lossyScale); 
            Gizmos.color = Color.yellow; #1#
            float colliderRadius = mySphereCollider.radius;
            float maxScale = Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            float actualWorldRadius = colliderRadius * maxScale*Mathf.PI;
            Gizmos.matrix = Matrix4x4.TRS(_reflector.transform.position, _reflector.transform.rotation, Vector3.one);
            Gizmos.DrawSphere(Vector3.zero, actualWorldRadius);

            Gizmos.color = originalGizmoColor;
            Gizmos.matrix = originalGizmoMatrix;*/
        }
        /*
        private void Update()
        {
            if(init)transform.parent.position = new Vector3(_reflector.transform.position.x, _reflector.transform.position.y, transform.position.z);
        }*/
    }
}