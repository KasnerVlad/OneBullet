using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Enums;
using Project.Scripts.Reflector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Scripts
{
    [RequireComponent(typeof(LineRenderer))]
    public class BulletPathPreview : MonoBehaviour
    {
        [SerializeField]private int maxBounces = 5;
        [SerializeField]private float maxDistance = 100f;
        [SerializeField]private LayerMask reflectorMask;
        [SerializeField]private LayerMask notIgnoreMask;
        private Dictionary<Vector3, ReflectorType> points;
        private LineRenderer lineRenderer;
        
        void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        void Update()
        {
            Vector3 startPosition = transform.position;
            Vector3 direction = transform.right;

            points = new Dictionary<Vector3, ReflectorType>();
            points.Add(startPosition, ReflectorType.None);

            for (int i = 0; i < maxBounces; i++)
            {
                Ray ray = new Ray(startPosition, direction);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, maxDistance, notIgnoreMask))
                {
                    RaycastHit rhit;
                    ReflectorTypeSet reflectorTypeSet = hit.collider.gameObject.GetComponent<ReflectorTypeSet>();
                    if (reflectorTypeSet != null) points.Add(hit.point, reflectorTypeSet.reflectorType);
                    else points.Add(hit.point, ReflectorType.None);
                    direction = Vector3.Reflect(direction, hit.normal);
                    startPosition = hit.point + direction * 0.01f; 
                    if (Physics.Raycast(ray, out rhit, maxDistance, reflectorMask))
                    {
                        if (rhit.point != hit.point)
                            break;
                    }
                    else
                    {
                        break;
                    }

                }
                else
                {
                    points.Add(startPosition + direction * maxDistance, ReflectorType.None);
                    break;
                }
            }

            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.Keys.ToArray());
        }

        public Dictionary<Vector3, ReflectorType> GetPoints()
        {
            return points;
        }
    }
}