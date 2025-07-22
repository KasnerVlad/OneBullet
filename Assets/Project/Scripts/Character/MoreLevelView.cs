using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Scripts.Character
{
    public class MoreLevelView : MonoBehaviour
    {
        [SerializeField] private List<Camera> cameras;
        [SerializeField] private float viewSize;
        [SerializeField] private float normalSize;
        [SerializeField] private float smoothTime;
        private Dictionary<Camera, float> camerasSizes = new Dictionary<Camera, float>();
        private List<float> dampsVelocities = new List<float>();

        private void Awake()
        {
            Init();
            InputManager.playerInput.UI.ViewMoreLevel.performed += e => SetCamsSize(viewSize);
            InputManager.playerInput.UI.ViewMoreLevel.canceled += e => SetCamsSize(normalSize);
            InputManager.playerInput.Enable();
            
        }

        private void Init()
        {
            for (int i = 0; i < cameras.Count; i++)
            {
                dampsVelocities.Add(0);
            }    
        }

        private void SetCamsSize(float size)
        {
            camerasSizes = new Dictionary<Camera, float>();
            foreach (var camera in cameras)
            {
                camerasSizes.Add(camera,size);
                Debug.Log(camera.name +" " + camerasSizes[camera]);
            }
        }

        private void Update()
        {
            for (int i = 0; i < camerasSizes.Count; i++)
            {
                Debug.Log(cameras[i].name);
                float nowDamps = dampsVelocities[i];
                cameras[i].orthographicSize = Mathf.SmoothDamp(cameras[i].orthographicSize, camerasSizes[cameras[i]],
                    ref nowDamps, smoothTime);
                dampsVelocities[i] = nowDamps;
            }

        }
    }
}