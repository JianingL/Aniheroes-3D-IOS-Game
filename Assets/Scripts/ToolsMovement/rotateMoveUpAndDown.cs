using UnityEngine;
using System.Collections;

public class rotateMoveUpAndDown : MonoBehaviour {

	float maxUpAndDown = 18;
	float speed = 25;            
	float angle = Mathf.PI; 
	float toDegrees = Mathf.PI / 170; 

	Vector3 origin;


//	public GameObject player;
//	private PlayerController playerScript;

	void Start(){
//		playerScript = (PlayerController)player.GetComponent(typeof(PlayerController)); 

		origin = transform.position;
	}

	void Update () {
		if (PlayerController.RotateMove == true) {

			angle += speed * Time.deltaTime;
			//		if (angle > 360) angle -= 360;
			origin.y = maxUpAndDown * Mathf.Sin (angle * toDegrees) - 48;
			transform.position = origin;
		}
	}
}
