using Project.Scripts.Enums;
using UnityEngine;

namespace Project.Scripts.Character
{
    public class ModeManager : MonoBehaviour
    {
        public static ModeManager Instance;
        [HideInInspector]
        public Mode nowMode;

        private void Awake()
        {
            Instance = this;
            InputManager.playerInput.UI.ChangeMode.performed += _ => ChangeMode();
            InputManager.playerInput.Enable();
        }

        private void ChangeMode()
        {
            int mode = (int)nowMode;
            mode++;
            int maxEnumValue = System.Enum.GetValues(typeof(Mode)).Length - 1;
            
            if (mode > maxEnumValue)
            {
                mode = 0;
            }
            nowMode = (Mode)mode;
        }
        
    }
}