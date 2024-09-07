using System.Numerics;
using ESC.Patron;

namespace ESC.Implementation.Components
{
    public class AlignmentComponent : BoidComponent
    {
        public float AvgX;
        public float AvgY;
        public float AvgZ;
        public float DirectionX;
        public float DirectionY;
        public float DirectionZ;

        public AlignmentComponent(float avgX, float avgY,float avgZ,float directionX,float directionY,float directionZ)
        {
            AvgX = avgX;
            AvgY = avgY;
            AvgZ = avgZ;
            DirectionX = directionX;
            DirectionY = directionY;
            DirectionZ = directionZ;
        }
    }
}