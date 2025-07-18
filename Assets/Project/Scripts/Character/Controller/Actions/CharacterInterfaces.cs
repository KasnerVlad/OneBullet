namespace Player.Character
{
    using UnityEngine;
    using static Models;
    public interface IMove
    {
        void CalculateJump(bool isJumping);

        void CalculateMovement(bool isSprinting, Vector2 movement,
            ref Vector3 currentSpeed);
    }
    public interface ILook
    {
        void CalculateView(Vector2 view,  float smoothTime);
    }
    /*public interface IStanceCalculate
    {
        void CalculateStance(PlayerStance currentPlayerStance, float playerStanceSmoothing);
        void Crouch(ref PlayerStance currentPlayerStance);
        void Prone(ref PlayerStance currentPlayerStance);

    }*/
}