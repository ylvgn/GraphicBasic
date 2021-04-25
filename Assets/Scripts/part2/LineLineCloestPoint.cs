using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineLineCloestPoint : MonoBehaviour
{
    public GameObject p1;
    public GameObject p2;
    public GameObject p3;
    public GameObject p4;

    // TODO
    void OnDrawGizmos()
    {
        // p1p2 -> ab, p3p4 -> cb
        if (p1 == null || p2 == null || p3 == null || p4 == null) return;
        var a = p1.transform.position;
        var b = p2.transform.position;
        var c = p3.transform.position;
        var d = p4.transform.position;

        // debug
        MyUtility.LogPoint(a, p1.name, Color.white);
        MyUtility.LogPoint(b, p2.name, Color.white);
        MyUtility.LogPoint(c, p3.name, Color.white);
        MyUtility.LogPoint(d, p4.name, Color.white);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(a, b);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(c, d);
    }
}
