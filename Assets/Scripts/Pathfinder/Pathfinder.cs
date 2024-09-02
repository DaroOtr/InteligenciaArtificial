using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[Serializable]
public enum PathfinderFlags
{
    AStar_Pf,Breadth_Pf,Depth_Pf,Dijstra_Pf
}
public abstract class Pathfinder<TNodeType,TCoordinateType> 
    where TNodeType : INode , INode<TCoordinateType>
    where TCoordinateType : IEquatable<TCoordinateType>
{
    public List<TNodeType> FindPath(TNodeType startNode, TNodeType destinationNode, ICollection<TNodeType> graph)
    {
        Dictionary<TNodeType, (TNodeType Parent, int AcumulativeCost, int Heuristic)> nodes =
            new Dictionary<TNodeType, (TNodeType Parent, int AcumulativeCost, int Heuristic)>();

        foreach (TNodeType node in graph)
        {
            nodes.Add(node, (default, Random.Range(0,10), Random.Range(0,10)));
        }

        List<TNodeType> openList = new List<TNodeType>();
        List<TNodeType> closedList = new List<TNodeType>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            TNodeType currentNode = openList[0];
            int currentIndex = 0;

            for (int i = 1; i < openList.Count; i++)
            {
                if (nodes[openList[i]].AcumulativeCost + nodes[openList[i]].Heuristic <
                nodes[currentNode].AcumulativeCost + nodes[currentNode].Heuristic)
                {
                    currentNode = openList[i];
                    currentIndex = i;
                }
            }

            openList.RemoveAt(currentIndex);
            closedList.Add(currentNode);

            if (NodesEquals(currentNode, destinationNode))
            {
                return GeneratePath(startNode, destinationNode);
            }

            foreach (TNodeType neighbor in GetNeighbors(currentNode))
            {
                if (!nodes.ContainsKey(neighbor) ||
                IsBloqued(neighbor) ||
                closedList.Contains(neighbor))
                {
                    continue;
                }

                int tentativeNewAcumulatedCost = 0;
                tentativeNewAcumulatedCost += nodes[currentNode].AcumulativeCost;
                tentativeNewAcumulatedCost += MoveToNeighborCost(currentNode, neighbor);

                if (!openList.Contains(neighbor) || tentativeNewAcumulatedCost < nodes[currentNode].AcumulativeCost)
                {
                    nodes[neighbor] = (currentNode, tentativeNewAcumulatedCost, Distance(neighbor, destinationNode));

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }
        
        return null;
        
        List<TNodeType> GeneratePath(TNodeType startNode, TNodeType goalNode)
        {
            List<TNodeType> path = new List<TNodeType>();
            TNodeType currentNode = goalNode;

            while (!NodesEquals(currentNode, startNode))
            {
                path.Add(currentNode);
                currentNode = nodes[currentNode].Parent;
            }

            path.Reverse();
            return path;
        }
    }

    protected abstract ICollection<TNodeType> GetNeighbors(TNodeType node);

    protected abstract int Distance(TNodeType A, TNodeType B);

    protected abstract bool NodesEquals(TNodeType A, TNodeType B);

    protected abstract int MoveToNeighborCost(TNodeType A, TNodeType B);

    protected abstract bool IsBloqued(TNodeType node);
}