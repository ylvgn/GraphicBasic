using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineLineCloestPoint : MonoBehaviour
{
    public GameObject p1;
    public GameObject p2;
    public GameObject p3;
    public GameObject p4;


    // 计算p投影到平面的p'
    Vector3 PointProject2Plane(Vector3 center, Vector3 nl, Vector3 p)
    {
        var pcDotnl = Vector3.Dot((center - p), nl);
        return p + pcDotnl * nl;
    }

    // (同一个平面上)直线ab和直线cd的交点o
    Vector3 LineLineIntersectPoint(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        var S_cde = Vector3.Cross(a - b, c - d); // 过c点作ab//ce, 得到三角形cde
        var S_acb = Vector3.Cross(a - c, c - b);
        var coOvercd = Vector3.Dot(S_acb, S_cde) / Vector3.Dot(S_cde, S_cde); // Sacb面积: Scde = co:cd
        var o = c + (d - c) * coOvercd;
        return o;
    }

    void OnDrawGizmos()
    {
        // p1p2 -> ab, p3p4 -> cb
        if (p1 == null || p2 == null || p3 == null || p4 == null) return;
        var a = p1.transform.position;
        var b = p2.transform.position;
        var c = p3.transform.position;
        var d = p4.transform.position;
        var ba = (a - b).normalized;
        var dc = (c - d).normalized;
        var nl = Vector3.Cross(dc, ba).normalized;

        // cd -> c'd', 令abc'd'在同一个平面上
        var c2planeAB = PointProject2Plane(a, nl, c); // c'
        var d2planeAB = PointProject2Plane(a, nl, d); // d'
        
        // output
        var o_onAB = LineLineIntersectPoint(a, b, c2planeAB, d2planeAB); // o_onAB是直线c'd'和ab的交点
        Vector3 o_onCD;
        float outputDistance;
        var resultNearestPoint = "O";
        LineClosestPoint.GetPointToLineNearestDistance(c, d, o_onAB, out o_onCD, out outputDistance);
        var resultNearestPointPos = o_onCD;

        // 若o在ab线段外, 比较oo'的距离 和 c, d分别到ab直线距离
        Vector3 t1, t2;
        float dist_c2ab, dist_d2ab;
        LineClosestPoint.GetPointToLineNearestDistance(a, b, c, out t1, out dist_c2ab);
        LineClosestPoint.GetPointToLineNearestDistance(a, b, d, out t2, out dist_d2ab);
        if (dist_c2ab <= outputDistance) // 若c到ab距离更短, 则取c点作为直线ab最近点
        {
            outputDistance = dist_c2ab;
            resultNearestPoint = "C";
            resultNearestPointPos = c;
        }
        if (dist_d2ab <= outputDistance) // 若d到ab距离更短, 则取c点作为直线ab最近点
        {
            outputDistance = dist_d2ab;
            resultNearestPoint = "D";
            resultNearestPointPos = d;
        }

        MyUtility.LogPoint(o_onAB, $"o'{o_onAB}", Color.white, 15);
        MyUtility.LogPoint(o_onCD, $"o{o_onCD}", Color.white, 15);
        MyUtility.LogPoint(new Rect(0, 0, 100, 30), $"outputDistance={outputDistance}", Color.black);
        MyUtility.LogPoint(new Rect(0, 40, 100, 30), $"resultPoint={resultNearestPoint}{resultNearestPointPos}", Color.black);
        MyUtility.LogPoint(new Rect(0, 80, 100, 30), $"==============================", Color.black, 20);
        MyUtility.LogPoint(new Rect(0, 105, 100, 30), $"BA-Dot-DC   = {Vector3.Dot(ba, dc)}", Color.black, 20);
        MyUtility.LogPoint(new Rect(0, 135, 100, 30), $"BA-Cross-DC= {nl}", Color.black, 20);
        MyUtility.LogPoint(new Rect(0, 160, 100, 30), $"==============================", Color.black, 20);

        // debug: ab, cd
        MyUtility.LogPoint(a, $"{p1.name}{a}", Color.white, 15);
        MyUtility.LogPoint(b, $"{p2.name}{b}", Color.white, 15);
        MyUtility.LogPoint(c, $"{p3.name}{c}", Color.white, 15);
        MyUtility.LogPoint(d, $"{p4.name}{d}", Color.white, 15);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(a, b);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(c, d);
        Gizmos.color = Color.white;

        // debug: c' d'
        Gizmos.DrawSphere(c2planeAB, 0.2f);
        MyUtility.LogPoint(c2planeAB, $"C'{c2planeAB}", Color.white, 15);
        Gizmos.DrawSphere(d2planeAB, 0.2f);
        MyUtility.LogPoint(d2planeAB, $"D'{d2planeAB}", Color.white, 15);

        // debug: c'a, c'b, d'a, d'b
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(c2planeAB, d2planeAB);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(c2planeAB, a);
        Gizmos.DrawLine(c2planeAB, b);
        Gizmos.DrawLine(d2planeAB, a);
        Gizmos.DrawLine(d2planeAB, b);

        // debug: oo'
        Gizmos.color = new Color(0, 1, 1, 1);
        Gizmos.DrawSphere(o_onAB, 0.2f);
        Gizmos.DrawSphere(o_onCD, 0.2f);
        Gizmos.DrawLine(o_onAB, o_onCD);

        // debug: ab平面, cd平面
        //DrawPlane(a, nl, (b - a).magnitude + (c - d).magnitude, Color.red);
        //DrawPlane(c, nl, (c - d).magnitude + (b - a).magnitude, Color.green);
    }

    void DrawPlane(Vector3 center, Vector3 nl, float size, Color c)
    {
        var oldColor = Gizmos.color;
        Gizmos.color = c;
        int[] dx = new int[4] { 90, 0, -90, 0 };
        int[] dz = new int[4] { 0, -90, 0, 90 };
        Vector3 last = center;
        Vector3 first = center;
        for (int i = 0; i < 4; i++)
        {
            var rotation = Quaternion.Euler(dx[i], 0, dz[i]);
            var t = rotation * nl;
            var cur = center + t * size;
            if (last != center) Gizmos.DrawLine(last, cur);
            else first = cur;
            last = cur;
        }
        Gizmos.DrawLine(last, first);
        Gizmos.color = oldColor;
    }
}
