using System;
using Project.Scripts.Character;
using Project.Scripts.Enums;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Scripts.Reflector
{
    public class ReflectorRotator : MonoBehaviour , IDragHandler, IBeginDragHandler
    {
        private GameObject _reflector;
        private Quaternion startOffset;
        private bool init;
        public void Initialize(GameObject reflector)
        {
            _reflector = reflector;
            init = true;
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            if(init) startOffset = _reflector.transform.localRotation;
        }
        [Obsolete("Obsolete")]
        public void OnDrag(PointerEventData eventData)
        {            
            if (Camera.main != null&&init&&ModeManager.Instance.nowMode==Mode.EditMode)
            {
                Vector3 mouseScreenPosition = Input.mousePosition;


                mouseScreenPosition.z = transform.position.z - Camera.main.transform.position.z;
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

                Vector3 direction = mouseWorldPosition - transform.position;

                float angleRad = Mathf.Atan2(direction.y, direction.x);
                float angleDeg = angleRad * Mathf.Rad2Deg;

                Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angleDeg) + Quaternion.ToEulerAngles(startOffset));

                Quaternion finalRotation = targetRotation;
                _reflector.transform.localRotation = finalRotation;
                transform.localRotation = finalRotation;
            }
        }

        private void Update()
        {
            if(init)transform.position = new Vector3(_reflector.transform.position.x, _reflector.transform.position.y, transform.position.z);
        }
    }
}