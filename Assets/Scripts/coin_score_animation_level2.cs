using UnityEngine;
using System.Collections;

public class coin_score_animation_level2 : MonoBehaviour {

	public int scoreValue;
	public Material material; // None in Inspector
	public Font font; // None in Inspector
	private GameObject point;
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
	//    public Text bonus;
	//    public GUIText bonusScore;
	//    public bool showScore;
	// Use this for initialization
	void Start () {
		this.destroyed = false;
		this.increaseHeight = 0;
		this.point = new GameObject ();
		this.pointPos = new Vector3 (transform.position.x, transform.position.y, transform.position.z);

		shrinking = false;
		gotPoint = false;
		scoreAppeared = false;
		player = transform.root.gameObject;

	}

	// Update is called once per frame
	void Update () {


		//		if (shrinking) {
		//			transform.localScale = new Vector3 (transform.localScale.x / 100, transform.localScale.y / 100, transform.localScale.z / 100);
		//			if (transform.localScale.x < targetScale) {
		//
		//				shrinking = false;
		//				//                player.SetActive (false);
		//				gotPoint = true;
		//
		//
		//			}
		//
		//		}
		if (shrinking) {
			//			transform.localScale = new Vector3 (transform.localScale.x / 100, transform.localScale.y / 100, transform.localScale.z / 100);
			shrinking = false;
			transform.position = new Vector3(transform.position.x, transform.position.y + 30, transform.position.z);
			//				//                player.SetActive (false);
			gotPoint = true;

		}
		if (shrinking == false && gotPoint == true && scoreAppeared == false && this.increaseHeight < 3) {

			this.point.AddComponent<MeshRenderer>(); // Throws Error

			TextMesh tm = this.point.AddComponent<TextMesh>(); // This too throws Error

			this.increaseHeight += 0.07F;
			//			this.pointPos = this.pointPos + new Vector3 (0.1F, 0.1F, 0.1F);
			this.pointPos.y += 0.07F;
			this.point.transform.position = this.pointPos;

			//			this.point.transform.rotation.x = ;
			//			this.point.transform.rotation.y = ;
			//			this.point.transform.localScale += new Vector3 (0.1F, 0.1F, 0.1F);
			//			print (tm.transform.position.y);

			tm.text = "+" + this.scoreValue;
			tm.font = font;
			tm.characterSize = 0.5F;
			tm.color = Color.green;

			var CharacterRotation = Camera.main.transform.rotation;
			CharacterRotation.x = 0;
			CharacterRotation.z = 0;

			tm.transform.rotation = CharacterRotation;


		}

		if (this.increaseHeight >= 3 && this.destroyed == false) {
			this.destroyed = true;
			Destroy (this.point);
			transform.gameObject.SetActive (false);
		}

	}
}
