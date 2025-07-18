/*namespace Player.Character
{
    using UnityEngine;
    using static Models;
    using Custom;
    public class StanceCalculation : IStanceCalculate
    {
        private CharacterStance _stand;
        private CharacterStance _crouch;
        private CharacterStance _prone;
        
        private Transform cameraHolder;
        
        private float cameraChangePosVelocity;
        private float colliderChangeRadiusVelocity;
        private float colliderChangeHeistVelocity;
        private Vector3 colliderChangeCenterVelocity;
        
        private float cameraHeight;
        
        private CharacterController characterController;

        private Rm<bool, float> CanStand;
        public StanceCalculation( CharacterStance stand, 
            CharacterStance crouch, 
            CharacterStance prone, 
            Transform _cameraHolder, 
            CharacterController c,
            Rm<bool, float> canStand)
        {
            _stand = stand;
            _crouch = crouch;
            _prone = prone;
            cameraHolder = _cameraHolder;
            characterController = c;
            CanStand = canStand;
        }
        public void CalculateStance(PlayerStance currentPlayerStance, float playerStanceSmoothing)
        {
            var stance = _stand;
            if (currentPlayerStance == PlayerStance.Crouch) { stance = _crouch; }
            else if (currentPlayerStance == PlayerStance.Prone) { stance = _prone; }

            cameraHeight=Mathf.SmoothDamp(cameraHolder.localPosition.y, stance.cameraHeist, ref cameraChangePosVelocity, playerStanceSmoothing);
            cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x, cameraHeight, cameraHolder.localPosition.z);
        
            characterController.radius = Mathf.SmoothDamp(characterController.radius,
                stance.Collider.radius,
                ref colliderChangeRadiusVelocity,
                playerStanceSmoothing);
        
            characterController.height = Mathf.SmoothDamp(characterController.height,
                stance.Collider.height,ref colliderChangeHeistVelocity,
                playerStanceSmoothing);
        
            characterController.center = Vector3.SmoothDamp(characterController.center,
                stance.Collider.center,ref colliderChangeCenterVelocity,
                playerStanceSmoothing);
        }
        
        public void Crouch(ref PlayerStance currentPlayerStance)
        {
            if (currentPlayerStance == PlayerStance.Crouch)
            {
                if(!CanStand.Invoke(_crouch.Collider.height)) currentPlayerStance = PlayerStance.Stand;
                return;
            }
            if(!CanStand(_prone.Collider.height)) {currentPlayerStance = PlayerStance.Crouch;}
        }
        public void Prone(ref PlayerStance currentPlayerStance)
        {
            if (currentPlayerStance == PlayerStance.Prone)
            {
                if(!CanStand.Invoke(_crouch.Collider.height))currentPlayerStance = PlayerStance.Stand;
                return;
            }
            currentPlayerStance = PlayerStance.Prone;
        }

    }
}*/