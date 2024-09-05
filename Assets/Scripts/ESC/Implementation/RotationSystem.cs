using System.Collections.Generic;
using System.Threading.Tasks;
using ESC.Patron;

namespace ESC.Implementation
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
                _rotationComponents[i].rotationX +=  _rotationComponents[i].rotX * _rotationComponents[i].rotationVelocity * deltaTime;
                _rotationComponents[i].rotationY +=  _rotationComponents[i].rotY * _rotationComponents[i].rotationVelocity * deltaTime;
                _rotationComponents[i].rotationZ +=  _rotationComponents[i].rotZ * _rotationComponents[i].rotationVelocity * deltaTime;
            });
        }

        protected override void PostExecute(float deltaTime)
        {
        }
    
    }
}