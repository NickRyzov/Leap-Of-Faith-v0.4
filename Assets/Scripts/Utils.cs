using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    //Рассчет позиции по трем точкам Безье
    public Vector2 Bezier(float u, List<Vector2> points)
    {
        if (points.Count == 1) return points[0];

        Vector2 p01, p12, p012;

        p01 = (1 - u) * points[0] + u * points[1];
        p12 = (1 - u) * points[1] + u * points[2];
        p012 = (1 - u) * p01 + u * p12;

        return p012;
    }
}
