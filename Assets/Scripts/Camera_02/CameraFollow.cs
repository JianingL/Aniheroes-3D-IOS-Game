using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public GameObject player;
	public float smoothing = 5f;

	Vector3 offset;

	void Start(){
		offset = transform.position - player.transform.position;
	}

	void FixedUpdated(){
		Vector3 targetCamPos = player.transform.position + offset;
		transform.position = Vector3.Lerp (transform.position,targetCamPos,smoothing*Time.deltaTime);
//		transform.position = player.transform.position + offset;
	}
}
