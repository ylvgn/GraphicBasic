using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineClosestPoint : MonoBehaviour
{
    public GameObject p1;
    public GameObject p2;
    public GameObject p;
    public Color lineColor;
    public Color normalLineColor;
    public Color outputLineColor;


    void OnDrawGizmos()
    {
        var a = p1.transform.position;
        var b = p2.transform.position;
        var c = p.transform.position;
        
        Gizmos.color = lineColor;
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(a, c);
        MyUtility.LogPoint(a, "A", Color.white);
        MyUtility.LogPoint(b, "B", Color.white);
        MyUtility.LogPoint(c, "C", Color.white);
        PointToLineNearestDistance(a, b, c);
        PointToLineNearestDistance2(a, b, c);
    }
    
    // c到直线ab的距离
    void PointToLineNearestDistance(Vector3 a, Vector3 b, Vector3 c)
    {
        // |v1 x v2| = |v1||v2|sin<v1, v2>
        var v1 = a - c;
        var v2 = (b - a).normalized;
        var nl = Vector3.Cross(v1, v2);
        var outputLine = Vector3.Cross(nl, (a-b).normalized);
        var distance = nl.magnitude;

        // output
        MyUtility.LogPoint(new Rect(0, 0, 20, 20), $"distance={distance}", Color.white);
        var m = c + outputLine;

        // clamp
        if ((m-b).sqrMagnitude + (m-a).sqrMagnitude > (a-b).sqrMagnitude)
        {
            if ((c-a).sqrMagnitude < (c - b).sqrMagnitude) m = a;
            else m = b;
            outputLine = m - c;
        }

        MyUtility.LogPoint(m, "M", Color.white);
        Gizmos.color = outputLineColor;
        Gizmos.DrawLine(c, m);
        
        Gizmos.matrix = p.transform.localToWorldMatrix;
        Gizmos.color = normalLineColor;
        Gizmos.DrawLine(Vector3.zero, nl);
        Gizmos.matrix = Matrix4x4.identity;

        MyUtility.LogPoint(c + outputLine * 0.5f, distance.ToString(), Color.white, 15);
        MyUtility.LogPoint(c + v1 * 0.5f, v1.magnitude.ToString(), Color.white, 15);
        MyUtility.LogPoint(m + (a-m) * 0.5f, (a-m).magnitude.ToString(), Color.white, 15);
    }

    // dot product
    void PointToLineNearestDistance2(Vector3 a, Vector3 b, Vector3 c)
    {
        var CA = c-a;
        var AB = (b - a).normalized;
        var t = Vector3.Dot(CA, AB);
        t = MyUtility.Clamp(t, 0, (b-a).magnitude);
        var m = t * AB + a;
        var distance = Vector3.Distance(c, m);

        // output
        MyUtility.LogPoint(new Rect(0, 40, 20, 20), $"distance2={distance}", Color.white);
        MyUtility.LogPoint(m + Vector3.up, "M2", Color.white);
        Gizmos.color = outputLineColor;
        Gizmos.DrawLine(c, m);
    }
}
