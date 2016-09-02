using UnityEngine;
using System.Collections;

public class MovingForwardAndBackwardX : MonoBehaviour {

	Vector3 origin;
	float distance = 4.5f;
	float speed = 0.3f;

	void Start(){
		origin = transform.position;
	}

	void Update () {
		if(PlayerController.RotateMove == true){
			transform.position = origin + (transform.forward*(Mathf.PingPong(Time.time*distance*speed + (distance*speed/2), distance*2)-distance));
		}
	}

}
