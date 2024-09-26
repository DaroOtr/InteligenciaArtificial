using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Voronoi
{
    [Serializable]
    public class MinesHolder
    {
        public Vector3 position;
        public Poligon poligon;
        public Color color;

        public MinesHolder(Vector3 position, Poligon poligon)
        {
            this.position = position;
            this.poligon = poligon;
            color = new UnityEngine.Color(UnityEngine.Random.value, Random.value, Random.value, 1);
        }
    }
}
