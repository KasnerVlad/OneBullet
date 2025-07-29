using UnityEngine;

namespace Project.Scripts
{
    public class TestScript : MonoBehaviour
    {
        void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 250, 60), "Change to scene2"))
            {
                Debug.Log("Exit1");
            }
        }
    }
}