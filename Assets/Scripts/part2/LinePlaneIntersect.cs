using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePlaneIntersect : MonoBehaviour
{
    public GameObject O, A, B;
    [Range(0, 100)]public float planeSize = 1f;
    public Color planeColor;
    public Color resultPointColor;

    private void OnDrawGizmos()
    {
        if (O == null || A == null || B == null) return;

        var o = O.transform.position;
        var a = A.transform.position;
        var b = B.transform.position;
        var nl = O.transform.up;

        var a2Plane = MyUtility.PointCast2Plane(o, nl, a);
        var b2Plane = MyUtility.PointCast2Plane(o, nl, b);
        
        // output
        var resultPoint = MyUtility.LineLineInterSect(a, b, a2Plane, b2Plane);
        Gizmos.color = resultPointColor;
        Gizmos.DrawSphere(resultPoint, 0.2f);

        // debug
        Gizmos.color = Color.white;
        MyUtility.LogPoint(o, $"{O.name}{o}", Color.white, 15);
        MyUtility.LogPoint(a, $"{A.name}{a}", Color.white, 15);
        MyUtility.LogPoint(b, $"{B.name}{b}", Color.white, 15);

        Gizmos.DrawSphere(a2Plane, 0.2f);
        Gizmos.DrawSphere(b2Plane, 0.2f);
        Gizmos.DrawLine(a2Plane, a);
        Gizmos.DrawLine(b, b2Plane);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(a2Plane, b2Plane);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(a, b);

        Gizmos.matrix = O.transform.localToWorldMatrix;
        Gizmos.color = planeColor;
        Gizmos.DrawCube(Vector3.zero, new Vector3(planeSize, 0.05f, planeSize));
    }
}
