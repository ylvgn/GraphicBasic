using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCameraController : MonoBehaviour
{
    public Camera MyCamera;

    // debug
    Vector3 mouseLeft;
    Vector3 mouseRight;
    Vector3 mouseMiddle;

    void Update()
    {
        if (MyCamera == null) return;
        MoveCamera();
    }

    void MoveCamera()
    {
        // offset
        if (Input.GetMouseButtonDown(2))
            mouseMiddle = MyCamera.transform.position;
        if (Input.GetMouseButton(2))
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");
            mouseMiddle.x += x;
            mouseMiddle.y += y;
            MyCamera.transform.position = Vector3.Lerp(MyCamera.transform.position, mouseMiddle, Time.deltaTime * 10);
        }

        // Rotate
        if (Input.GetMouseButtonDown(1))
            mouseRight = MyCamera.transform.eulerAngles;
        if (Input.GetMouseButton(1))
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");
            mouseRight.y += x;
            mouseRight.x += -y;
            MyCamera.transform.rotation = Quaternion.Lerp(MyCamera.transform.rotation, Quaternion.Euler(mouseRight), Time.deltaTime * 20);
        }

        // FOV
        if (Input.GetMouseButtonDown(0))
            mouseLeft = Input.mousePosition;

        if (Input.GetMouseButton(0))
        {
            var dir = Input.mousePosition - mouseLeft;
            var speed = Mathf.Clamp(dir.y, -10, 10);
            var fov = MyCamera.fieldOfView + speed;
            MyCamera.fieldOfView = Mathf.Lerp(MyCamera.fieldOfView, fov, Time.deltaTime);
            mouseLeft = Input.mousePosition;
        }
    }
}
