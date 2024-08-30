using System.Collections.Generic;
using System.Threading.Tasks;

public class RotationSystem : ECSSystem
{
    
    private ParallelOptions parallelOptions;
    
    private IDictionary<uint, RotationComponent> rotationComponents;
    private IDictionary<uint, VelocityComponent> velocityComponents;
    private IEnumerable<uint> queryedEntities;

    public override void Initialize()
    {
        parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 32 };
    }

    protected override void PreExecute(float deltaTime)
    {
        rotationComponents??= ECSManager.GetComponents<RotationComponent>();
        //velocityComponents??= ECSManager.GetComponents<VelocityComponent>();
        queryedEntities??= ECSManager.GetEntitiesWhitComponentTypes(typeof(RotationComponent), typeof(VelocityComponent));
    }

    protected override void Execute(float deltaTime)
    {
        Parallel.ForEach(queryedEntities, parallelOptions, i =>
        {
            rotationComponents[i].rotationX +=  rotationComponents[i].rotX * rotationComponents[i].rotationVelocity * deltaTime;
            rotationComponents[i].rotationY +=  rotationComponents[i].rotY * rotationComponents[i].rotationVelocity * deltaTime;
            rotationComponents[i].rotationZ +=  rotationComponents[i].rotZ * rotationComponents[i].rotationVelocity * deltaTime;
        });
    }

    protected override void PostExecute(float deltaTime)
    {
    }
    
}