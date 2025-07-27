using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Enemy.CustomAIAgent
{
    public class SimpleAStar
    {
        /*public static List<WaypointNode> FindPath(WaypointNode start, WaypointNode goal)
        {
            List<WaypointNode> openSet = new List<WaypointNode> { start };
            Dictionary<WaypointNode, WaypointNode> cameFrom = new Dictionary<WaypointNode, WaypointNode>();
            Dictionary<WaypointNode, float> gScore = new Dictionary<WaypointNode, float>();
            Dictionary<WaypointNode, float> fScore = new Dictionary<WaypointNode, float>();

            foreach (var node in GameObject.FindObjectsOfType<WaypointNode>())
            {
                gScore[node] = Mathf.Infinity;
                fScore[node] = Mathf.Infinity;
            }

            gScore[start] = 0;
            fScore[start] = Vector3.Distance(start.transform.position, goal.transform.position);

            while (openSet.Count > 0)
            {
                openSet.Sort((a, b) => fScore[a].CompareTo(fScore[b]));
                WaypointNode current = openSet[0];

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
                    float tentativeG = gScore[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);
                    if (tentativeG < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeG;
                        fScore[neighbor] = gScore[neighbor] + Vector3.Distance(neighbor.transform.position, goal.transform.position);

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }

            return null; // путь не найден
        }*/
    }
}