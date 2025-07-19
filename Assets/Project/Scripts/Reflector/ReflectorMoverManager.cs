using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Reflector
{
    public class ReflectorMoverManager : MonoBehaviour 
    {
        [SerializeField] private List<GameObject> reflectors;
        /*[SerializeField] */private List<ReflectorRotator> rotators = new List<ReflectorRotator>();
        [SerializeField] private GameObject rotatorPrefab;
        private void Start()
        {
            foreach (var reflector in reflectors)
            {
                GameObject g = Instantiate(rotatorPrefab, transform);
                g.transform.position = reflector.transform.position;
                g.transform.rotation = reflector.transform.rotation;
                ReflectorRotator rotator = g.GetComponent<ReflectorRotator>();
                if (rotator != null)
                {
                    rotators.Add(rotator);
                    rotator.Initialize(reflector);
                }
            }
        }
    }
}