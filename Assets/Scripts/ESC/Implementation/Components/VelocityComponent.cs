using ESC.Patron;

namespace ESC.Implementation.Components
{
    public class VelocityComponent : EcsComponent
    {
        public float Velocity;

        public float DirectionX;
        public float DirectionY;
        public float DirectionZ;

        public VelocityComponent(float velocity, float directionX, float directionY, float directionZ)
        {
            Velocity = velocity;
            DirectionX = directionX;
            DirectionY = directionY;
            DirectionZ = directionZ;
        }
    }
}
