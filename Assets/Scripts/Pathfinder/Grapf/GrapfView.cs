using System;
using System.Collections.Generic;
using System.Linq;
using Pathfinder.Algorithm;
using Pathfinder.Node;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pathfinder.Grapf
{
    public class GrapfView : MonoBehaviour
    {
        public Grapf<Node<Vector2Int>> Grapf { get; private set; }
        public AlgorithmType _algorithmType { get; private set; }
        public List<Node<Vector2Int>> nodes { get; private set; }
        private int _grapfWidth;
        private int _grapfHeight;
        public int _nodeSeparation { get; private set; }
        public int _mineCount { get; private set; }

        private bool _isGrapfInitialized = false;
        
        [SerializeField] private GameObject terrainPrefab;
        [SerializeField] private GameObject urbancenterPrefab;
        [SerializeField] private GameObject minePrefab;

        public void SetGrapfCreationParams(int grapfWidth, int grapfHeight, int nodeSeparation, int mineCount)
        {
            _grapfWidth = grapfWidth;
            _grapfHeight = grapfHeight;
            _nodeSeparation = nodeSeparation;
            _mineCount = mineCount;
            nodes = new List<Node<Vector2Int>>();
            _algorithmType = AlgorithmType.AStar_Pf;
        }

        public void InitGrapf()
        {
            Grapf = new Grapf<Node<Vector2Int>>();
            Grapf.SetgrapfParameters
            (() =>
                {
                    List<Node<Vector2Int>> _nodes = new List<Node<Vector2Int>>();
                    for (int i = 0; i < _grapfWidth; i++)
                    {
                        for (int j = 0; j < _grapfHeight; j++)
                        {
                            Node<Vector2Int> node = new Node<Vector2Int>();
                            node.SetCoordinate(new Vector2Int(i, j));
                            node.SetDistanceMethod((other) =>
                            {
                                return Vector2Int.Distance(node.GetCoordinate(), other);
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
                                currentNode.AddNeighbor(neighbor.GetNodeID(), 0);
                            }

                            else if (neighbor.GetCoordinate().y == currentNode.GetCoordinate().y &&
                                     Math.Abs(neighbor.GetCoordinate().x - currentNode.GetCoordinate().x) == 1)
                            {
                                currentNode.AddNeighbor(neighbor.GetNodeID(), 0);
                            }

                            if (_algorithmType.Equals(AlgorithmType.Dijstra_Pf) ||
                                _algorithmType.Equals(AlgorithmType.AStar_Pf))
                            {
                                if (Math.Abs(neighbor.GetCoordinate().y - currentNode.GetCoordinate().y) == 1 &&
                                    Math.Abs(neighbor.GetCoordinate().x - currentNode.GetCoordinate().x) == 1)
                                    currentNode.AddNeighbor(neighbor.GetNodeID(), 0);
                            }
                        }
                    }
                },_grapfWidth,_grapfHeight);

            Grapf.InitGrapf();
            foreach (Node<Vector2Int> node in Grapf.GetNodes())
            {
                nodes.Add(node);
            }

            SetInitialNodes();
            DrawGrapf();
            _isGrapfInitialized = true;
        }

        public void SetInitialNodes()
        {
            List<Node<Vector2Int>> minesPLaced = new List<Node<Vector2Int>>();

            for (int i = 0; i < _mineCount; i++)
            {
                int random = Random.Range(0, nodes.Count);
                while (!minesPLaced.Contains(Grapf.Nodes[random]))
                {
                    random = Random.Range(0, nodes.Count);
                    if (Grapf.Nodes[random].GetNodeType() != RtsNodeType.Mine &&
                        Grapf.Nodes[random].GetNodeType() != RtsNodeType.UrbanCenter)
                    {
                        Grapf.Nodes[random].SetNodeType(RtsNodeType.Mine);
                        minesPLaced.Add(Grapf.Nodes[random]);
                    }
                }
            }

            int halfWidth = _grapfWidth / 2;
            int halfHeight = _grapfHeight / 2;
            foreach (Node<Vector2Int> node in Grapf.Nodes)
            {
                if (node.GetCoordinate().x == halfWidth && node.GetCoordinate().y == halfHeight)
                    node.SetNodeType(RtsNodeType.UrbanCenter);
            }
        }
        
        private void DrawGrapf()
        {
            foreach (Node<Vector2Int> node in Grapf.Nodes)
            {
                Vector3 nodeCordinates = new Vector3(node.GetCoordinate().x * _nodeSeparation,
                    node.GetCoordinate().y * _nodeSeparation);

                switch (node)
                {
                    case var _ when node.GetNodeType().Equals(RtsNodeType.NormalTerrain):
                        Instantiate(terrainPrefab, nodeCordinates, Quaternion.identity);
                        break;
                    case var _ when node.GetNodeType().Equals(RtsNodeType.DificultTerrain):
                        Gizmos.color = Color.Lerp(Color.green, Color.yellow, 0.7f);
                        break;
                    case var _ when node.GetNodeType().Equals(RtsNodeType.UrbanCenter):
                        Instantiate(urbancenterPrefab, nodeCordinates, Quaternion.identity);
                        break;
                    case var _ when node.GetNodeType().Equals(RtsNodeType.Mine):
                        Instantiate(minePrefab, nodeCordinates, Quaternion.identity);
                        break;
                    default:
                        if (node.IsBloqued())
                            Gizmos.color = Color.red;
                        break;
                }


            }

        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || !_isGrapfInitialized)
                return;


            foreach (Node<Vector2Int> node in Grapf.Nodes)
            {
                switch (node)
                {
                    case var _ when node.GetNodeType().Equals(RtsNodeType.NormalTerrain):
                        Gizmos.color = Color.green;
                        break;
                    case var _ when node.GetNodeType().Equals(RtsNodeType.DificultTerrain):
                        Gizmos.color = Color.Lerp(Color.green, Color.yellow, 0.7f);
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
                
                Vector3 nodeCordinates = new Vector3(node.GetCoordinate().x * _nodeSeparation,
                    node.GetCoordinate().y * _nodeSeparation);
                Gizmos.DrawWireSphere(nodeCordinates, 0.2f);
                
                foreach (int neighbor in node.GetNeighbors())
                {
                    Gizmos.color = Color.white;
                    Vector2Int neighborCordinates = Grapf.GetNode(neighbor).GetCoordinate();
                    Gizmos.DrawLine(nodeCordinates,
                        new Vector3(neighborCordinates.x * _nodeSeparation, neighborCordinates.y * _nodeSeparation));
                }
            }
        }
    }
}
