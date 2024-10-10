using UnityEngine;

public class BirdAI : BirdBase
{
    protected override void OnThink(float dt, BirdBehaviour birdBehaviour, Obstacle obstacle,Coin coin)
    {
        float[] inputs = new float[4];
        inputs[0] = (obstacle.transform.position - birdBehaviour.transform.position).x / 10.0f;
        inputs[1] = (obstacle.transform.position - birdBehaviour.transform.position).y / 10.0f;
        inputs[2] = (coin.transform.position - birdBehaviour.transform.position).x / 10.0f;
        inputs[3] = (coin.transform.position - birdBehaviour.transform.position).y / 10.0f;

        float[] outputs;
        outputs = brain.Synapsis(inputs);
        if (outputs[0] < 0.5f)
        {
            birdBehaviour.Flap();
        }

        float obstacleDistance = Vector2.Distance(obstacle.transform.position, birdBehaviour.transform.position);
        float coinDistance = Vector2.Distance(coin.transform.position, birdBehaviour.transform.position);

        if (obstacleDistance <= 1.0f)
            genome.fitness += 2;
        if (coinDistance <= 1.0f)
            genome.fitness *= 2;

        genome.fitness += (10.0f - obstacleDistance);
        genome.fitness += (10.0f - coinDistance);
    }

    protected override void OnDead()
    {
    }

    protected override void OnReset()
    {
        genome.fitness = 0.0f;
    }
}
