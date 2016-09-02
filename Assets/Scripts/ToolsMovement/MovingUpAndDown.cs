using UnityEngine;
using System.Collections;

public class MovingUpAndDown : MonoBehaviour {

	Vector3 origin;
	float distance = 8;
	float speed = 0.3f;

	void Start(){
		origin = transform.position;
	}

	void Update () {
		transform.position = origin + (transform.forward*(Mathf.PingPong(Time.time*distance*speed + (distance*speed/2), distance*2)-distance));
//		transform.position.y = origin.y + Mathf.Sin(Time.time * speed);
	}
}
