using System;
using System.Globalization;
using Custom;
using Project.Scripts.Character;
using Project.Scripts.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Scripts.Reflector
{
    public class ReflectorRotator : MonoBehaviour , IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private GameObject _reflector;
        private Quaternion startOffset;
        private TMP_InputField _inputField;
        private bool init;

        private BoxCollider _childBoxCollider; 
        private Transform _childTransform; 

        public LayerMask ignoreLayer; 

        private Quaternion _lastValidRotation;
        private Camera _editCamera;
        
        private Vector3 _lastMousePosition;
        private Vector3 ReflectorCenterPosition=>_reflector.transform.position;
        private StringEffect _effect;
        public void Initialize(GameObject reflector, TMP_InputField inputField, Camera editCamera, StringEffect stringEffect)
        {
            _reflector = reflector;
            _inputField = inputField;
            _editCamera = editCamera;
            _effect = stringEffect;
            init = true;
            _inputField.text = (_reflector.transform.rotation.ToEuler().z * Mathf.Rad2Deg).ToString(CultureInfo.InvariantCulture);
            
            if (_inputField != null) 
                _inputField.onEndEdit.AddListener(str => { 
                    Quaternion targetRot = Quaternion.Euler(0, 0, Helper.GetFloatFromInputField(_inputField));
                    if (CanRotateTo(targetRot))
                    {
                        _reflector.transform.rotation = targetRot;
                        transform.rotation = targetRot;
                        _lastValidRotation = targetRot;
                    }
                    else
                    {
                        _inputField.text = (_lastValidRotation.ToEuler().z * Mathf.Rad2Deg).ToString(CultureInfo.InvariantCulture);
                        Debug.Log("Manual rotation blocked due to collision!");
                    }
                });

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
            
            /*
            ignoreLayer = ~(_reflector.layer); 
            */
            
            _lastValidRotation = _reflector.transform.localRotation; 
        }

        [Obsolete("Obsolete")]
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (init)
            {
                startOffset = _reflector.transform.localRotation;
                _lastValidRotation = _reflector.transform.localRotation;
                Vector3 mouseScreenPosition = Input.mousePosition;
                mouseScreenPosition.z = transform.position.z - _editCamera.transform.position.z;
                Vector3 mouseWorldPosition = _editCamera.ScreenToWorldPoint(mouseScreenPosition);
                _lastMousePosition = mouseWorldPosition;
                _effect.SetAll(ReflectorCenterPosition, _lastMousePosition, _reflector.transform);
                
                _effect.SetShow(true);
                OnDrag(eventData);
            }
        }

        [Obsolete("Obsolete")]
        public void OnDrag(PointerEventData eventData)
        {            
            if (_editCamera != null && init && ModeManager.Instance.nowMode == Mode.EditMode && !_inputField.isFocused)
            {
                Vector3 mouseScreenPosition = Input.mousePosition;
                mouseScreenPosition.z = transform.position.z - _editCamera.transform.position.z;
                Vector3 mouseWorldPosition = _editCamera.ScreenToWorldPoint(mouseScreenPosition);

                Vector3 direction = mouseWorldPosition - transform.position;

                float angleRad = Mathf.Atan2(direction.y, direction.x);
                float angleDeg = angleRad * Mathf.Rad2Deg;

                Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angleDeg) + (Quaternion.ToEulerAngles(startOffset)* Mathf.Rad2Deg));

                if (CanRotateTo(targetRotation))
                {
                    _reflector.transform.localRotation = targetRotation;
                    transform.localRotation = targetRotation;
                    _lastValidRotation = targetRotation; 
                    _inputField.text = (_reflector.transform.localRotation.ToEuler().z * Mathf.Rad2Deg).ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    Debug.Log("Rotation blocked by drag due to collision!");
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _effect.SetShow(false);
        }
        private bool CanRotateTo(Quaternion targetRotation)
        {
            if (_childBoxCollider == null) return true;

            Quaternion originalReflectorRotation = _reflector.transform.rotation;
            Quaternion originalChildLocalRotation = _childTransform.localRotation;
            Vector3 originalChildLocalPosition = _childTransform.localPosition;

            _reflector.transform.rotation = targetRotation;
            
            Vector3 worldHalfExtents = Vector3.Scale(_childBoxCollider.size * 0.5f, _childTransform.lossyScale); 

            Collider[] hitColliders = Physics.OverlapBox(_childTransform.position, worldHalfExtents, _childTransform.rotation, ignoreLayer);

            // Возвращаем _reflector и _childTransform в исходное состояние
            _reflector.transform.rotation = originalReflectorRotation;
            _childTransform.localPosition = originalChildLocalPosition;
            _childTransform.localRotation = originalChildLocalRotation;

            foreach (Collider hitCollider in hitColliders)
            {
                Debug.Log(hitCollider.gameObject.name);
                if (hitCollider.gameObject != _reflector&&hitCollider.gameObject != _childTransform.gameObject) 
                {
                    return false;
                }
            }

            return true;
        }

        private void Update()
        {
            if(init)transform.parent.position = new Vector3(_reflector.transform.position.x, _reflector.transform.position.y, transform.position.z);
        }

        /*private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Color originalGizmoColor = Gizmos.color;
            Matrix4x4 originalGizmoMatrix = Gizmos.matrix;

            Vector3 worldHalfExtents = Vector3.Scale(_childBoxCollider.size * 0.5f, _childTransform.lossyScale); 
            Gizmos.color = Color.yellow; 

            Gizmos.matrix = Matrix4x4.TRS(_childTransform.position, _childTransform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, worldHalfExtents * 2);

            Gizmos.color = originalGizmoColor;
            Gizmos.matrix = originalGizmoMatrix;
        }*/
    }
}