

public class RotationComponent : ECSComponent
{
    public float rotationVelocity;
    public float rotX;
    public float rotY;
    public float rotZ;
    
    public float rotationX;
    public float rotationY;
    public float rotationZ;
    
    public RotationComponent(float rotationVelocity, float rotationX,float rotationY,float rotationZ) 
    {
        this.rotationVelocity = rotationVelocity;
        rotX = rotationX;
        rotY = rotationY;
        rotZ = rotationZ;
    }
    
}