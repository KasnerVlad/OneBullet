using System.Threading;
using UnityEngine;
using DG.Tweening;/*
using D;*/
namespace Project.Scripts
{
    public class StringEffect : MonoBehaviour
    {
        [SerializeField] private GameObject target;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private GameObject targetPoint;
        [SerializeField] private Vector3 targetPointMinScale;
        [SerializeField] private float dur;
        private Vector3 position;
        private bool _show;
        private Transform parentObject;
        private Transform childObject; 
    
        private Vector3 initialLocalPosition;
        private Quaternion initialLocalRotation;
        private Vector3 defaultTargetPointScale;
        private CancellationTokenSource _cts;
        private bool isStartDoTween;
        private void Awake()
        {
            defaultTargetPointScale = targetPoint.transform.localScale;
        }
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
            if(_cts!=null) _cts.Cancel();
            isStartDoTween = false;
            targetPoint.transform.localScale = defaultTargetPointScale;
        }
        private void Update()
        {
            
           if(_show){ DoStringEffect(position); lineRenderer.enabled=true;}
           else lineRenderer.enabled=false;

           if (!isStartDoTween)
           {
               targetPoint.transform.DOScale(targetPointMinScale, dur);
               isStartDoTween = true;
           }
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