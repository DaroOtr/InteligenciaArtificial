using System;
using System.Collections.Generic;
using Pathfinder.Algorithm;
using Pathfinder.Grapf;
using Pathfinder.Node;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Voronoi
{
    public class VoronoidTest : MonoBehaviour
    {
        private Grapf<Node<Vector2Int>> _grapf = new Grapf<Node<Vector2Int>>();
        private List<Node<Vector2Int>> _cornerLimits = new List<Node<Vector2Int>>();
        public AlgorithmType _algorithmType = AlgorithmType.AStar_Pf;
        public int _nodeSeparation = 1;
        private int _grapfMaxWidth = 50;
        private int _grapfMaxHeight = 50;
        private int _mineCount = 217;
        private bool _isGrapfInitialized = false;
        [SerializeField] private VoronoiMap coronoid = new VoronoiMap();

        // Start is called before the first frame update
        //void Start()
        //{
        //    _grapf.SetgrapfParameters
        //    (() =>
        //        {
        //            List<Node<Vector2Int>> _nodes = new List<Node<Vector2Int>>();
        //            for (int i = 0; i < _grapfMaxHeight; i++)
        //            {
        //                for (int j = 0; j < _grapfMaxWidth; j++)
        //                {
        //                    Node<Vector2Int> node = new Node<Vector2Int>();
        //                    node.SetCoordinate(new Vector2Int(i, j));
        //                    node.SetDistanceMethod((other) => { return Vector2Int.Distance(node.GetCoordinate(), other); });
        //                    node.SetNodeCost(0);
        //                    node.SetBlock(false);
        //                    node.SetNodeType(RtsNodeType.NormalTerrain);
        //                    _nodes.Add(node);
        //                }
        //            }
        //
        //            return _nodes;
        //        },
        //        () =>
        //        {
        //            foreach (Node<Vector2Int> currentNode in _grapf.GetNodes())
        //            {
        //                foreach (Node<Vector2Int> neighbor in _grapf.GetNodes())
        //                {
        //                    if (neighbor.GetCoordinate().x == currentNode.GetCoordinate().x &&
        //                        Math.Abs(neighbor.GetCoordinate().y - currentNode.GetCoordinate().y) == 1)
        //                    {
        //                        currentNode.AddNeighbor(neighbor.GetNodeID(), 0);
        //                    }
        //
        //                    else if (neighbor.GetCoordinate().y == currentNode.GetCoordinate().y &&
        //                             Math.Abs(neighbor.GetCoordinate().x - currentNode.GetCoordinate().x) == 1)
        //                    {
        //                        currentNode.AddNeighbor(neighbor.GetNodeID(), 0);
        //                    }
        //
        //                    if (_algorithmType.Equals(AlgorithmType.Dijstra_Pf) ||
        //                        _algorithmType.Equals(AlgorithmType.AStar_Pf))
        //                    {
        //                        if (Math.Abs(neighbor.GetCoordinate().y - currentNode.GetCoordinate().y) == 1 &&
        //                            Math.Abs(neighbor.GetCoordinate().x - currentNode.GetCoordinate().x) == 1)
        //                            currentNode.AddNeighbor(neighbor.GetNodeID(), 0);
        //                    }
        //                }
        //            }
        //        });
        //    _grapf.InitGrapf();
        //    foreach (Node<Vector2Int> node in _grapf.GetNodes())
        //    {
        //        if (node.GetCoordinate().x == 0)
        //        {
        //            if (node.GetCoordinate().y == 0)
        //                _cornerLimits.Add(node);
        //            else if (node.GetCoordinate().y == _grapfMaxHeight - 1)
        //                _cornerLimits.Add(node);
        //        }
        //        else if (node.GetCoordinate().x == _grapfMaxWidth - 1)
        //        {
        //            if (node.GetCoordinate().y == 0)
        //                _cornerLimits.Add(node);
        //            else if (node.GetCoordinate().y == _grapfMaxHeight -1)
        //                _cornerLimits.Add(node);
        //        }
        //    }
        //
        //    SetInitialNodes();
        //    coronoid.InitVoronoid(_grapfMaxWidth,_grapfMaxHeight,_mineCount,_grapf);
        //    _isGrapfInitialized = true;
        //}
        [ContextMenu("RemoveMine")]
        public void RemoveMine()
        {
            _grapf.GetNode(RtsNodeType.Mine).SetBlock(true);
            coronoid.ReCalculateVoronoiMap();
        }

        public void SetInitialNodes()
        {
            List<Node<Vector2Int>> minesPLaced = new List<Node<Vector2Int>>();

            for (int i = 0; i < _mineCount; i++)
            {
                int random = Random.Range(0, _grapf.Nodes.Count);
                while (!minesPLaced.Contains(_grapf.Nodes[random]))
                {
                    random = Random.Range(0, _grapf.Nodes.Count);
                    if (_grapf.Nodes[random].GetNodeType() != RtsNodeType.Mine &&
                        _grapf.Nodes[random].GetNodeType() != RtsNodeType.UrbanCenter)
                    {
                        _grapf.Nodes[random].SetNodeType(RtsNodeType.Mine);
                        minesPLaced.Add(_grapf.Nodes[random]);
                    }
                }
            }

            int halfWidth = _grapfMaxWidth / 2;
            int halfHeight = _grapfMaxHeight / 2;
            foreach (Node<Vector2Int> node in _grapf.Nodes)
            {
                if (node.GetCoordinate().x == halfWidth && node.GetCoordinate().y == halfHeight)
                    node.SetNodeType(RtsNodeType.UrbanCenter);
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || !_isGrapfInitialized)
                return;


            foreach (Node<Vector2Int> node in _grapf.Nodes)
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
            }
            foreach (Node<Vector2Int> limit in _cornerLimits)
            {
                Gizmos.color = Color.red;
                Vector3 limitCordinates = new Vector3(limit.GetCoordinate().x * _nodeSeparation,
                    limit.GetCoordinate().y * _nodeSeparation);
                Gizmos.DrawWireSphere(limitCordinates, 0.2f);
            }

            coronoid.OnDrawGizmos();
        }
    }
}