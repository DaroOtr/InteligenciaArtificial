using System;
using System.Collections.Generic;
using System.Numerics;
using Pathfinder.Grapf;
using Pathfinder.Node;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class VoronoidController : MonoBehaviour
{
    private List<Vector3> nodes = new List<Vector3>();
    [SerializeField] private List<MinesTemp> mines = new List<MinesTemp>();
    private int _grapfMaxWidth;
    private int _grapfMaxHeight;
    private int _maxMineCount;
    Vector3 mid;

    List<Vector3> vertex = new List<Vector3>();

    List<Side> aux;
    
    public void InitVoronoid(int grapfMaxWidth,int grapfMaxHeight,int maxMineCount,Grapf<Node<Vector2Int>> grapf)
    {
        _grapfMaxWidth = grapfMaxWidth;
        _grapfMaxHeight = grapfMaxHeight;
        _maxMineCount = maxMineCount;
        
        vertex.Add(new Vector3(grapfMaxWidth,0,grapfMaxHeight));
        vertex.Add(new Vector3(grapfMaxWidth,0,-1));
        vertex.Add(new Vector3(-1,0,-1));
        vertex.Add(new Vector3(-1,0,grapfMaxHeight));

        foreach (Node<Vector2Int> node in grapf.GetNodesOfType(RtsNodeType.Mine))
        {
            mines.Add(new MinesTemp(new Vector3(node.GetCoordinate().x,0,node.GetCoordinate().y), new Poligon(vertex)));
        }
        
        for (int i = 0; i < mines.Count; i++)
        {
            for (int j = 0; j < mines.Count; j++)
            {
                if(i != j)
                    GetSide(mines[i], mines[j]);
            }
        }
    }

    public void GetSide(MinesTemp A, MinesTemp B)
    {
        Vector3 mid = new Vector3((A.position.x + B.position.x) / 2.0f, (A.position.y + B.position.y) / 2, (A.position.z + B.position.z) / 2.0f);

        Vector3 connector = A.position - B.position;

        Vector3 direction = new Vector3(-connector.z, 0, connector.x).normalized;

        Side outCut = new Side(mid - direction * 100.0f / 2, mid + direction * 100.0f / 2);
        
        PolygonCutter polygonCutter = new PolygonCutter();

        polygonCutter.CutPolygon(A, outCut);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        for (int i = 0; i < mines.Count; i++)
        {
            Gizmos.color = mines[i].color;
            Vector3 center = new Vector3(mines[i].position.x, mines[i].position.z, 0);
            Gizmos.DrawCube(center, Vector3.one / 2);
        }

        Gizmos.color = UnityEngine.Color.magenta;
        for (int i = 0; i < vertex.Count; i++)
        {
            Vector3 current = new Vector3(vertex[i].x,vertex[i].z);
            Vector3 next = new Vector3(vertex[(i + 1) % vertex.Count].x,vertex[(i + 1) % vertex.Count].z);
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
                array.Add(new Vector3(mines[i].poligon.vertices[k].x, mines[i].poligon.vertices[k].z, 0));
            }
            Handles.color = mines[i].color;
            Handles.DrawAAConvexPolygon(array.ToArray());
        }

        Gizmos.color = UnityEngine.Color.black;
        for (int j = 0; j < mines.Count; j++)
        {
            for (int i = 0; i < mines[j].poligon.vertices.Count; i++)
            {
                Vector3 vertex1 = new Vector3(mines[j].poligon.vertices[i].x,mines[j].poligon.vertices[i].z);
                Vector3 vertex2 = new Vector3(mines[j].poligon.vertices[(i + 1) % mines[j].poligon.vertices.Count].x,mines[j].poligon.vertices[(i + 1) % mines[j].poligon.vertices.Count].z);
                //Vector3 vertex1 = mines[j].poligon.vertices[i];
                //Vector3 vertex2 = mines[j].poligon.vertices[(i + 1) % mines[j].poligon.vertices.Count];

                Gizmos.DrawLine(vertex1, vertex2);
            }
        }
        
        Gizmos.color = Color.yellow;
        for (int i = 0; i < vertex.Count; i++)
        {
            Vector3 center = new Vector3(vertex[i].x, vertex[i].z);
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

public struct Side
{
    public Vector3 start;
    public Vector3 end;

    public Side(Vector3 start, Vector3 end)
    {
        this.start = start;
        this.end = end;
    }
}

[Serializable]
public struct Poligon
{
    public List<Vector3> vertices;

    public Poligon(List<Vector3> vertices)
    {
        this.vertices = vertices;
    }
}

[Serializable]
public class MinesTemp
{
    public Vector3 position;
    public Poligon poligon;
    public UnityEngine.Color color;

    public MinesTemp(Vector3 position, Poligon poligon)
    {
        this.position = position;
        this.poligon = poligon;
        color = new UnityEngine.Color(UnityEngine.Random.value, Random.value, Random.value, 1);
    }
}

public class PolygonCutter
{
    public void CutPolygon(MinesTemp mine, Side cut)
    {
        List<Vector3> polygon1 = new List<Vector3>();
        List<Vector3> polygon2 = new List<Vector3>();

        List<Vector3> intersections = new List<Vector3>();

        bool isFirstPolygon = true;


        for (int i = 0; i < mine.poligon.vertices.Count; i++)
        {
            Vector3 p1 = mine.poligon.vertices[i];
            Vector3 p2 = mine.poligon.vertices[(i + 1) % mine.poligon.vertices.Count];

            if (isFirstPolygon)
            {
                polygon1.Add(p1);
            }
            else
            {
                polygon2.Add(p1);
            }

            Side side1 = new Side(p1, p2);
            Side side2 = new Side(cut.start, cut.end);

            if (DetectarInterseccion(side1, side2, out Vector3 intersection))
            {
                if (!intersections.Contains(intersection) && intersections.Count < 2)
                {
                    intersections.Add(intersection);

                    polygon1.Add(intersection);
                    polygon2.Add(intersection);

                    isFirstPolygon = !isFirstPolygon;
                }
            }
        }

        if (intersections.Count == 2)
        {

            if (IsPointInPolygon(mine.position, new Poligon(polygon1)))
            {

                mine.poligon = new Poligon(polygon1);
            }
            else if (IsPointInPolygon(mine.position, new Poligon(polygon2)))
            {
                mine.poligon = new Poligon(polygon2);
            }
            else
            {
                Debug.LogError("POR FAVOR NO SALTES");
            }
        }
        else
        {
            Debug.LogWarning($"No se encontraron intersecciones o el poligono no fue cortado correctamente. {intersections.Count}");
        }
    }

    public static bool DetectarInterseccion(Side side1, Side side2, out Vector3 puntoInterseccion)
    {
        puntoInterseccion = new Vector3();

        float denominator = (side1.start.x - side1.end.x) * (side2.start.z - side2.end.z) -
                            (side1.start.z - side1.end.z) * (side2.start.x - side2.end.x);

        if (Mathf.Abs(denominator) < Mathf.Epsilon)
        {
            return false;
        }

        float t = ((side1.start.x - side2.start.x) * (side2.start.z - side2.end.z) -
                   (side1.start.z - side2.start.z) * (side2.start.x - side2.end.x)) / denominator;
        float u = ((side1.start.x - side2.start.x) * (side1.start.z - side1.end.z) -
                   (side1.start.z - side2.start.z) * (side1.start.x - side1.end.x)) / denominator;

        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            puntoInterseccion = new Vector3(
                side1.start.x + t * (side1.end.x - side1.start.x),
                0,
                side1.start.z + t * (side1.end.z - side1.start.z)
            );
            return true;
        }

        return false;
    }

    public bool IsPointInPolygon(Vector3 point, Poligon polygon)
    {
        int intersectCount = 0;

        for (int i = 0; i < polygon.vertices.Count; i++)
        {
            Vector3 vertex1 = polygon.vertices[i];
            Vector3 vertex2 = polygon.vertices[(i + 1) % polygon.vertices.Count];

            if (RayIntersectsSegment(point, vertex1, vertex2))
            {
                intersectCount++;
            }
        }

        return (intersectCount % 2) == 1;
    }

    public bool RayIntersectsSegment(UnityEngine.Vector3 point, UnityEngine.Vector3 v1, Vector3 v2)
    {
        if (v1.z > v2.z)
        {
            Vector3 temp = v1;
            v1 = v2;
            v2 = temp;
        }

        if (Mathf.Abs(point.z - v1.z) < 0.001f || Mathf.Abs(point.z - v2.z) < 0.001f)
        {
            point.z += 0.0001f;  
        }

        if (point.z < v1.z || point.z > v2.z)
        {
            return false;
        }

        if (point.x > Mathf.Max(v1.x, v2.x))
        {
            return false;
        }

        if (point.x < Mathf.Min(v1.x, v2.x))
        {
            return true;
        }

        float deltaZ = v2.z - v1.z;
        float deltaX = v2.x - v1.x;

        if (Mathf.Abs(deltaZ) < 0.001f)
        {
            return false;
        }

        float slope = deltaX / deltaZ;
        float xIntersect = v1.x + (point.z - v1.z) * slope;

        return point.x < xIntersect;
    }
}