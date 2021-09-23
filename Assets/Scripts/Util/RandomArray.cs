using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomArray : MonoBehaviour
{
    internal static T GetRandom<T>(params T[] Params)
    {
        return Params[Random.Range(0, Params.Length)];
    }

    internal static T GetRandom<T>(List<T> Params)
    {
        return Params[Random.Range(0, Params.Count)];
    }

    internal static bool Probability(float fPercent)
    {
        float fProbabilityRate = UnityEngine.Random.value * 100.0f;

        if (fPercent == 100.0f && fProbabilityRate == fPercent)
        {
            return true;
        }
        else if (fProbabilityRate < fPercent)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
