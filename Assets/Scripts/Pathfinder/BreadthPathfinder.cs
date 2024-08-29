﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BreadthPathfinder<NodeType,CoordinateType> : Pathfinder<NodeType,CoordinateType>
    where NodeType : INode , INode<CoordinateType>
    where CoordinateType : IEquatable<CoordinateType>
{
    protected override int Distance(NodeType A, NodeType B)
    {
        return 0;
    }

    protected override ICollection<NodeType> GetNeighbors(NodeType node)
    {
        if (node == null)
        {
            Debug.LogError("this node is null");
            return null;
        }
        ICollection<NodeType> neighbors = new List<NodeType>();

        foreach (NodeType Neighbor in node.GetNeighbors())
        {
            neighbors.Add(Neighbor);
        }

        neighbors.Reverse();
        return neighbors;
    }

    protected override bool IsBloqued(NodeType node)
    {
        return node.IsBloqued();
    }

    protected override int MoveToNeighborCost(NodeType A, NodeType B)
    {
        return 0;
    }

    protected override bool NodesEquals(NodeType A, NodeType B)
    {
        return A.Equals(B);
    }
}
