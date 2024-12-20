using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinder.Algorithm;
using Pathfinder.Grapf;
using Pathfinder.Node;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pathfinder
{
    public class Traveler : MonoBehaviour
    {
        public GrapfView grapfView;
        private AlgorithmType _algorithmType;

        private Pathfinder<Node<Vector2Int>, Vector2Int> _pathfinder;
        List<Node<Vector2Int>> path = new List<Node<Vector2Int>>();

        private Node<Vector2Int> _startNode;
        private Node<Vector2Int> _destinationNode;

        private bool _isTravelerInitialized = false;

        void Start()
        {
            //InitTraveler();
        }

        public void InitTraveler()
        {
            _algorithmType = grapfView._algorithmType;
            Debug.Log(_algorithmType.ToString());
            switch (_algorithmType)
            {
                case AlgorithmType.AStar_Pf:
                    _pathfinder = new AStarPathfinder<Node<Vector2Int>, Vector2Int>();
                    break;
                case AlgorithmType.Breadth_Pf:
                    _pathfinder = new BreadthPathfinder<Node<Vector2Int>, Vector2Int>();
                    break;
                case AlgorithmType.Depth_Pf:
                    _pathfinder = new DepthFirstPathfinder<Node<Vector2Int>, Vector2Int>();
                    break;
                case AlgorithmType.Dijstra_Pf:
                    _pathfinder = new DijstraPathfinder<Node<Vector2Int>, Vector2Int>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            int nodeMinValue = grapfView.Grapf.Nodes.Count / 2;
            int nodeMaxValue = grapfView.Grapf.Nodes.Count;
            
            _startNode = grapfView.Grapf.Nodes[Random.Range(0, nodeMinValue)];
            
            _destinationNode = grapfView.Grapf.Nodes[Random.Range(nodeMinValue, nodeMaxValue)];
            
            path = _pathfinder.FindPath(_startNode, _destinationNode, grapfView.Grapf);
            StartCoroutine(Move(path));
            _isTravelerInitialized = true;
        }

        public IEnumerator Move(List<Node<Vector2Int>> path)
        {
            //Vector3 lastPos = transform.position;
            foreach (Node<Vector2Int> node in path)
            {
                Vector3 newPos = new Vector3(node.GetCoordinate().x * grapfView._nodeSeparation,
                    node.GetCoordinate().y * grapfView._nodeSeparation);
                
                transform.position = newPos;
                yield return new WaitForSeconds(1.0f);
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || !_isTravelerInitialized)
                return;


            foreach (Node<Vector2Int> node in grapfView.Grapf.Nodes)
            {
                switch (node)
                {
                    case var _ when node.Equals(_startNode):
                        Gizmos.color = Color.blue;
                        break;
                    case var _ when node.Equals(_destinationNode):
                        Gizmos.color = Color.black;
                        break;
                    case var _ when path.Contains(node):
                        Gizmos.color = Color.gray;
                        break;
                    case var _ when node.GetNodeType().Equals(RtsNodeType.NormalTerrain):
                        Gizmos.color = Color.green;
                        break;
                    case var _ when node.GetNodeType().Equals(RtsNodeType.DificultTerrain):
                        Gizmos.color = Color.Lerp(Color.green,Color.yellow,0.7f);
                        break;
                    case var _ when node.GetNodeType().Equals(RtsNodeType.UrbanCenter):
                        Gizmos.color = Color.magenta;
                        break;
                    case var _ when node.GetNodeType().Equals(RtsNodeType.Mine):
                        Gizmos.color = Color.cyan;
                        break;
                    default:
                        if (node.IsBloqued())
                            Gizmos.color = Color.red;
                        break;
                }

                Vector3 nodeCordinates = new Vector3(node.GetCoordinate().x * grapfView._nodeSeparation,
                    node.GetCoordinate().y * grapfView._nodeSeparation);
                Gizmos.DrawWireSphere(nodeCordinates, 0.2f);
            }
        }
    }
}