using System.Collections.Generic;
using System.Threading.Tasks;
using ESC.Patron;

namespace ESC.Implementation
{
    public sealed class MovementSystem : ECSSystem
    {
        private ParallelOptions _parallelOptions;

        private IDictionary<uint, PositionComponent> _positionComponents;
        private IDictionary<uint, VelocityComponent> _velocityComponents;
        private IEnumerable<uint> _queryedEntities;

        public override void Initialize()
        {
            _parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 32 };
        }

        protected override void PreExecute(float deltaTime)
        {
            _positionComponents??= EcsManager.GetComponents<PositionComponent>();
            _velocityComponents??= EcsManager.GetComponents<VelocityComponent>();
            _queryedEntities??= EcsManager.GetEntitiesWhitComponentTypes(typeof(PositionComponent), typeof(VelocityComponent));
        }

        protected override void Execute(float deltaTime)
        {
            Parallel.ForEach(_queryedEntities, _parallelOptions, i =>
            {
                _positionComponents[i].X += _velocityComponents[i].directionX * _velocityComponents[i].velocity * deltaTime;
                _positionComponents[i].Y += _velocityComponents[i].directionY * _velocityComponents[i].velocity * deltaTime;
                _positionComponents[i].Z += _velocityComponents[i].directionZ * _velocityComponents[i].velocity * deltaTime;
            });
        }

        protected override void PostExecute(float deltaTime)
        {
        }
    }
}
