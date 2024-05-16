using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphSearch {
    public static BFSResult BFSGetRange(HexGrid grid, Vector3Int startingPoint, int movementPonits) {
        Dictionary<Vector3Int, Vector3Int?> visitedNodes = new Dictionary<Vector3Int, Vector3Int?>();
        Dictionary<Vector3Int, int> costSoFar = new Dictionary<Vector3Int, int>();
        Queue<Vector3Int> nodesToVisitQueue = new Queue<Vector3Int>();

        nodesToVisitQueue.Enqueue(startingPoint);
        costSoFar.Add(startingPoint, 0);
        visitedNodes.Add(startingPoint, null);

        while (nodesToVisitQueue.Count > 0) {
            Vector3Int currentNode = nodesToVisitQueue.Dequeue();
            foreach (Vector3Int neighbour in grid.GetNeighboursFor(currentNode)) {
                if (!grid[neighbour].isObstacle) {
                    int nodeCost = grid[neighbour].Cost + (currentNode.y < neighbour.y ? 1 : 0);
                    int currentCost = costSoFar[currentNode];
                    int newCost = currentCost + nodeCost;
                    if (newCost <= movementPonits) {
                        if (!costSoFar.Keys.Contains(neighbour) || newCost < costSoFar[neighbour]) {
                            costSoFar[neighbour] = newCost;
                            visitedNodes[neighbour] = currentNode;
                            if (!nodesToVisitQueue.Contains(neighbour)) {
                                nodesToVisitQueue.Enqueue(neighbour);
                            }
                        }
                    }
                }
            }
        }
        return new BFSResult(visitedNodes);
    }
}

