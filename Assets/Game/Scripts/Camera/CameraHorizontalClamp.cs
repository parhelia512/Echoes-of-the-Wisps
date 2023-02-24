using UnityEngine;

public class CameraHorizontalClamp : MonoBehaviour
{
    public static CameraHorizontalClamp instance;

    [SerializeField] private Transform targetToFollow;


    private void Awake()
    {
        instance = this;
    }

    //this is only used for horizontal camera clamping. Do vertical clamping by 
    void Update()
    {
        if (targetToFollow != null)
        {
            transform.position = new Vector3(Mathf.Clamp(targetToFollow.position.x, -1000f, 1000f), 0f, transform.position.z);
        }
    }
}
