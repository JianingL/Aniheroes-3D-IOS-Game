using UnityEngine;
using System.Collections;

public class yRotateFast : MonoBehaviour {
	void Update () {
		transform.Rotate (new Vector3 (0, 90, 0) * Time.deltaTime);
	}

}
