using UnityEngine;
using System.Collections;

public class CardRotator : MonoBehaviour {

	void Update () {
		transform.Rotate (new Vector3 (0, 55, 0) * Time.deltaTime * 13);
	}
}
