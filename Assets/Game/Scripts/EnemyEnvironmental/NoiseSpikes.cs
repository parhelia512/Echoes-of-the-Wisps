using UnityEngine;

public class NoiseSpikes : MonoBehaviour
{
    [SerializeField] private float speed;
    private Transform target;
    private Vector3 startPosition;

    [SerializeField] private GameObject noiseDetector;


    [SerializeField] private float chaseDuration;
    private float chaseDurationCounter;


    private bool isActive = false;

    private void OnEnable()
    {
        Actions.OnNoiseMade += GetDistractedByNoise;
    }


    private void OnDisable()
    {
        Actions.OnNoiseMade -= GetDistractedByNoise;
    }

    private void Start()
    {
        startPosition = transform.position;
        chaseDurationCounter = chaseDuration;
    }



    void Update()
    {
        if (isActive)
        {
            if (chaseDurationCounter <= 0f)
            {
                isActive = false;
                chaseDurationCounter = chaseDuration;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                chaseDurationCounter -= Time.deltaTime; 
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
        }
    }


    private void GetDistractedByNoise(float noiseRange, Vector2 noisePos)
    {
        for (int i = 0; i < noiseDetector.gameObject.transform.childCount; i++)
        {
            if (Vector2.Distance(noiseDetector.gameObject.transform.GetChild(i).gameObject.transform.position, Geist.instance.transform.position) < noiseRange)
            {
                ReactToNoise(noiseRange);
                return;
            }
        }
    }



    private void ReactToNoise(float noiseRange)
    {

        if (Geist.instance.SmallNoiseRange - 0.1f <= noiseRange && noiseRange <= Geist.instance.SmallNoiseRange + 0.1f)
        {
            Debug.Log("small noise heard");
        }



        if (Geist.instance.BigNoiseRange - 0.1f <= noiseRange && noiseRange <= Geist.instance.BigNoiseRange + 0.1f)
        {
            Debug.Log("big noise heard");
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            isActive = true;
        }
    }


}
