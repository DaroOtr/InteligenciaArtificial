using UnityEngine;

public class Tank : TankBase
{
    float fitness = 0;
    private float goodmines = 0;
    private float badmines = 0;
    private float outputDifference = 0;
    private float distanceModifier = 1;
    private float mineModifier = 1;

    protected override void OnReset()
    {
        fitness = 1;
    }

    protected override void OnThink(float dt)
    {
        Vector3 dirToMine = GetDirToMine(nearMine);

        inputs[0] = dirToMine.x;
        inputs[1] = dirToMine.z;
        inputs[2] = transform.forward.x;
        inputs[3] = transform.forward.z;

        float[] output = brain.Synapsis(inputs);
        outputDifference = Mathf.Abs(output[0] - (output[1]));
        
        
        if (outputDifference <= -0.2f || outputDifference >= 0.2f)
            distanceModifier *= 0.9f;
        else
            distanceModifier *= 1.1f;

        if (outputDifference >= 2)
            outputDifference = 2;
        if (outputDifference <= 0)
            outputDifference = 0;

        SetForces(output[0], output[1], dt);
        fitness += 10 * distanceModifier;
    }

    protected override void OnTakeMine(GameObject mine)
    {
        if (IsGoodMine(mine))
            mineModifier *= 1.1f;
        if (IsBadMine(mine))
            mineModifier *= 0.9f;

        if (mineModifier >= 2)
            mineModifier = 2;
        if (mineModifier <= 0)
            mineModifier = 0;

        fitness += 10 * mineModifier;
        genome.fitness = fitness;
    }
}