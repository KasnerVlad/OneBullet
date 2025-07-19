using Project.Scripts.Enums;
using UnityEngine;

namespace Project.Scripts.Character
{
    public class CamManager : MonoBehaviour
    {
        [SerializeField]private Camera allLevelCamera;
        [SerializeField]private Camera editModeCamera;
        [SerializeField]private Camera shootCamera;
        private bool _isSeeAllLevel;

        private void Awake()
        {
            
        }
        private void Update()
        {
            if (_isSeeAllLevel)
            {
                allLevelCamera.enabled = true;
                editModeCamera.enabled = false;
                shootCamera.enabled = false;
            }
            else
            {
                if (ModeManager.Instance.nowMode == Mode.EditMode)
                {
                    /*allLevelCamera.enabled = false;
                    editModeCamera.enabled = true;
                    shootCamera.enabled = false;*/
                } 
                else if (ModeManager.Instance.nowMode == Mode.ShootMode)
                {
                    allLevelCamera.enabled = false;
                    editModeCamera.enabled = false;
                    shootCamera.enabled = true;
                }
            }
        }
    }
}