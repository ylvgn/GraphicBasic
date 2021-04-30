using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSphereIntersect : MonoBehaviour
{
    public GameObject O, A, B;
    [Range(0, 100)]public float radius;
    public Color SphereColor;
    public Color resultPointColor;

    void OnDrawGizmos()
    {
        if (O == null || A == null || B == null) return;
        var o = O.transform.position;
        var a = A.transform.position;
        var b = B.transform.position;

        var ab = (b - a).normalized;
        var d = Vector3.Dot(ab, o - a);
        var m = a + d * ab;
        var distance2 = (o - m).sqrMagnitude;
        var u2 = radius * radius - distance2;
        var u = Mathf.Sqrt(u2);
        var b2Sphere = m + ab * u;
        var a2Sphere = m - ab * u;

        // output
        Gizmos.color = resultPointColor;
        Gizmos.DrawSphere(a2Sphere, 0.1f);
        Gizmos.DrawSphere(b2Sphere, 0.1f);
        MyUtility.LogPoint(a2Sphere, $"a'{a2Sphere}", Color.white, 15);
        MyUtility.LogPoint(b2Sphere, $"b'{b2Sphere}", Color.white, 15);
        MyUtility.LogPoint((m + a2Sphere) / 2, $"d={u}", Color.white, 15);

        // debug
        Gizmos.color = Color.white;
        MyUtility.LogPoint(o, $"{O.name}{o}", Color.white, 15);
        MyUtility.LogPoint(a, $"{A.name}{a}", Color.white, 15);
        MyUtility.LogPoint(b, $"{B.name}{b}", Color.white, 15);
        MyUtility.LogPoint(m, $"m{m}", Color.white, 15);
        Gizmos.DrawLine(a, b);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(m, 0.2f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(o, m);

        MyUtility.LogPoint(new Rect(0, 0, 20, 30), $"OM={Mathf.Sqrt(distance2)}", Color.white, 40);
        Gizmos.color = SphereColor;
        Gizmos.matrix = O.transform.localToWorldMatrix;
        Gizmos.DrawSphere(Vector3.zero, radius);
    }
}
