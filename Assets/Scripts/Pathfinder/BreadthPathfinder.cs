using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pathfinder
{
    public class BreadthPathfinder<TNodeType,TCoordinateType> : Pathfinder<TNodeType,TCoordinateType>
        where TNodeType : INode<TCoordinateType>
        where TCoordinateType : IEquatable<TCoordinateType>
    {
        protected override int Distance(TNodeType A, TNodeType B)
        {
            return 0;
        }

        protected override ICollection<int> GetNeighbors(TNodeType node)
        {
            if (node == null)
            {
                Debug.LogError("this node is null");
                return null;
            }
            ICollection<int> neighbors = new List<int>();

            foreach (int Neighbor in node.GetNeighbors().Reverse())
            {
                neighbors.Add(Neighbor);
            }
            
            return  neighbors;
        }

        protected override bool IsBloqued(TNodeType node)
        {
            return node.IsBloqued();
        }

        protected override int MoveToNeighborCost(TNodeType A, TNodeType B)
        {
            return 0;
        }

        protected override bool NodesEquals(TNodeType A, TNodeType B)
        {
            return A.Equals(B);
        }
    }
}
