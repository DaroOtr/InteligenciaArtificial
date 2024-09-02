using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pathfinder
{
    public class Vector2IntGrapf<TNodeType,TCoordinateType,TPathType> : IGrapf<TNodeType,TCoordinateType,TPathType>
        where TNodeType : INode<TCoordinateType>, INode, new()
        where TPathType : Enum
        where TCoordinateType : IEquatable<TCoordinateType>
    {
        /*
     * TODO : Hacer un grafo generico
     */
        public List<TNodeType> nodes = new List<TNodeType>();
        private Func<List<TNodeType>> _grapfCrationBehaviour;
        //private Action _grapfCrationBehaviour;
        private Action _AddNodeNeighborsBehaviour;
        private TPathType path;
        private int currentNodeId = 0;

        public Vector2IntGrapf(Func<List<TNodeType>> grapfCrationMethod,Action AddNodeNeighborsBehaviour,TPathType pathType)
        {
            _grapfCrationBehaviour = grapfCrationMethod;
            _AddNodeNeighborsBehaviour = AddNodeNeighborsBehaviour;
            path = pathType;
        }

        public void InitGrapf()
        {
            nodes = _grapfCrationBehaviour?.Invoke();
            foreach (TNodeType node in nodes)
            {
                node.SetNodeID(currentNodeId);
                currentNodeId++;
            }
            _AddNodeNeighborsBehaviour?.Invoke();
        }
        
        public void AddNodeNeighbors()
        {
            _AddNodeNeighborsBehaviour?.Invoke();
        }

        public TNodeType GetNode(int NodeId)
        {
            foreach (TNodeType node in nodes)
            {
                if (node.GetNodeID() == NodeId)
                    return node;
            }

            return new TNodeType();
        }

        public List<TNodeType> GetNodes()
        {
            return nodes;
        }

        public void SetNodeCost(int nodeId,int nodeCost)
        {
            GetNode(nodeId).SetNodeCost(nodeCost);
        }

        public void SetNodeTransitionCost(int fromNodeId, int toNodeId,int transitionCost)
        {
            GetNode(fromNodeId).SetNeighborTransitionCost(toNodeId,transitionCost);
        }
    }
}