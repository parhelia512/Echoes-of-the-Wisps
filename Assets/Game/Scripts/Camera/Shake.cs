
using System.Collections;
using UnityEngine;

public class Shake : MonoBehaviour
{
    private bool hasStarted;
    [SerializeField] private float duration;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private AudioSource earthquakeSfx;


    private void OnEnable()
    {
        Actions.OnEnemySpawning += ReactToEnemySpawning;
    }


    private void OnDisable()
    {
        Actions.OnEnemySpawning -= ReactToEnemySpawning;
    }






    private void Update()
    {
        if (hasStarted)
        {
            hasStarted = false;
            StartCoroutine(ShakingCoroutine());
        }
    }



    private void ReactToEnemySpawning()
    {

        hasStarted = true;
        earthquakeSfx.Play();
    }




    IEnumerator ShakingCoroutine()
    {
        Vector3 startPosition = transform.position;
        float elapsedTimeSinceShakeStart = 0f;

        while(elapsedTimeSinceShakeStart < duration)
        {
            elapsedTimeSinceShakeStart += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTimeSinceShakeStart / duration); //parameter goes from 0 to 1
            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }
        transform.position = startPosition; //set the object back at its original position
    }

}
