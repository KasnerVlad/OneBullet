using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Scripts
{
    public class InputFieldFocusChecker : MonoBehaviour
    {
        public static bool InputFieldFocused = false;
        void Update()
        {
            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

            if (currentSelected != null)
            {
                TMP_InputField focusedInputField = currentSelected.GetComponent<TMP_InputField>();

                if (focusedInputField != null)
                {
                    InputFieldFocused = true;
                }
                else
                {
                    InputFieldFocused = false;
                }

            }
            else
            {
                InputFieldFocused = false;
            }
        }
    }
}