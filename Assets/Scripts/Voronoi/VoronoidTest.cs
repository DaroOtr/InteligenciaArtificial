using System;
using System.Collections.Generic;
using Pathfinder.Algorithm;
using Pathfinder.Grapf;
using Pathfinder.Node;
using UnityEditor;
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
        private int _grapfMaxWidth = 11;
        private int _grapfMaxHeight = 11;
        private int _mineCount = 5;
        private bool _isGrapfInitialized = false;

        private List<MinesHolder> mines = new List<MinesHolder>();
        List<Vector3> vertex = new List<Vector3>();

        List<Side> aux;

        //[SerializeField] private VoronoiMap coronoid = new VoronoiMap();
        private Voronoi<Node<Vector2Int>, Vector2Int> map = new Voronoi<Node<Vector2Int>, Vector2Int>();

        void Start()
        {
            _grapf.SetgrapfParameters
            (() =>
                {
                    List<Node<Vector2Int>> _nodes = new List<Node<Vector2Int>>();
                    for (int i = 0; i < _grapfMaxHeight; i++)
                    {
                        for (int j = 0; j < _grapfMaxWidth; j++)
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
                    foreach (Node<Vector2Int> currentNode in _grapf.GetNodes())
                    {
                        foreach (Node<Vector2Int> neighbor in _grapf.GetNodes())
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
                }, _grapfMaxWidth, _grapfMaxHeight);
            _grapf.InitGrapf();
            foreach (Node<Vector2Int> node in _grapf.GetNodes())
            {
                if (node.GetCoordinate().x == 0)
                {
                    if (node.GetCoordinate().y == 0)
                        _cornerLimits.Add(node);
                    else if (node.GetCoordinate().y == _grapfMaxHeight - 1)
                        _cornerLimits.Add(node);
                }
                else if (node.GetCoordinate().x == _grapfMaxWidth - 1)
                {
                    if (node.GetCoordinate().y == 0)
                        _cornerLimits.Add(node);
                    else if (node.GetCoordinate().y == _grapfMaxHeight - 1)
                        _cornerLimits.Add(node);
                }
            }

            SetInitialNodes();
            //coronoid.InitVoronoid(_grapfMaxWidth,_grapfMaxHeight,_grapf);
            SetVoronoid();
            _isGrapfInitialized = true;
        }

        public void SetVoronoid()
        {
            Func<List<MinesHolder>> _creator = CreatorFunc;
            Func<Node<Vector2Int>, Vector2Int> _closestInterestPointFunc = GetClosestInterestPoint;
            Action<MinesHolder, MinesHolder> _calculateSidesFunc = GetSide;
            Action _drawFunc = DrawFunc;
            map.SetVoronoidCreationParams(_grapfMaxWidth, _grapfMaxHeight, _grapf, _creator, _closestInterestPointFunc,
                _calculateSidesFunc, _drawFunc);
            map.InitVoronoid();
        }


        [ContextMenu("RemoveMine")]
        public void RemoveMine()
        {
            _grapf.GetNode(RtsNodeType.Mine).SetBlock(true);
            //coronoid.ReCalculateVoronoiMap();
            map.ReCalculateVoronoiMap();
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

            // coronoid.OnDrawGizmos();
            map.DrawVoronoi();
        }

        #region VoronoiCreationFuncs

        private List<MinesHolder> CreatorFunc()
        {
            vertex.Add(new Vector3(_grapfMaxWidth, _grapfMaxHeight, 0));
            vertex.Add(new Vector3(_grapfMaxWidth, -1, 0));
            vertex.Add(new Vector3(-1, -1, 0));
            vertex.Add(new Vector3(-1, _grapfMaxHeight, 0));

            foreach (Node<Vector2Int> node in _grapf.GetNodesOfType(RtsNodeType.Mine))
            {
                if (!node.IsBloqued())
                    mines.Add(new MinesHolder(new Vector3(node.GetCoordinate().x, node.GetCoordinate().y),
                        new Poligon(vertex)));
            }

            for (int i = 0; i < mines.Count; i++)
            {
                for (int j = 0; j < mines.Count; j++)
                {
                    if (i != j)
                        GetSide(mines[i], mines[j]);
                }
            }

            return mines;
        }

        public void GetSide(MinesHolder A, MinesHolder B)
        {
            Vector3 mid = new Vector3((A.position.x + B.position.x) / 2.0f, (A.position.y + B.position.y) / 2,
                (A.position.z + B.position.z) / 2.0f);

            Vector3 connector = A.position - B.position;

            Vector3 direction = new Vector3(-connector.y, connector.x, 0).normalized;

            Side outCut = new Side(mid - direction * 100.0f / 2, mid + direction * 100.0f / 2);

            PolygonCutter polygonCutter = new PolygonCutter();

            polygonCutter.CutPolygon(A, outCut);
        }

        public Vector2Int GetClosestInterestPoint(Node<Vector2Int> node)
        {
            PolygonCutter polygonCutter = new PolygonCutter();

            foreach (MinesHolder mine in mines)
            {
                if (polygonCutter.IsPointInPolygon(new Vector3(node.GetCoordinate().x, node.GetCoordinate().y),
                        mine.poligon))
                {
                    return new Vector2Int((int)mine.position.x, (int)mine.position.y);
                }
            }

            return Vector2Int.zero;
        }

        private void DrawFunc()
        {
            if (!Application.isPlaying)
                return;
            
            for (int i = 0; i < mines.Count; i++)
            {
                Gizmos.color = mines[i].color;
                Vector3 center = new Vector3(mines[i].position.x, mines[i].position.y);
                Gizmos.DrawCube(center, Vector3.one / 2);
            }

            Gizmos.color = Color.magenta;
            for (int i = 0; i < vertex.Count; i++)
            {
                Vector3 current = new Vector3(vertex[i].x, vertex[i].y);
                Vector3 next = new Vector3(vertex[(i + 1) % vertex.Count].x, vertex[(i + 1) % vertex.Count].y);
                Gizmos.DrawLine(current, next);
            }


            for (int i = 0; i < mines.Count; i++)
            {
                Gizmos.color = mines[i].color;
                List<Vector3> array = new List<Vector3>();

                for (int k = 0; k < mines[i].poligon.vertices.Count; k++)
                {
                    array.Add(new Vector3(mines[i].poligon.vertices[k].x, mines[i].poligon.vertices[k].y, 0));
                }

                Handles.color = mines[i].color;
                Handles.DrawAAConvexPolygon(array.ToArray());
            }

            Gizmos.color = Color.black;
            for (int j = 0; j < mines.Count; j++)
            {
                for (int i = 0; i < mines[j].poligon.vertices.Count; i++)
                {
                    Vector3 vertex1 = new Vector3(mines[j].poligon.vertices[i].x, mines[j].poligon.vertices[i].y);
                    Vector3 vertex2 =
                        new Vector3(mines[j].poligon.vertices[(i + 1) % mines[j].poligon.vertices.Count].x,
                            mines[j].poligon.vertices[(i + 1) % mines[j].poligon.vertices.Count].y);

                    Gizmos.DrawLine(vertex1, vertex2);
                }
            }

            Gizmos.color = Color.yellow;
            for (int i = 0; i < vertex.Count; i++)
            {
                Vector3 center = new Vector3(vertex[i].x, vertex[i].y);
                Gizmos.DrawSphere(center, 0.25f);
            }

            //for (int i = 0; i < mines.Count; i++)
            //{
            //    GUIStyle style = new GUIStyle();
            //    style.fontSize = 35;
            //    style.normal.textColor = Color.black;
            //    Handles.Label(new Vector3(mines[i].position.x, mines[i].position.z + 0.5f, mines[i].position.z),
            //        (i + 1).ToString(),
            //        style);
            //}
        }

        #endregion
    }
}