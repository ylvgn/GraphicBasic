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
        MyUtility.LogPoint(a + Vector3.up, "A", Color.white);
        MyUtility.LogPoint(b + Vector3.up, "B", Color.white);
        MyUtility.LogPoint(c + Vector3.up, "C", Color.white);
        PointToLineNearestDistance(a, b, c);
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
}
