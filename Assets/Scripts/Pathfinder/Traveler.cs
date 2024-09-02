using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinder;
using UnityEngine;
using Random = UnityEngine.Random;

public class Traveler : MonoBehaviour
{
    public GrapfView grapfView;
    private PathfinderFlags pathfinder_flag;

    private Pathfinder<Node<Vector2Int>, Vector2Int> pathfinder;
    List<Node<Vector2Int>> path = new List<Node<Vector2Int>>();

    private Node<Vector2Int> startNode; 
    private Node<Vector2Int> destinationNode;

    void Start()
    {
        grapfView.InitGrapf();
        pathfinder_flag = grapfView.pathfinder_flag;
        Debug.Log(pathfinder_flag.ToString());
        switch (pathfinder_flag)
        {
            case PathfinderFlags.AStar_Pf:
                pathfinder = new AStarPathfinder<Node<Vector2Int>, Vector2Int>();
                break;
            case PathfinderFlags.Breadth_Pf:
                pathfinder = new BreadthPathfinder<Node<Vector2Int>, Vector2Int>();
                break;
            case PathfinderFlags.Depth_Pf:
                pathfinder = new DepthFirstPathfinder<Node<Vector2Int>, Vector2Int>();
                break;
            case PathfinderFlags.Dijstra_Pf:
                pathfinder = new DijstraPathfinder<Node<Vector2Int>, Vector2Int>();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        int nodeMinValue = grapfView.grapf.nodes.Count / 2;
        int nodeMaxValue = grapfView.grapf.nodes.Count;
        
        startNode = grapfView.grapf.nodes[Random.Range(0,nodeMinValue)];
        
        destinationNode = grapfView.grapf.nodes[Random.Range(nodeMinValue,nodeMaxValue)];

        //path = pathfinder.FindPath(startNode, destinationNode, grapfView.grapf.nodes);
        //StartCoroutine(Move(path));
    }

    public IEnumerator Move(List<Node<Vector2Int>> path) 
    {
        foreach (Node<Vector2Int> node in path)
        {
            transform.position = new Vector3(node.GetCoordinate().x, node.GetCoordinate().y);
            yield return new WaitForSeconds(1.0f);
        }
    }
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;


        foreach (Node<Vector2Int> node in grapfView.grapf.nodes)
        {
            switch (node)
            {
                case var _ when node.Equals(startNode):
                    Gizmos.color = Color.blue;
                    break;
                case var _ when node.Equals(destinationNode):
                    Gizmos.color = Color.black;
                    break;
                case var _ when path.Contains(node):
                    Gizmos.color = Color.yellow;
                    break;
                default:
                    Gizmos.color = node.IsBloqued() ? Color.red : Color.green;
                    break;
            }
            Vector3 nodeCordinates = new Vector3(node.GetCoordinate().x, node.GetCoordinate().y);
            Gizmos.DrawWireSphere(nodeCordinates,0.2f);
        }
    }
}
