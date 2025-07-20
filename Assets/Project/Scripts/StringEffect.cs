using UnityEngine;

namespace Project.Scripts
{
    public class StringEffect : MonoBehaviour
    {
        [SerializeField] private GameObject target;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private GameObject targetPoint;
        private Vector3 position;
        private bool _show;
        private Transform parentObject;
        private Transform childObject; 
    
        private Vector3 initialLocalPosition;
        private Quaternion initialLocalRotation;
        public void SetShow(bool show)
        {
            _show = show;
        }
        public void SetAll(Vector3 pos, Vector3 mousePos, Transform parent)
        {
            position = pos;
            targetPoint.transform.position = mousePos;
            childObject=targetPoint.transform;
            parentObject = parent;
            initialLocalPosition = parentObject.InverseTransformPoint(childObject.position);
            initialLocalRotation = Quaternion.Inverse(parentObject.rotation) * childObject.rotation;
        }
        private void Update()
        {
           if(_show){ DoStringEffect(position); lineRenderer.enabled=true;}
           else lineRenderer.enabled=false;
        }
        private void DoStringEffect(Vector3 position)
        { 
            lineRenderer.positionCount = 3;
            lineRenderer.SetPosition(0, target.transform.position);
            lineRenderer.SetPosition(1, childObject.position);
            lineRenderer.SetPosition(2, position);
        }
    
        void LateUpdate()
        {
            if (parentObject == null || childObject == null)
            {
                return;
            }
            if (_show)
            {
                
  
                childObject.gameObject.SetActive(true);
                childObject.position = parentObject.TransformPoint(initialLocalPosition);
    
                childObject.rotation = parentObject.rotation * initialLocalRotation;
            }
            else
            {
                childObject.gameObject.SetActive(false);
            }

        }
    }
}