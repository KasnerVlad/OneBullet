using UnityEngine;

namespace Project.Scripts.Enemy
{
    public class Enemy : MonoBehaviour
    {
        public void OnDeath()
        {
            Destroy(gameObject);
        }
    }
}