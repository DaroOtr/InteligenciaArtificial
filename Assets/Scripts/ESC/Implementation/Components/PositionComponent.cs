using ESC.Patron;

namespace ESC.Implementation.Components
{
    public class PositionComponent : EcsComponent
    {
        public float X;
        public float Y;
        public float Z;

        public PositionComponent(float x, float y, float z) 
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
