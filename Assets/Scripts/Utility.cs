using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utility
{
    public static void ShuffleList<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);

            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}