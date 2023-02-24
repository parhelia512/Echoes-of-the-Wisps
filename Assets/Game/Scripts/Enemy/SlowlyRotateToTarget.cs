using UnityEngine;

public class SlowlyRotateToTarget : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Quaternion targetRot;
    private bool isRotating;




    private void OnEnable()
    {
        Actions.OnNoiseMade += StartRotating;
    }


    private void OnDisable()
    {
        Actions.OnNoiseMade -= StartRotating;
    }


    private void Start()
    {
        isRotating = false;
    }



    private void Update()
    {
        if (isRotating)
        {
            RotateToTarget(targetRot);

            if (transform.rotation == targetRot)
            {
                isRotating = false;
            }
        }
    }

    private void StartRotating(float noiseRange, Vector2 noisePos)
    {
        if (noiseRange <= Geist.instance.BigNoiseRange + 0.1f && noiseRange >= Geist.instance.BigNoiseRange - 0.1f)
        {
            if(Vector2.Distance(noisePos, transform.position) < noiseRange)
            {
                isRotating = true;
                var distance = new Vector3(noisePos.x - transform.position.x, noisePos.y - transform.position.y, 0f);
                targetRot =  Quaternion.LookRotation(distance) * Quaternion.Euler(0, 90f, 0);
                Debug.Log(distance);
            }
        }

    }


    private void RotateToTarget(Quaternion targetRot)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, speed * Time.deltaTime);
    }


}
