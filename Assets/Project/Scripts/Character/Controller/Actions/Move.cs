using System;

namespace Player.Character
{
    using UnityEngine;
    using static Models;
    using Custom;
    public class Move : IMove
    {
        #region Jumping
        private readonly CharacterController characterController;
        private readonly PlayerSettings.Jump jumpSettings;
        private Vector3 jumpingForceVelocity;
        private Vector3 jumpForce;
        private float _playerGravity;
        #endregion
        #region Movement
        private readonly PlayerSettings.MovementSettings movementSettings;
        private readonly Transform _player;
        private Vector3 newMovementSpeedVector;
        private Vector3 moveVelocity;
        private float targetRotation;
        private float targetRotationVelocity;
        #endregion
        public Move(PlayerSettings.Jump _jumpSettings,
            PlayerSettings.MovementSettings _movementSettings,
            Transform player,
            CharacterController _characterController)
        {
            jumpSettings = _jumpSettings;
            movementSettings = _movementSettings;
            _player = player;
            characterController = _characterController;
        }
        #region Jumping
        public void CalculateJump(bool isJumping)
        {
            Jump(isJumping);
            jumpForce = Vector3.SmoothDamp(jumpForce, Vector3.zero, ref jumpingForceVelocity, jumpSettings.JumpFallOff);
        }

        private void Jump(bool isJumping)
        {
            if ((characterController.isGrounded) && isJumping)
            {           
                jumpForce = Vector3.up * jumpSettings.JumpHeight;
                _playerGravity = 0; 
            }
        }
        #endregion
        #region Movement
        public void CalculateMovement(bool isSprinting, Vector2 movement, ref Vector3 currentSpeed)
        {
            var targetHorizontalSpeed = (!isSprinting?movementSettings.WalkingStrafeSpeed:movementSettings.RunningStrafeSpeed)*CTime.timeScale;
            
            /*var targetVerticalSpeed =movement.y<0?
                !isSprinting?movementSettings.WalkingBackwardSpeed:movementSettings.RunningBackwardSpeed:
                !isSprinting?movementSettings.WalkingForwardSpeed:movementSettings.RunningForwardSpeed;*/
            
            var horizontal = movement.x*Time.deltaTime*targetHorizontalSpeed;/*
            var vertical = movement.y*Time.deltaTime*targetVerticalSpeed;*/
            
            if (!characterController.isGrounded)
            {
                horizontal *= movementSettings.airControlMultiplier;/*
                vertical *= movementSettings.airControlMultiplier;*/
            }
            float targetRot = horizontal>0?-90:horizontal<0?90:0;
            targetRotation=Mathf.SmoothDamp(targetRotation, targetRot, ref targetRotationVelocity, characterController.isGrounded?movementSettings.moveSmoothing:movementSettings.fallingSmoothing );
            newMovementSpeedVector=Vector3.SmoothDamp(newMovementSpeedVector,
                new Vector3(horizontal,0,/*vertical*/0), 
                ref moveVelocity, 
                (characterController.isGrounded?movementSettings.moveSmoothing:movementSettings.fallingSmoothing )) ;
            
            characterController.transform.rotation = Quaternion.Euler(new Vector3(0, targetRotation, 0));
            var movementSpeedVector = /*_player.TransformDirection(*/newMovementSpeedVector/*)*/;
            
            if (_playerGravity > jumpSettings.gravityMin) { _playerGravity-=jumpSettings.gravity*Time.deltaTime; }
            
            if (_playerGravity < jumpSettings.gravityMinOnGround && (characterController.isGrounded)) { _playerGravity = jumpSettings.gravityMinOnGround; }

            
            movementSpeedVector.y += _playerGravity*CTime.timeScale;
            movementSpeedVector += jumpForce *Time.deltaTime;
            
            characterController.Move(movementSpeedVector);
            var characterVelocityInverseTransformDirection = _player.InverseTransformDirection(characterController.velocity);
            
            currentSpeed=new Vector3(characterVelocityInverseTransformDirection.x,
                characterController.velocity.y,
                characterVelocityInverseTransformDirection.z);
        }
        #endregion
    }
}