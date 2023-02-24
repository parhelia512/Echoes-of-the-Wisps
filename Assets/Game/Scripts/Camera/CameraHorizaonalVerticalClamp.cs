using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHorizaonalVerticalClamp : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public Vector3 cameraOffset;
    public float speed;
    Vector3 velocity = Vector3.zero;

    [Header("Camera Min/Max positions")]
    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;

    private void Update()
    {
        float xClamp = Mathf.Clamp(target.position.x, xMin, xMax);
        float yClamp = Mathf.Clamp(target.position.y, yMin, yMax);

        Vector3 targetpos = target.position + cameraOffset;
        Vector2 clampedpos = new Vector3(Mathf.Clamp(targetpos.x, xMin, xMax), Mathf.Clamp(targetpos.y, yMin, yMax), -1);

        Vector3 smoothpos = Vector3.SmoothDamp(transform.position, clampedpos, ref velocity, speed * Time.deltaTime);

        transform.position = smoothpos;
    }
}
