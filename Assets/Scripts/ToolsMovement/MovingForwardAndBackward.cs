using UnityEngine;
using System.Collections;

public class MovingForwardAndBackward : MonoBehaviour {

	Vector3 origin;
	float distance = 6;
	float speed = 0.3f;

	void Start(){
		origin = transform.position;
	}

	void Update () {

//		print ("++++++ controller");
//		print (PlayerController.Score);
		if(PlayerController.Score >= 3){
			transform.position = origin + (transform.forward*(Mathf.PingPong(Time.time*distance*speed + (distance*speed/2), distance*2)-distance));
		}
	}
}
