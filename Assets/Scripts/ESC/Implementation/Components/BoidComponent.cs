using ESC.Patron;

namespace ESC.Implementation.Components
{
    public abstract class BoidComponent : EcsComponent
    {
        private uint _boidRad = 0;

        public uint BoidRad { get => _boidRad; set => _boidRad = value; }
    }
}