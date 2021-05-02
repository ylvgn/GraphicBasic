using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTriangleIntersect : MonoBehaviour
{
    public GameObject p1, p2, p3, p4, p5;
    public Color InterSectPointColor;
    private Vector3 nl; // p1,p2,p3的normal

    bool CheckIsSameSide(Vector3 a, Vector3 b, Vector3 p)
    {
        var check_nl = Vector3.Cross(b - a, p - a);
        return Vector3.Dot(nl, check_nl) > 0;
    }

    bool CheckIsInTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 p)
    {
        return (CheckIsSameSide(a, b, p) && CheckIsSameSide(b, c, p) && CheckIsSameSide(c, a, p));
    }

    void OnDrawGizmos()
    {
        if (p1 == null || p2 == null || p3 == null) return;
        if (p4 == null || p5 == null) return;
        var a = p1.transform.position;
        var b = p2.transform.position;
        var c = p3.transform.position;
        var d = p4.transform.position;
        var e = p5.transform.position;
        nl = Vector3.Cross(b - a, c - a).normalized;
        var o = new Vector3((a.x+b.x+c.x) / 3f, (a.y+b.y+c.y)/3f, (a.z+b.z+c.z) / 3f);

        var d_onABC = d + Vector3.Dot((o - d), nl) * nl;
        var e_onABC = e + Vector3.Dot((o - e), nl) * nl;
        var target = MyUtility.LineLineInterSect(d, e, d_onABC, e_onABC);

        // output
        if (CheckIsInTriangle(a, b, c, target))
        {
            Gizmos.color = InterSectPointColor;
            Gizmos.DrawSphere(target, 0.2f);
        }

        // debug
        Gizmos.color = Color.black;
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(a, c);
        Gizmos.DrawLine(b, c);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(d, e);
        
        //Gizmos.DrawLine(o, o + nl*5f);
        MyUtility.LogPoint(a, $"{p1.name}{a}", Color.white, 15);
        MyUtility.LogPoint(b, $"{p2.name}{b}", Color.white, 15);
        MyUtility.LogPoint(c, $"{p3.name}{c}", Color.white, 15);
        MyUtility.LogPoint(d, $"{p4.name}{d}", Color.white, 15);
        MyUtility.LogPoint(e, $"{p5.name}{e}", Color.white, 15);
        Gizmos.DrawSphere(o, 0.15f);
        MyUtility.LogPoint(o, $"o{o}", Color.white, 15);

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(d_onABC, 0.2f);
        Gizmos.DrawSphere(e_onABC, 0.2f);
        Gizmos.DrawLine(d, d_onABC);
        Gizmos.DrawLine(e, e_onABC);
        MyUtility.LogPoint(d_onABC, $"d'{d_onABC}", Color.white, 15);
        MyUtility.LogPoint(e_onABC, $"e'{e_onABC}", Color.white, 15);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(d_onABC, e_onABC);
    }
}
