using UnityEngine;

namespace Project.Scripts
{
    public class LockObjectRot : MonoBehaviour
    {
        [SerializeField] private Vector3 rotation;
        [SerializeField] private Transform target;
        [SerializeField]private bool isTransform;
        [SerializeField]private bool lockRot;
        [SerializeField]private bool lockPos;
        private void Update()
        {
            if (isTransform && lockRot)
            {
                transform.rotation = target.rotation;
            }
            else if(lockRot)
            {
                transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
            }

            if (lockPos)
            {
                transform.position = target.position;
            }
        }
    }
}