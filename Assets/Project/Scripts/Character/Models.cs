
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;

public static class Models
{
    #region PlayerStance
    /*[Serializable]
    public class CharacterStance
    {
        public float cameraHeist;
        public CapsuleCollider Collider;
    }*/
    /*public enum PlayerStance
    {
        Stand,
        Crouch,
        Prone
    }*/
    #endregion
    #region Player
    [Serializable]
    public class PlayerSettings
    {
        [Space(10)]
        public MovementSettings movementSettings;
        [Space(10)]
        public Jump jumpSettings;
        [Serializable]
        public class MovementSettings
        {
            [Header("Movement Settings")] 
            public float WalkingStrafeSpeed;
            public float RunningStrafeSpeed;
            [Space(10)] public float airControlMultiplier;
            [Space(10)] [Header("Smoothing Settings")]
            public float moveSmoothing;
            public float fallingSmoothing;
        }
        [Serializable]
        public class Jump
        {
            [Header("Jump Settings")] 
            public float JumpFallOff;
            public float JumpHeight;
            public float cullDown;
            [Space(10)]
            [Header("Gravity")]
            public float gravity;
            public float gravityMin;
            public float gravityMinOnGround;
        }

    }

    #endregion
    /*#region WeaponSway
    [Serializable]
    public class GunSwaySettings
    {
        [Header("Sway")]
        public Vector3 ViewSwayAmount;
        public Vector3 MovementSwayAmount;
        [Space(10)]
        public bool ViewSwayXinverted;
        public bool ViewSwayYinverted;
        public bool ViewSwayZinverted;
        [Space(5)]
        public bool MovementSwayXinverted;
        public bool MovementSwayYinverted;
        public bool MovementSwayZinverted;
        [Space(10)]
        public float ViewSwayResetSmoothing;
        public float ViewSwaySmoothing;
        [Space(5)]
        public float MovementSwayResetSmoothing;
        public float MovementSwaySmoothing;
        [Space(5)]
        public float AimSmoothing;
        public float AimResetSmoothing;
        [Space(10)]
        public float ViewSwayClampX;
        public float ViewSwayClampY;
        public float ViewSwayClampZ;
        [Space(5)]
        public float MovementSwayClampX;
        public float MovementSwayClampY;
        public float MovementSwayClampZ;
    }
    #endregion*/
    #region WeaponAnimSettings
    /*[Serializable]
    public class GunAnimSettings
    {
        [Header("Anim Timing Settings")]
        [Tooltip("Type there animation time in seconds")]
        public float FireRate;
        [Tooltip("Type there animation time in seconds")]
        public float ReloadTime;
        [Tooltip("Type there animation time in seconds")]
        public float WatchTime;
        [Tooltip("Type there animation time in seconds")]
        public float TakeTime;
        [Tooltip("Type there animation time in seconds")]
        public float HideTime;
    }*/
    /*[Serializable]
    public class GunSettings
    {
        public float Range;
        public float Damage;
        public int _magazineSize;
    }*/
    #endregion
}
