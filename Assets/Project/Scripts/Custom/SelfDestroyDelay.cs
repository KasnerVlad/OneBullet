using UnityEngine;

public class SelfDestroyDelay : MonoBehaviour
{
    [SerializeField] private float delay;
    void Start()
    {
        Invoke("Destroy", delay);
    }
}
