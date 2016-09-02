using UnityEngine;
using System.Collections;
using CnControls;

public class CameraController : MonoBehaviour
{
	public GameObject player;
	public Vector3 offset;

	//	private Vector3 offset;

	// Use this for initialization
	void Start ()
	{
		//		offset = transform.position - player.transform.position;
		transform.position = player.transform.position + offset;
	}

	// Update is called once per frame
	void LateUpdate ()
	{
		//check if need to rotate camera
		transform.position = player.transform.position + offset;
		

		if (CnInputManager.GetButton ("RightRotate")) {
			transform.RotateAround (player.transform.position, Vector3.up, 90 * Time.deltaTime);
			offset = transform.position - player.transform.position;

		} else if (CnInputManager.GetButton ("LeftRotate")) {
			transform.RotateAround (player.transform.position, Vector3.up * (-1), 90 * Time.deltaTime);
			offset = transform.position - player.transform.position;
		} else {
			transform.position = player.transform.position + offset;
			//			offset=transform.position - player.transform.position;
		}
	}




}