using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voronoi
{
    [Serializable]
    public struct Poligon
    {
        public List<Vector3> vertices;

        public Poligon(List<Vector3> vertices)
        {
            this.vertices = vertices;
        }
    }
}
