using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Enemy.CustomAIAgent
{
    public class AIAgent : MonoBehaviour
    {
        public float speed = 3f;
        [SerializeField]private float reachThreshold = 0.2f;
        [SerializeField]private LayerMask obstacleMask;

        private CharacterController controller;
        private List<WaypointNode> path;
        private int currentIndex = 0;
        private float gravity = -9.81f;
        private float verticalSpeed = 0f;
        public bool isStopped = false;
        public bool positionChanged=false;
        private WaypointGrid grid;
        
        void Start()
        {
            controller = GetComponent<CharacterController>();
            grid = new WaypointGrid(50, 50, 1f, Vector3.zero, obstacleMask); // параметры: размер сетки, клетка = 1, и начало в (0,0,0)
        }

        public void SetDestination(Vector3 target)
        {
            var start = grid.GetClosestNode(transform.position);
            var goal = grid.GetClosestNode(target);

            if (start != null && goal != null)
            {
                path = AStarPathfinding.FindPath(start, goal);
                currentIndex = 0;
                isStopped = false;
            }
        }

        void Update()
        {/*
            if(!positionChanged) return;*/
            
            Vector3 targetPos=Vector3.zero;
            Vector3 dir=Vector3.zero;
            if (path != null&&!isStopped&&currentIndex < path.Count)
            {
                Debug.Log("path found");
                targetPos = path[currentIndex].position;
                dir = (targetPos - transform.position);
                dir.y = 0;
                if (dir.magnitude < reachThreshold)
                {
                    currentIndex++;
                    return;
                }
            }


            Vector3 move = dir.normalized * speed;/*

            if (controller.isGrounded)
                verticalSpeed = -0.01f;
            else
                verticalSpeed += gravity * Time.deltaTime;

            move.y = verticalSpeed;*/
            /*if(positionChanged) */controller.Move(move * Time.deltaTime);
        }
    }
}