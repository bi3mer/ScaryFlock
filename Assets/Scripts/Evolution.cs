using UnityEngine;

public static class Evolution 
{
    public const float MutationLikelihood = 0.15f;
    public static bool ShouldMutate => Random.Range(0f, 1f) < MutationLikelihood;

    public static void Run(FlockingAgent child, OnUpdateFlockingAgent parent1, OnUpdateFlockingAgent parent2)
    {
        float[] parent1Weights = parent1.Weights;
        float[] parent2Weights = parent2.Weights;
        float[] parameters = new float[FlockingAgent.ParameterSpace];

        float parent1Likelihood = parent1.MatedCount / (float) (parent1.MatedCount + parent2.MatedCount);

        for (int i = 0; i < FlockingAgent.ParameterSpace; ++i)
        {
            float maxVal;
            if (i < 6)
            {
                maxVal = parent1.MaxRadius;
            }
            else

            {
                maxVal = parent2.MaxWeight;
            }

            if (ShouldMutate)
            {
                parameters[i] = Random.Range(0f, maxVal);
            }
            else if (Random.Range(0f, 1f) < parent1Likelihood)
            {
                parameters[i] = parent1Weights[i];
            }
            else
            {
                parameters[i] = parent2Weights[i];
            }
        }

        child.UpdateWeights(parameters);
    }
}
