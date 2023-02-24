using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] private float autoDestroyTime;

    private void Awake()
    {
        Destroy(gameObject, autoDestroyTime);
    }
}
