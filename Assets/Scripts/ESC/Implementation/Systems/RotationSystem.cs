using System.Collections.Generic;
using System.Threading.Tasks;
using ESC.Implementation.Components;
using ESC.Patron;

namespace ESC.Implementation.Systems
{
    public class RotationSystem : ECSSystem
    {
        private ParallelOptions _parallelOptions;
    
        private IDictionary<uint, RotationComponent> _rotationComponents;
        private IDictionary<uint, VelocityComponent> _velocityComponents;
        private IEnumerable<uint> _queryedEntities;

        public override void Initialize()
        {
            _parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 32 };
        }

        protected override void PreExecute(float deltaTime)
        {
            _rotationComponents??= EcsManager.GetComponents<RotationComponent>();
            //velocityComponents??= ECSManager.GetComponents<VelocityComponent>();
            _queryedEntities??= EcsManager.GetEntitiesWhitComponentTypes(typeof(RotationComponent), typeof(VelocityComponent));
        }

        protected override void Execute(float deltaTime)
        {
            Parallel.ForEach(_queryedEntities, _parallelOptions, i =>
            {
                _rotationComponents[i].RotationX +=  _rotationComponents[i].RotX * _rotationComponents[i].RotationVelocity * deltaTime;
                _rotationComponents[i].RotationY +=  _rotationComponents[i].RotY * _rotationComponents[i].RotationVelocity * deltaTime;
                _rotationComponents[i].RotationZ +=  _rotationComponents[i].RotZ * _rotationComponents[i].RotationVelocity * deltaTime;
            });
        }

        protected override void PostExecute(float deltaTime)
        {
        }
    
    }
}