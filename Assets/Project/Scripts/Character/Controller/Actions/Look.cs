using System;
using Project.Scripts.SaveSystem;

namespace Player.Character
{
    using UnityEngine;
    using static Models;
    using Custom;
    public class Look : ILook
    {/*
        private readonly Transform _handRoot;
        
        private readonly Vector3 _handRootRotationOffset;*/
        private readonly Transform _aimObj;
        private readonly Camera _cam;
        private Vector3 _velocity;
        public Look(/*Transform handRoot, Vector3 handRootRotationOffset, */Transform aimObj, Camera camera)
        {/*
            _handRoot = handRoot;
            
            _handRootRotationOffset = handRootRotationOffset;*/
            _aimObj = aimObj;
            _cam = camera;
        }
        public void CalculateView(Vector2 view,  float smoothTime)
        {
            Vector3 mouseScreenPosition = view;
            mouseScreenPosition.z = _cam.WorldToScreenPoint(_aimObj.position).z;
            Vector3 mouseWorldPosition = _cam.ScreenToWorldPoint(mouseScreenPosition);
            mouseWorldPosition.z = 0;
            
            /*
            _handRoot.rotation = Quaternion.Euler(Helper.RotateTo<GameObject>(mouseWorldPosition, _handRoot.position, _handRootRotationOffset));*/
            _aimObj.position = Vector3.SmoothDamp(_aimObj.position, mouseWorldPosition, ref _velocity, smoothTime);
        }
    }
}