

using ESC.Patron;

namespace ESC.Implementation.Components
{
    public class RotationComponent : EcsComponent
    {
        public float RotationVelocity;
        public float RotX;
        public float RotY;
        public float RotZ;
    
        public float RotationX;
        public float RotationY;
        public float RotationZ;
    
        public RotationComponent(float rotationVelocity, float rotationX,float rotationY,float rotationZ) 
        {
            RotationVelocity = rotationVelocity;
            RotX = rotationX;
            RotY = rotationY;
            RotZ = rotationZ;
        }
    
    }
}