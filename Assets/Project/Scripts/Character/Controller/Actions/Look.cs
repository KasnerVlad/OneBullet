namespace Player.Character
{
    using UnityEngine;
    using static Models;
    using Custom;
    public class Look : ILook
    {
        private readonly Transform _obj;
        private readonly Camera _cam;
        private readonly Vector3 _offset;
        private Vector3 tagetRot;
        private Vector3 tagetRotVelocity;
        private Vector3 rot;
        
        public Look(Transform obj, Camera camera, Vector3 offset)
        {
            _obj = obj;
            _cam = camera;
            _offset = offset;
        }
        public void CalculateView(Vector2 view,  float smoothTime)
        {/*
            _newCameraRotation.z += viewSettings.SensitivityY*Time.deltaTime*view.y*(viewSettings.YInvetred?-1f:1f);*/
            
            /*Vector3 direction = new Vector3(view.x, view.y, _obj.position.z) - _obj.position;

            if (direction.x != 0) // Чтобы не было ошибки при нулевом направлении
            {
                float angleZ = Mathf.Atan2(direction.y, direction.z) * Mathf.Rad2Deg;

                _newCameraRotation = new Vector3(0, 0, angleZ);
            }*/
            Vector3 mouseScreenPosition = view;
            mouseScreenPosition.z = _cam.WorldToScreenPoint(_obj.position).z;
        
            Vector3 mouseWorldPosition = _cam.ScreenToWorldPoint(mouseScreenPosition);

            Vector2 direction = mouseWorldPosition - _obj.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            tagetRot = new Vector3(0, 0, angle)+_offset;
            rot = Vector3.SmoothDamp(rot, tagetRot, ref tagetRotVelocity, smoothTime/CTime.timeScale);
            _obj.rotation = Quaternion.Euler(rot);
        }
    }
}