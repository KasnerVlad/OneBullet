using System;

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
        {
            Vector3 mouseScreenPosition = view;
            mouseScreenPosition.z = _cam.WorldToScreenPoint(_obj.position).z;
        
            Vector3 mouseWorldPosition = _cam.ScreenToWorldPoint(mouseScreenPosition);
            mouseWorldPosition.z = 0;
            Vector2 direction = mouseWorldPosition - _obj.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            tagetRot = new Vector3(0, 0, angle)+_offset;
            rot = Vector3.SmoothDamp(rot, tagetRot, ref tagetRotVelocity, smoothTime/CTime.timeScale);
            _obj.rotation = Quaternion.Euler(rot);
        }
    }
}