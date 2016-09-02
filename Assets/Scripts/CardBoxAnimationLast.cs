using UnityEngine;
using System.Collections;

public class CardBoxAnimationLast : MonoBehaviour {

	public int scoreValue;
	public Material material; // None in Inspector
	public Font font; // None in Inspector
	//	private GameObject point;
	private Vector3 pointPos;

	private bool destroyed;

	public float targetScale = 0.0001f;
	public float shrinkSpeed = 3.1f;
	public bool shrinking;
	private CharacterController controller;
	private bool gotPoint;
	private bool scoreAppeared;
	GameObject player;
	private double increaseHeight;

	private GameObject card;

	void Start () {

		this.destroyed = false;
		this.increaseHeight = 0;
		//		this.point = new GameObject ();
		//		this.card = new GameObject();
		this.card = GameObject.FindGameObjectWithTag("CardLevel103");
		this.card.SetActive (false);
		this.pointPos = new Vector3 (transform.position.x, transform.position.y, transform.position.z);

		shrinking = false;
		gotPoint = false;
		scoreAppeared = false;
		player = transform.root.gameObject;

	}

	// Update is called once per frame
	void Update () {
		if (shrinking) {
			transform.localScale -= new Vector3 (0.02F, 0.02F, 0.02F);
			//						transform.localScale -= new Vector3 (transform.localScale.x / 1.5, transform.localScale.y / 1.5, transform.localScale.z / 1.5);
			if (transform.localScale.x < targetScale) {

				shrinking = false;

				gotPoint = true;
				transform.position = new Vector3(transform.position.x, transform.position.y + 30, transform.position.z);

			}

		}

		if (shrinking == false && gotPoint == true && scoreAppeared == false && this.increaseHeight < 12) {

			this.increaseHeight += 0.07F;

			this.card.SetActive (true);
		}

		if (this.increaseHeight >= 12 && this.destroyed == false) {
			this.destroyed = true;
			Destroy (this.card);
			transform.gameObject.SetActive (false);
		}

	}
}
