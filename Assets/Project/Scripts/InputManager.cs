using System;
using UnityEngine;

namespace Project.Scripts
{
    public class InputManager : MonoBehaviour
    {
        public static PlayerInput playerInput = new PlayerInput();

        private void Start()
        {
            playerInput.Enable();
        }
    }
}