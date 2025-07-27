using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Enemy.CustomAIAgent
{
    public static class AStarPathfinding
    {
        public static List<WaypointNode> FindPath(WaypointNode start, WaypointNode goal)
        {
            var openSet = new List<WaypointNode> { start };
            var cameFrom = new Dictionary<WaypointNode, WaypointNode>();
            var gScore = new Dictionary<WaypointNode, float>();
            var fScore = new Dictionary<WaypointNode, float>();

            gScore[start] = 0;
            fScore[start] = Vector3.Distance(start.position, goal.position);

            while (openSet.Count > 0)
            {
                openSet.Sort((a, b) => fScore.GetValueOrDefault(a, float.PositiveInfinity).CompareTo(fScore.GetValueOrDefault(b, float.PositiveInfinity)));
                var current = openSet[0];

                if (current == goal)
                {
                    List<WaypointNode> path = new List<WaypointNode>();
                    while (cameFrom.ContainsKey(current))
                    {
                        path.Insert(0, current);
                        current = cameFrom[current];
                    }
                    return path;
                }

                openSet.Remove(current);

                foreach (var neighbor in current.neighbors)
                {
                    float tentativeG = gScore.GetValueOrDefault(current, float.PositiveInfinity) + Vector3.Distance(current.position, neighbor.position);
                    if (tentativeG < gScore.GetValueOrDefault(neighbor, float.PositiveInfinity))
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeG;
                        fScore[neighbor] = tentativeG + Vector3.Distance(neighbor.position, goal.position);
                        if (!openSet.Contains(neighbor)) openSet.Add(neighbor);
                    }
                }
            }

            return null;
        }
    }
}