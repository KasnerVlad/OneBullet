using UnityEngine;

namespace Project.Scripts.Enemy.CustomAIAgent
{
    public class WaypointGrid
    {
        public WaypointNode[,] nodes;
        public int width, height;
        public float cellSize;
        public Vector3 origin;
    
        private LayerMask obstacleMask;
    
        public WaypointGrid(int width, int height, float cellSize, Vector3 origin, LayerMask obstacleMask)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.origin = origin;
            this.obstacleMask = obstacleMask;
    
            GenerateGrid();
        }
    
        void GenerateGrid()
        {
            nodes = new WaypointNode[width, height];
    
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    Vector3 worldPos = origin + new Vector3(x * cellSize, 0, z * cellSize);
                    bool walkable = !Physics.CheckBox(worldPos + Vector3.up * 0.5f, Vector3.one * cellSize * 0.45f, Quaternion.identity, obstacleMask);
                    nodes[x, z] = new WaypointNode(worldPos, walkable);
                }
            }
    
            // Связи
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    WaypointNode node = nodes[x, z];
                    if (!node.walkable) continue;
    
                    foreach (Vector2Int offset in new Vector2Int[] {
                        new Vector2Int(-1, 0), new Vector2Int(1, 0),
                        new Vector2Int(0, -1), new Vector2Int(0, 1),
                        new Vector2Int(-1, -1), new Vector2Int(1, 1),
                        new Vector2Int(-1, 1), new Vector2Int(1, -1)
                    })
                    {
                        int nx = x + offset.x;
                        int nz = z + offset.y;
                        if (nx >= 0 && nz >= 0 && nx < width && nz < height)
                        {
                            WaypointNode neighbor = nodes[nx, nz];
                            if (neighbor.walkable)
                                node.neighbors.Add(neighbor);
                        }
                    }
                }
            }
        }
    
        public WaypointNode GetClosestNode(Vector3 worldPos)
        {
            int x = Mathf.Clamp(Mathf.RoundToInt((worldPos.x - origin.x) / cellSize), 0, width - 1);
            int z = Mathf.Clamp(Mathf.RoundToInt((worldPos.z - origin.z) / cellSize), 0, height - 1);
            return nodes[x, z];
        }
    }
}