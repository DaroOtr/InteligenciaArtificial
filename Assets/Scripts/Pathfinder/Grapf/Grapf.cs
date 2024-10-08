﻿using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinder.Node;
using UnityEngine;

namespace Pathfinder.Grapf
{
    public class Grapf<TNodeType> : IGrapf<TNodeType>
        where TNodeType : INode, new()
    {
        public List<TNodeType> Nodes = new List<TNodeType>();
        public int _grapfMaxWidth;
        public int _grapfMaxHeight;
        private Func<List<TNodeType>> _grapfCrationBehaviour;
        private Action _addNodeNeighborsBehaviour;
        private int _currentNodeId = 0;

        public void SetgrapfParameters(Func<List<TNodeType>> grapfCrationMethod, Action addNodeNeighborsBehaviour,int grapfMaxWidth,int grapfMaxHeight)
        {
            _grapfCrationBehaviour = grapfCrationMethod;
            _addNodeNeighborsBehaviour = addNodeNeighborsBehaviour;
            _grapfMaxWidth = grapfMaxWidth;
            _grapfMaxHeight = grapfMaxHeight;
        }

        public void InitGrapf()
        {
            Nodes = _grapfCrationBehaviour?.Invoke();
            if (Nodes != null)
            {
                foreach (TNodeType node in Nodes)
                {
                    node.SetNodeID(_currentNodeId);
                    _currentNodeId++;
                }

                _addNodeNeighborsBehaviour?.Invoke();
            }
            else
            {
                throw new Exception("NULL :: NODES IN GRAPF");
            }
        }

        public void AddNodeNeighbors()
        {
            _addNodeNeighborsBehaviour?.Invoke();
        }

        public TNodeType GetNode(int NodeId)
        {
            foreach (TNodeType node in Nodes)
            {
                if (node.GetNodeID() == NodeId)
                    return node;
            }

            return new TNodeType();
        }
        

        public TNodeType GetNode(RtsNodeType nodeType)
        {
            foreach (TNodeType node in Nodes)
            {
                if (!node.IsBloqued())
                {
                    if (node.GetNodeType() == nodeType)
                        return node;
                }
            }

            return new TNodeType();
        }

        public ICollection<TNodeType> GetNodesOfType(RtsNodeType nodeType)
        {
            ICollection<TNodeType> nodeTypes = new List<TNodeType>();
            foreach (TNodeType node in Nodes)
            {
                if (node.GetNodeType() == nodeType)
                    nodeTypes.Add(node);
            }

            if (nodeTypes.Count > 0)
                return nodeTypes;
            
            return null;
        }

        public List<TNodeType> GetNodes()
        {
            return Nodes;
        }

        public void SetNodeCost(int nodeId, int nodeCost)
        {
            GetNode(nodeId).SetNodeCost(nodeCost);
        }

        public void SetNodeTransitionCost(int fromNodeId, int toNodeId, int transitionCost)
        {
            GetNode(fromNodeId).SetNeighborTransitionCost(toNodeId, transitionCost);
        }

        public IEnumerator<TNodeType> GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TNodeType item)
        {
            Nodes.Add(item);
        }

        public void Clear()
        {
            Nodes.Clear();
        }

        public bool Contains(TNodeType item)
        {
            return Nodes.Contains(item);
        }

        public void CopyTo(TNodeType[] array, int arrayIndex)
        {
            Nodes.CopyTo(array, arrayIndex);
        }

        public bool Remove(TNodeType item)
        {
            return Nodes.Remove(item);
        }

        public int Count
        {
            get { return Nodes.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }
    }
}