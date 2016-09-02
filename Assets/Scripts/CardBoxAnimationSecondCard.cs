using UnityEngine;
using System.Collections;

public class CardBoxAnimationSecondCard : MonoBehaviour {

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
		this.card = GameObject.FindGameObjectWithTag("CardLevel102");
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
//			transform.localScale = new Vector3 (1.22F, 1.22F, 1.22F);
			transform.localScale = new Vector3 (transform.localScale.x / 10, transform.localScale.y / 10, transform.localScale.z / 10);
			if (transform.localScale.x < targetScale) {

				shrinking = false;

				gotPoint = true;
				transform.position = new Vector3(transform.position.x, transform.position.y + 30, transform.position.z);

			}

		}
		//		if (shrinking) {
		//			transform.localScale = new Vector3 (transform.localScale.x / 100, transform.localScale.y / 100, transform.localScale.z / 100);
		//			shrinking = false;
		//			transform.position = new Vector3(transform.position.x, transform.position.y + 30, transform.position.z);
		//			//				//                player.SetActive (false);
		//			gotPoint = true;
		//
		//		}
		if (shrinking == false && gotPoint == true && scoreAppeared == false && this.increaseHeight < 12) {

			//			this.point.AddComponent<MeshRenderer>(); // Throws Error

			//			TextMesh tm = this.point.AddComponent<TextMesh>(); // This too throws Error

			this.increaseHeight += 0.07F;
			//
			//			this.pointPos.y += 0.07F;
			//			this.point.transform.position = this.pointPos;
			//
			//			tm.text = "+" + this.scoreValue;
			//			tm.font = font;
			//			tm.characterSize = 1F;
			//			tm.color = Color.green;


			this.card.SetActive (true);
		}

		if (this.increaseHeight >= 12 && this.destroyed == false) {
			this.destroyed = true;
			Destroy (this.card);
			transform.gameObject.SetActive (false);
		}

	}
}
