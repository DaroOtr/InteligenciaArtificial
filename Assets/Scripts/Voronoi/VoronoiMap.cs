using System.Collections.Generic;
using Pathfinder.Grapf;
using Pathfinder.Node;
using UnityEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Voronoi
{
    public class VoronoiMap
    {
        private List<Vector3> nodes = new List<Vector3>();
        private List<MinesHolder> mines = new List<MinesHolder>();
        private Grapf<Node<Vector2Int>> _grapf = new Grapf<Node<Vector2Int>>();
        private int _grapfMaxWidth;
        private int _grapfMaxHeight;
        Vector3 mid;

        List<Vector3> vertex = new List<Vector3>();

        List<Side> aux;

        public void InitVoronoid(int grapfMaxWidth, int grapfMaxHeight, Grapf<Node<Vector2Int>> grapf)
        {
            _grapfMaxWidth = grapfMaxWidth;
            _grapfMaxHeight = grapfMaxHeight;
            _grapf = grapf;

            vertex.Add(new Vector3(grapfMaxWidth, grapfMaxHeight, 0));
            vertex.Add(new Vector3(grapfMaxWidth, -1, 0));
            vertex.Add(new Vector3(-1, -1, 0));
            vertex.Add(new Vector3(-1, grapfMaxHeight, 0));

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
        }
        
        public Vector2? GetClosestInterestPoint(Vector3 point)
        {
            PolygonCutter polygonCutter = new PolygonCutter();

            foreach (MinesHolder mine in mines)
            {
                if (polygonCutter.IsPointInPolygon(point, mine.poligon))
                {
                    return mine.position;
                }
            }

            return null;
        }

        public void ReCalculateVoronoiMap()
        {
            vertex.Clear();
            mines.Clear();
            vertex.Add(new Vector3(_grapfMaxWidth, _grapfMaxHeight));
            vertex.Add(new Vector3(_grapfMaxWidth, -1));
            vertex.Add(new Vector3(-1, -1));
            vertex.Add(new Vector3(-1, _grapfMaxHeight));

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

        public void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            Debug.Log(mines.Count);
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
                //Vector3 current = vertex[i];
                //Vector3 next = vertex[(i + 1) % vertex.Count];
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
    }
}