using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Enemy.CustomAIAgent
{
    public class WaypointNode
    {
        public Vector3 position;
        public List<WaypointNode> neighbors = new List<WaypointNode>();
        public bool walkable = true;

        public WaypointNode(Vector3 pos, bool walkable = true)
        {
            this.position = pos;
            this.walkable = walkable;
        }
    }
}