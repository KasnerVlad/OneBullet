using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace Project.Scripts.Reflector
{
    public class ReflectorMoverManager : MonoBehaviour 
    {
        [SerializeField] private List<GameObject> reflectors;
        /*[SerializeField] */private List<ReflectorRotator> rotators = new List<ReflectorRotator>();
        private List<ReflectorMover> movers = new List<ReflectorMover>();
        [SerializeField] private GameObject rotatorPrefab;
        [SerializeField] private Camera editCamera;
        [SerializeField] private StringEffect stringEffect;
        private void Start()
        {
            foreach (var reflector in reflectors)
            {
                GameObject g = Instantiate(rotatorPrefab, transform);
                g.transform.position = new Vector3(reflector.transform.position.x, reflector.transform.position.y, transform.position.z);
                
                TMP_InputField inputField=null;
                ReflectorRotator rotator = null;
                ReflectorMover mover = null;
                foreach (Transform child in g.transform)
                {
                    if(rotator==null){rotator = child.GetComponent<ReflectorRotator>();  }
                    if(inputField==null){ inputField = child.GetComponent<TMP_InputField>();}
                    if(mover==null){mover = child.GetComponent<ReflectorMover>();}
                }
                if (rotator != null&&inputField!=null)
                {
                    rotators.Add(rotator);
                    rotator.Initialize(reflector, inputField,editCamera, stringEffect);
                    rotator.transform.rotation = reflector.transform.rotation;
                }

                if (mover != null && inputField != null)
                {
                    movers.Add(mover);
                    mover.Initialize(reflector, editCamera, stringEffect);/*
                    mover.transform.rotation = reflector.transform.rotation;*/
                }

            }
        }
    }
}