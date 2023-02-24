using UnityEngine;
using System.Collections;
using Cinemachine;

public class Parallaxx : MonoBehaviour
{
	public float distanceX = 0f; // Distance of the item (z-index based) 
	public float distanceY = 0f;

	private Vector3 startPos;

	private bool canMove = false;

	public Transform cameraFollow;


	void Awake()
	{
		startPos = transform.position;
		cameraFollow = FindObjectOfType<CameraHorizaonalVerticalClamp>().transform;
	}

	void LateUpdate()
	{

		var offset = cameraFollow.transform.position;
		offset.x *= 1f - distanceX;
		offset.y *= 1f - distanceY;
		transform.position = startPos + offset;

		
	}



	IEnumerator WaitToPlaceCoroutine()
	{
		yield return new WaitForSeconds(0.5f);
		canMove = true;
	}


}