using System.Threading.Tasks;
using Project.Scripts.Enums;
using Project.Scripts.Weapon;
using UnityEngine;

namespace Project.Scripts.Character
{
    public class CamManager : MonoBehaviour
    {
        [SerializeField] private Camera allLevelCamera;
        [SerializeField] private Camera editModeCamera;
        [SerializeField] private Camera shootCamera;
        [SerializeField] private Camera focusedCamera;
        [SerializeField] private Vector3 focusedCameraOffset;
        private bool _isSeeAllLevel;
        private bool Focusd;
        private void Awake()
        {

        }

        private void Update()
        {
            if (_isSeeAllLevel&&!Focusd)
            {
                allLevelCamera.enabled = true;
                editModeCamera.enabled = false;
                shootCamera.enabled = false;
                focusedCamera.enabled = false;
            }
            else
            {
                if (ModeManager.Instance.nowMode == Mode.EditMode&&!Focusd)
                {
                    allLevelCamera.enabled = false;
                    editModeCamera.enabled = true;
                    shootCamera.enabled = false;
                    focusedCamera.enabled = false;
                }
                else if (ModeManager.Instance.nowMode == Mode.ShootMode&&!Focusd)
                {
                    allLevelCamera.enabled = false;
                    editModeCamera.enabled = false;
                    shootCamera.enabled = true;
                    focusedCamera.enabled = false;
                }
            }
        }

        public void UnFocus()
        {
            Focusd = false;
            
            focusedCamera.enabled = false;
            allLevelCamera.enabled = false;
            editModeCamera.enabled = false;
            shootCamera.enabled = true;
        }
        public async Task FocusOnBullet(Bullet bullet)
        {
            Focusd = true;
            focusedCamera.enabled = true;
            allLevelCamera.enabled = false;
            editModeCamera.enabled = false;
            shootCamera.enabled = false;
            while (bullet._isShooting&&Application.isPlaying )
            {
                focusedCamera.transform.position = new Vector3( bullet.transform.position.x, bullet.transform.position.y, focusedCamera.transform.position.z )+focusedCameraOffset;
                await Task.Yield();
            }
            Focusd = false;
        }
    }  
}