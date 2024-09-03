using System;
using System.Collections.Generic;
using Pathfinder.Algorithm;
using Pathfinder.Node;
using UnityEngine;

namespace Pathfinder.Grapf
{
    public class GrapfView : MonoBehaviour
    {
        public Grapf<Node<Vector2Int>> Grapf;
        public AlgorithmType _algorithmType;
        public List<Node<Vector2Int>> nodes = new List<Node<Vector2Int>>();
        public int grapfWidth;
        public int grapfHeight;
        public int grapfDepth;
        public int nodeSeparation;
        
        public void InitGrapf()
        {
            Grapf = new Grapf<Node<Vector2Int>>();
            Grapf.SetgrapfParameters
                (() =>
                    {
                        List<Node<Vector2Int>> _nodes = new List<Node<Vector2Int>>();
                        for (int i = 0; i < grapfWidth; i++)
                        {
                            for (int j = 0; j < grapfHeight; j++)
                            {
                                Node<Vector2Int> node = new Node<Vector2Int>();
                                node.SetCoordinate(new Vector2Int(i ,j ));
                                node.SetDistanceMethod((other) =>
                                {
                                    return Vector2Int.Distance(node.GetCoordinate(),other);
                                });
                                node.SetNodeCost(0);
                                node.SetBlock(false);
                                node.SetNodeType(RtsNodeType.NormalTerrain);
                                _nodes.Add(node);
                            }
                        }
                        return _nodes;
                    },
                    () =>
                    {
                        foreach (Node<Vector2Int> currentNode in Grapf.GetNodes())
                        {
                            foreach (Node<Vector2Int> neighbor in Grapf.GetNodes())
                            {
                                if (neighbor.GetCoordinate().x == currentNode.GetCoordinate().x &&
                                    Math.Abs(neighbor.GetCoordinate().y - currentNode.GetCoordinate().y) == 1)
                                {
                                    currentNode.AddNeighbor(neighbor.GetNodeID(),0);
                                }
                                
                                else if (neighbor.GetCoordinate().y == currentNode.GetCoordinate().y &&
                                         Math.Abs(neighbor.GetCoordinate().x - currentNode.GetCoordinate().x) == 1)
                                {
                                    currentNode.AddNeighbor(neighbor.GetNodeID(),0);
                                }
                                
                                if (_algorithmType.Equals(AlgorithmType.Dijstra_Pf) || _algorithmType.Equals(AlgorithmType.AStar_Pf))
                                {
                                    if (Math.Abs(neighbor.GetCoordinate().y - currentNode.GetCoordinate().y) == 1 && Math.Abs(neighbor.GetCoordinate().x - currentNode.GetCoordinate().x) == 1)
                                        currentNode.AddNeighbor(neighbor.GetNodeID(),0);
                                }
                            }
                        }
                    });
            
            Grapf.InitGrapf();
            foreach (Node<Vector2Int> node in Grapf.GetNodes())
            {
                nodes.Add(node);
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;
        
            foreach (Node<Vector2Int> node in Grapf.Nodes)
            {
                if (node == null)
                    return;
            
                Vector3 nodeCordinates = new Vector3(node.GetCoordinate().x * nodeSeparation, node.GetCoordinate().y * nodeSeparation);
                
                foreach (int neighbor in node.GetNeighbors())
                {
                    Gizmos.color = Color.white;
                    Vector2Int neighborCordinates = Grapf.GetNode(neighbor).GetCoordinate();
                    Gizmos.DrawLine(nodeCordinates,new Vector3(neighborCordinates.x * nodeSeparation,neighborCordinates.y * nodeSeparation));
                } 
            }
        }
    }
}
