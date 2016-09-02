using UnityEngine;
using System.Collections;
using CnControls;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class BallPlayerController : MonoBehaviour
{
	public float speed;
	public float jumpSpeed = 0.5f;
	public int boxScore = 0;

	public Text restartText;
	public Text gameOverText;
	private bool gameOver;
	private bool restart;
	public Transform restartButton;

	private static int giftScore = 0;
	private bool restartClicked;

	public static int Score { 
		get { return giftScore; }
		set { giftScore = value; }

	}

	private static bool rotateMove = false;

	public static bool RotateMove { 
		get { return rotateMove; }
		set { rotateMove = value; }

	}


	// create the variable to hold the reference.
	private Rigidbody rb;
	//	bool isInAir = false;

	bool IsInAir = false;
	CharacterController controller;

	void Start ()
	{
		gameOver = false;
		restart = false;
		restartText.text = "";
		gameOverText.text = "";

		rb = GetComponent<Rigidbody> ();
		IsInAir = false;
	}

	void FixedUpdate ()
	{
		// grabs the input from our player through the keyboard.
		float moveHorizontal = CnInputManager.GetAxis ("Horizontal");
		float moveVertical = CnInputManager.GetAxis ("Vertical");
		//		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		// add speed
		//moving player based on camera rather than world orientation 
		Vector3 newX = moveHorizontal * Camera.main.transform.right;
		Vector3 newZ = moveVertical * Camera.main.transform.forward;
		Vector3 movement = newX + newZ;
		
		rb.AddForce (movement * speed);


		controller = GetComponent<CharacterController> ();
		// Question???   && controller.isGrounded
		//		&& !IsInAir
		if (CnInputManager.GetButtonDown("Jump") && !IsInAir) {
//		if (Input.GetKey("r") && !IsInAir) {
			Vector3 movementV = new Vector3 (0.0f, 30.0f, 0.0f);
			rb.AddForce (movementV * jumpSpeed);
			IsInAir = true;
		}

// 		print  (controller.isGrounded);


		if (gameOver) {
			restartButton.gameObject.SetActive (true);
			restartText.text = "Restart";

			GameOver ();
			restart = true;
			PlayerController.RotateMove = false;
			PlayerController.Score = 0;
			giftScore = 0;

		}



	}

	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.CompareTag ("DeadSea")) {
			gameOver = true;
			//			other.gameObject.SetActive (false);
		}

		if (other.gameObject.CompareTag ("EndPoint")) {
			gameOver = true;
			other.gameObject.SetActive (false);
		}

		if (other.gameObject.CompareTag ("ground")) {
			IsInAir = false;
		}
		if (other.gameObject.CompareTag ("Poison")) {
//			print ("this is knot p");
			other.gameObject.SetActive (false);

//			GetComponent("Arrow").
//			GameObject arrow = GameObject.Find ("PoinsonArrow");
//			public GameObject[] arrows;
			GameObject arrow = GameObject.FindGameObjectWithTag("PoinsonArrow");
//			print ("hello");
//			print (arrow);
			arrow.SetActive(false);


			transform.localScale -= new Vector3 (0.8F, 0.8F, 0.8F);
		}

		if (other.gameObject.CompareTag ("Gift")) {
			other.gameObject.SetActive (false);
			PlayerController.Score += 1;
			//			print ("player controller");
			//			print (PlayerController.Score);

		}

		if (other.gameObject.CompareTag ("CaveGround")) {
			GameObject arrow = GameObject.FindGameObjectWithTag("CaveArrow");
			arrow.SetActive(false);

		}

		//GiftCube
		if (other.gameObject.CompareTag ("GiftCube")) {
			other.gameObject.SetActive (false);
			PlayerController.RotateMove = true;
		}

		if (other.gameObject.CompareTag ("test")) {
			other.gameObject.SetActive (false);
			transform.localScale += new Vector3 (0.8F, 0.8F, 0.8F);
		}

		//		if (other.gameObject.CompareTag("Diamond")){
		//			other.gameObject.SetActive (false);
		//
		////			Color newColor = new Color( Random.value, Random.value, Random.value, 1.0f );
		//
		//		}

		if (other.gameObject.CompareTag ("Icecream")) {
			other.gameObject.SetActive (false);
		}

		if (other.gameObject.CompareTag ("Arrow")) {
			other.gameObject.SetActive (false);
		}

		if (other.gameObject.CompareTag ("Star")) {
			other.gameObject.SetActive (false);
		}

//		if (other.gameObject.CompareTag ("Big")) {
//			other.gameObject.SetActive (false);
//			transform.localScale += new Vector3 (4F, 4F, 4F);
//		}

	}

	// game over and restart
	public void GameOver ()
	{
		gameOverText.text = "Game Over";
		gameOver = true;
	}

	public void Restart ()
	{

		gameOverText.text = "";
		restartButton.gameObject.SetActive (false);
		//Application.LoadLevel ("PartOne");
		SceneManager.LoadScene ("PartOne");
	}
		
}