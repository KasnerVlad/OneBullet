using Project.Scripts.Pool;
using UnityEngine;

namespace Project.Scripts
{
    public class AllBulletGiver : MonoBehaviour
    {
        public static BulletGiver bulletGiver;

        private void Awake()
        {
            bulletGiver = new BulletGiver();
        }
    }
}