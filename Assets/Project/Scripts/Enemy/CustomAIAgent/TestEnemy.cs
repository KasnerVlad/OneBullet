using UnityEngine;

namespace Project.Scripts.Enemy.CustomAIAgent
{
    public class TestEnemy : MonoBehaviour
    {
        private AsyncPathfinderAI agent;
        [SerializeField] private Transform target;
        private void Awake()
        {
            agent = GetComponent<AsyncPathfinderAI>();/*
            agent.target = target;*/
        }
        
    }
}