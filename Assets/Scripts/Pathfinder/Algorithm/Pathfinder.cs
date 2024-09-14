using System;
using System.Collections.Generic;
using Pathfinder.Grapf;
using Pathfinder.Node;
using Random = UnityEngine.Random;

namespace Pathfinder.Algorithm
{
    [Serializable]
    public enum AlgorithmType
    {
        AStar_Pf,Breadth_Pf,Depth_Pf,Dijstra_Pf
    }
    public abstract class Pathfinder<TNodeType,TCoordinateType> 
        where TNodeType : INode , INode<TCoordinateType>
        where TCoordinateType : IEquatable<TCoordinateType>
    {
        public List<TNodeType> FindPath(TNodeType startNode, TNodeType destinationNode, IGrapf<TNodeType> graph)
        {
            Dictionary<TNodeType, (TNodeType Parent, int AcumulativeCost, int Heuristic)> nodes =
                new Dictionary<TNodeType, (TNodeType Parent, int AcumulativeCost, int Heuristic)>();

            foreach (TNodeType node in graph)
            {
                nodes.Add(node, (default, Random.Range(0,10), Random.Range(0,10)));
            }

            // TODO : Pasar a que los identifique con los ID
        
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
            
                foreach (int neighbor in GetNeighbors(currentNode))
                {
                
                    if (!nodes.ContainsKey(graph.GetNode(neighbor)) ||
                        IsBloqued(graph.GetNode(neighbor)) ||
                        closedList.Contains(graph.GetNode(neighbor)))
                    {
                        continue;
                    }
              
                    int tentativeNewAcumulatedCost = 0;
                    tentativeNewAcumulatedCost += nodes[currentNode].AcumulativeCost;
                    tentativeNewAcumulatedCost += MoveToNeighborCost(currentNode, graph.GetNode(neighbor));
              
                    if (!openList.Contains(graph.GetNode(neighbor)) || tentativeNewAcumulatedCost < nodes[currentNode].AcumulativeCost)
                    {
                        nodes[graph.GetNode(neighbor)] = (currentNode, tentativeNewAcumulatedCost, Distance(graph.GetNode(neighbor), destinationNode));
              
                        if (!openList.Contains(graph.GetNode(neighbor)))
                        {
                            openList.Add(graph.GetNode(neighbor));
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
                path.Add(startNode);
                path.Reverse();
                return path;
            }
        }

        protected abstract ICollection<int> GetNeighbors(TNodeType node);

        protected abstract int Distance(TNodeType A, TNodeType B);

        protected abstract bool NodesEquals(TNodeType A, TNodeType B);

        protected abstract int MoveToNeighborCost(TNodeType A, TNodeType B);

        protected abstract bool IsBloqued(TNodeType node);
    }
}