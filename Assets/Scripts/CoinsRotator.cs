using UnityEngine;
using System.Collections;

public class CoinsRotator : MonoBehaviour {

	
	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3 (0, 15, 0) * Time.deltaTime * 10);
	}
}
