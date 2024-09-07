using System.Collections.Generic;
using System.Threading.Tasks;
using ESC.Implementation.Components;
using ESC.Patron;

namespace ESC.Implementation.Systems
{
    public class AlignmentSystem : EcsBoidSystem<AlignmentComponent>
    {
        private ParallelOptions _parallelOptions;

        private IDictionary<uint, AlignmentComponent> _alignmentComponents;
        private IDictionary<uint, PositionComponent> _positionComponents;
        private IEnumerable<uint> _queryedEntities;
        private IEnumerable<uint> _boidsInRad;
        
        public override void Initialize()
        {
            _parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 32 };
        }

        protected override void PreExecute(float deltaTime)
        {
            _alignmentComponents??= EcsManager.GetComponents<AlignmentComponent>();
            _positionComponents??= EcsManager.GetComponents<PositionComponent>();
            _queryedEntities??= EcsManager.GetEntitiesWhitComponentTypes(typeof(AlignmentComponent));
            _boidsInRad??= EcsManager.GetEntitiesWhitComponentTypes(typeof(AlignmentComponent),typeof(PositionComponent));
        }

        protected override void Execute(float deltaTime)
        {
            ICollection<AlignmentComponent> boids = GetBoidsInsideRadius();
            
            if (boids.Count == 0)   
                return;  
            
            Parallel.ForEach(_boidsInRad, _parallelOptions, i =>
            {
                Parallel.ForEach(_boidsInRad, _parallelOptions, j =>
                {
                    if (boids.Contains(_alignmentComponents[i]) && boids.Contains(_alignmentComponents[j]))
                    {
                        if (_alignmentComponents[i] != _alignmentComponents[j])
                        {
                            _alignmentComponents[i].AvgX += _positionComponents[j].X;
                            _alignmentComponents[i].AvgY += _positionComponents[j].Y;
                            _alignmentComponents[i].AvgZ += _positionComponents[j].Z;
                        }
                    }
                });
            });
        }

        protected override void PostExecute(float deltaTime)
        {
            ICollection<AlignmentComponent> boids = GetBoidsInsideRadius();
            
            if (boids.Count == 0)   
                return;  
            Parallel.ForEach(_queryedEntities, _parallelOptions, i =>
            {
                _alignmentComponents[i].AvgX /= boids.Count;
                _alignmentComponents[i].AvgY /= boids.Count;
                _alignmentComponents[i].AvgZ /= boids.Count;
            });
        }
        
        public override ICollection<AlignmentComponent> GetBoidsInsideRadius()
        {
            ICollection<AlignmentComponent> insideRadiusBoids = new List<AlignmentComponent>();

            Parallel.ForEach(_boidsInRad, _parallelOptions, i =>
            {
                Parallel.ForEach(_boidsInRad, _parallelOptions, j =>
                {
                    if (_positionComponents[i] != _positionComponents[j])
                    {
                        float distance = 0;
                        distance += _positionComponents[i].X - _positionComponents[j].X;
                        distance += _positionComponents[i].Y - _positionComponents[j].Y;
                        distance += _positionComponents[i].Z - _positionComponents[j].Z;
                        
                        if (distance < _alignmentComponents[i].BoidRad)
                            insideRadiusBoids.Add(_alignmentComponents[j]);
                    }
                });
            });
            return insideRadiusBoids;
        }
    }
}