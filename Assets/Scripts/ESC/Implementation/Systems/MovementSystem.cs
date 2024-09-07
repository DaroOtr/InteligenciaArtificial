using System.Collections.Generic;
using System.Threading.Tasks;
using ESC.Implementation.Components;
using ESC.Patron;

namespace ESC.Implementation.Systems
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
                _positionComponents[i].X += _velocityComponents[i].DirectionX * _velocityComponents[i].Velocity * deltaTime;
                _positionComponents[i].Y += _velocityComponents[i].DirectionY * _velocityComponents[i].Velocity * deltaTime;
                _positionComponents[i].Z += _velocityComponents[i].DirectionZ * _velocityComponents[i].Velocity * deltaTime;
            });
        }

        protected override void PostExecute(float deltaTime)
        {
        }
    }
}
