using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Pathfinder
{
    public class GrapfView : MonoBehaviour
    {
        public Grapf<Node<Vector2Int>> grapf;
        public PathfinderFlags pathfinder_flag;
        public List<Node<Vector2Int>> nodes = new List<Node<Vector2Int>>();
        public int grapfWidth;
        public int grapfHeight;
        
        public void InitGrapf()
        {
            grapf = new Grapf<Node<Vector2Int>>
                (() =>
                    {
                        List<Node<Vector2Int>> _nodes = new List<Node<Vector2Int>>();
                        for (int i = 0; i < grapfWidth; i++)
                        {
                            for (int j = 0; j < grapfHeight; j++)
                            {
                                Node<Vector2Int> node = new Node<Vector2Int>();
                                node.SetCoordinate(new Vector2Int(i,j));
                                node.SetDistanceMethod((other) =>
                                {
                                    return Vector2Int.Distance(node.GetCoordinate(),other);
                                });
                                node.SetNodeCost(0);
                                node.SetBlock(false);
                                _nodes.Add(node);   
                            }
                        }
                        return _nodes;
                    },
                    () =>
                    {
                        foreach (Node<Vector2Int> currentNode in grapf.GetNodes())
                        {
                            foreach (Node<Vector2Int> neighbor in grapf.GetNodes())
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
                                
                                if (pathfinder_flag.Equals(PathfinderFlags.Dijstra_Pf) || pathfinder_flag.Equals(PathfinderFlags.AStar_Pf))
                                {
                                    if (Math.Abs(neighbor.GetCoordinate().y - currentNode.GetCoordinate().y) == 1 && Math.Abs(neighbor.GetCoordinate().x - currentNode.GetCoordinate().x) == 1)
                                        currentNode.AddNeighbor(neighbor.GetNodeID(),0);
                                }
                            }
                        }
                    });
            grapf.InitGrapf();
            foreach (Node<Vector2Int> node in grapf.GetNodes())
            {
                nodes.Add(node);
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;
        
            foreach (Node<Vector2Int> node in grapf.Nodes)
            {
                if (node == null)
                    return;
            
                Vector3 nodeCordinates = new Vector3(node.GetCoordinate().x, node.GetCoordinate().y);
                
                foreach (int neighbor in node.GetNeighbors())
                {
                    Gizmos.color = Color.white;
                    Vector2Int neighborCordinates = grapf.GetNode(neighbor).GetCoordinate();
                    Gizmos.DrawLine(nodeCordinates,new Vector3(neighborCordinates.x,neighborCordinates.y));
                } 
            }
        }
    }
}
