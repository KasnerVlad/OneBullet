using UnityEngine;

namespace Project.Scripts.Character.Controller
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField]private bool _isPlayerControl;
        [SerializeField] private Camera _camera;
        public bool IsPlayerControl=>_isPlayerControl;

        private void Awake()
        {
            if (_isPlayerControl)
            {
                if(_camera!=null) _camera.enabled = true;
            }
            else
            {
                if(_camera!=null)_camera.enabled = false;
            }
        }
        public void SetPlayerControl(bool isPlayerControl)
        {
            _isPlayerControl = isPlayerControl;
            if(_camera!=null)_camera.enabled = isPlayerControl;
        }
    }
}