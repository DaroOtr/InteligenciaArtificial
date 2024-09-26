using UnityEngine;

namespace Voronoi
{
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
}
