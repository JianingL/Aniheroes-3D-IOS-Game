using UnityEngine;
using System.Collections;
using CnControls;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/* kitten */
// Require a character controller to be attached to the same game object
[RequireComponent (typeof(CharacterController))]

public class KittenController : MonoBehaviour
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

	public Text scoreText;
	public Text highScoreText;
	private int count;

	/* level card */
	private int cardNumber = 1;

	/* kitten */
	public AnimationClip idleAnimation;
	public AnimationClip walkAnimation;
	public AnimationClip runAnimation;
	public AnimationClip jumpPoseAnimation;

	public float walkMaxAnimationSpeed = 0.75f;
	public float trotMaxAnimationSpeed = 1.0f;
	public float runMaxAnimationSpeed = 1.0f;
	public float jumpAnimationSpeed = 1.15f;
	public float landAnimationSpeed = 1.0f;

	private Animation _animation;

	enum CharacterState
	{
		Idle = 0,
		Walking = 1,
		Trotting = 2,
		Running = 3,
		Jumping = 4,
	}

	private CharacterState _characterState;

	// The speed when walking
	public float walkSpeed = 4.0f;
	// after runAfterSeconds of walking we run with trotSpeed
	public float runSpeed = 8.0f;

	public float inAirControlAcceleration = 3.0f;

	// How high do we jump when pressing jump and letting go immediately
	public float jumpHeight = 0.5f;

	// The gravity for the character
	public float gravity = 20.0f;
	// The gravity in controlled descent mode
	public float speedSmoothing = 10.0f;
	public float rotateSpeed = 500.0f;
	public float runAfterSeconds = 0.7f;

	public bool canJump = true;

	private float jumpRepeatTime = 0.05f;
	private float jumpTimeout = 0.15f;
	private float groundedTimeout = 0.25f;

	// The camera doesnt start following the target immediately but waits for a split second to avoid too much waving around.
	private float lockCameraTimer = 0.0f;

	// The current move direction in x-z
	private Vector3 moveDirection = Vector3.zero;
	// The current vertical speed
	private float verticalSpeed = 0.0f;
	// The current x-z move speed
	private float moveSpeed = 0.0f;

	// The last collision flags returned from controller.Move
	private CollisionFlags collisionFlags;

	// Are we jumping? (Initiated with jump button and not grounded yet)
	private bool jumping = false;
	private bool jumpingReachedApex = false;

	// Are we moving backwards (This locks the camera to not do a 180 degree spin)
	private bool movingBack = false;
	// Is the user pressing any keys?
	private bool isMoving = false;
	// When did the user start walking (Used for going into trot after a while)
	private float walkTimeStart = 0.0f;
	// Last time the jump button was clicked down
	private float lastJumpButtonTime = -10.0f;
	// Last time we performed a jump
	private float lastJumpTime = -1.0f;


	// the height we jumped from (Used to determine for how long to apply extra jump power after jumping.)
	private float lastJumpStartHeight = 0.0f;


	private Vector3 inAirVelocity = Vector3.zero;

	private float lastGroundedTime = 0.0f;


	private bool isControllable = true;
	/* kitten end */

	//for prize Hint
	private bool prizeHintEnable=true;
	public GameObject prizeCanvas;

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

	public GameObject mission;
	private bool missionEnable=true;

	bool IsInAir = false;
	CharacterController controller;

	private AudioSource audio;

	void Start ()
	{
		this.audio = GetComponent<AudioSource>();
		gameOver = false;
		restart = false;
		restartText.text = "";
		gameOverText.text = "";
		IsInAir = false;
		GameOverManager.missionComplete = false;
		restartButton.gameObject.SetActive (false);

		prizeCanvas.SetActive (false);
	}

	void Update ()
	{

//		print (controller.isGrounded);
		if (missionEnable) {
			mission.SetActive (true);
			Time.timeScale = 0;
			missionEnable = false;
		}

		if (!missionEnable) {
			if (Input.GetMouseButtonDown(0)) {
				mission.SetActive (false);
				Time.timeScale = 1;
			}
		}

		if (prizeCanvas.activeSelf) {
			if (Input.GetMouseButtonDown(0)) {
				prizeCanvas.SetActive (false);
				Time.timeScale = 1;
			}
		}

		if (gameOver) {
			//System.Threading.Thread.Sleep (1000);
			restartButton.gameObject.SetActive (true);
			restartText.text = "Restart";

			GameOver ();
			restart = true;
		}



	}

	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.CompareTag ("PrizeCup")) {
			GameObject cup = GameObject.FindGameObjectWithTag ("PrizeCup");
			cup.SetActive (false);
			GameOver ();
		}
		
		if (other.gameObject.CompareTag ("prizeHint")) {
			print("collspe with prize fence");
			if (prizeHintEnable) {
				prizeCanvas.SetActive (true);
				Time.timeScale = 0;
				prizeHintEnable = false;
			}
		}
		
		if (other.gameObject.CompareTag ("DeadSea")) {
			gameOver = true;
			//			other.gameObject.SetActive (false);
		}

		if (other.gameObject.CompareTag ("EndPoint")) {
			gameOver = true;
			//			other.gameObject.SetActive (false);
		}

		if (other.gameObject.CompareTag ("ground")) {
			IsInAir = false;
		}
		if (other.gameObject.CompareTag ("Poison")) {
			print ("this is knot p");
			other.gameObject.SetActive (false);

			//			Color newColor = new Color( Random.value, Random.value, Random.value, 1.0f );
			//			// apply it on current object's material
			//			GetComponent<Renderer>().material.color = Color.cyan;

			transform.localScale -= new Vector3 (4F, 4F, 4F);
		}

		if (other.gameObject.CompareTag ("Gift")) {
			other.gameObject.SetActive (false);
			PlayerController.Score += 1;
			//			print ("player controller");
			//			print (PlayerController.Score);
			GameObject.FindGameObjectWithTag(string.Concat("LevelCard", this.cardNumber)).GetComponent<Image>().color = Color.green;
			this.cardNumber = this.cardNumber + 1;
		}

		//GiftCube
		if (other.gameObject.CompareTag ("GiftCube")) {
			other.gameObject.SetActive (false);
			PlayerController.RotateMove = true;
		}

		if (other.gameObject.CompareTag ("test")) {
			other.gameObject.SetActive (false);
			transform.localScale += new Vector3 (4F, 4F, 4F);
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

//		if (other.gameObject.CompareTag ("Coins")) {
////			other.gameObject.SetActive (false);
//			coin_score_animation_level1 test = other.gameObject.GetComponent<coin_score_animation_level1>();
//			test.shrinking = true;
//
//			//			other.gameObject.SetActive (false);
//			count = count + 1;
//			scoreText.text = "Score: " + (count * 10).ToString ();
//		}

		if (other.gameObject.CompareTag ("CoinsLevel2")) {
			//			other.gameObject.SetActive (false);
			coin_score_animation_level2 test = other.gameObject.GetComponent<coin_score_animation_level2>();
			test.shrinking = true;

			//			other.gameObject.SetActive (false);
			count = count + 1;
			scoreText.text = "Score: " + (count * 10).ToString ();
			this.audio.Play ();
		}

		if (other.gameObject.CompareTag ("Arrow")) {
			other.gameObject.SetActive (false);
		}

		if (other.gameObject.CompareTag ("Star")) {
			other.gameObject.SetActive (false);
		}

	}

	// game over and restart
	public void GameOver ()
	{
		gameOverText.text = "Game Over";
		gameOver = true;

		// check whether the player complete the mission
		if (count * 10 >= 1000 && cardNumber >= 4) {
			gameOverText.text = "Mission Complete!" + cardNumber;
//			anim.SetTrigger ("GameOverAnimation");
			GameOverManager.missionComplete = true;
			// update the high score in PlayerPref and update the high score text in game
			PlayerPrefs.SetInt("Level2High", count * 10);
			this.highScoreText.text = "High: " + PlayerPrefs.GetInt("Level2High");
		} else {
			gameOverText.text = "Mission Failed!" + cardNumber;
			GameOverManager.missionComplete = false;
//			anim.SetTrigger ("GameOverAnimation");
		}
	}

	public void Restart ()
	{

		gameOverText.text = "";
//		restartButton.gameObject.SetActive (false);
//		GameOverManager.missionComplete = false;
		//Application.LoadLevel ("PartOne");
		SceneManager.LoadScene("PartTwo");
	}


	/* kitten begin */
	public void  Awake ()
	{
		// load high score from PlayerPrefs
		this.highScoreText.text = "High: " + PlayerPrefs.GetInt("Level2High");

		// TODO: load music options and control music
		if (PlayerPrefs.GetInt("music") == 1) {
			// TODO
		}

		moveDirection = transform.TransformDirection (Vector3.forward);

		_animation = GetComponent<Animation> ();
		if (!_animation)
			Debug.Log ("The character you would like to control doesn't have animations. Moving her might look weird.");

		/*
            public AnimationClip idleAnimation;
            public AnimationClip walkAnimation;
            public AnimationClip runAnimation;
            public AnimationClip jumpPoseAnimation;	
        */
		if (!idleAnimation) {
			_animation = null;
			Debug.Log ("No idle animation found. Turning off animations.");
		}
		if (!walkAnimation) {
			_animation = null;
			Debug.Log ("No walk animation found. Turning off animations.");
		}
		if (!runAnimation) {
			_animation = null;
			Debug.Log ("No run animation found. Turning off animations.");
		}
		if (!jumpPoseAnimation && canJump) {
			_animation = null;
			Debug.Log ("No jump animation found and the character has canJump enabled. Turning off animations.");
		}

	}

	public void  UpdateSmoothedMovementDirection ()
	{
		Transform cameraTransform = Camera.main.transform;
		bool grounded = IsGrounded ();

		// Forward vector relative to the camera along the x-z plane	
		Vector3 forward = cameraTransform.TransformDirection (Vector3.forward);
		forward.y = 0;
		forward = forward.normalized;

		// Right vector relative to the camera
		// Always orthogonal to the forward vector
		Vector3 right = new Vector3 (forward.z, 0.0f, -forward.x);

		//        float v= Input.GetAxisRaw("Vertical");
		//		float moveHorizontal = CnInputManager.GetAxis ("Horizontal");
		//		float moveVertical = CnInputManager.GetAxis ("Vertical");
		float v = CnInputManager.GetAxis ("Vertical");
		float h = CnInputManager.GetAxis ("Horizontal");
		//        float h= Input.GetAxisRaw("Horizontal");

		// Are we moving backwards or looking backwards
		if (v < -0.2f)
			movingBack = true;
		else
			movingBack = false;

		bool wasMoving = isMoving;
		isMoving = Mathf.Abs (h) > 0.1f || Mathf.Abs (v) > 0.1f;

		// Target direction relative to the camera
		Vector3 targetDirection = h * right + v * forward;

		// Grounded controls
		if (grounded) {
			// Lock camera for short period when transitioning moving & standing still
			lockCameraTimer += Time.deltaTime;
			if (isMoving != wasMoving)
				lockCameraTimer = 0.0f;

			// We store speed and direction seperately,
			// so that when the character stands still we still have a valid forward direction
			// moveDirection is always normalized, and we only update it if there is user input.
			if (targetDirection != Vector3.zero) {
				// If we are really slow, just snap to the target direction
				if (moveSpeed < walkSpeed * 0.9f && grounded) {
					moveDirection = targetDirection.normalized;
				}
				// Otherwise smoothly turn towards it
				else {
					moveDirection = Vector3.RotateTowards (moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);

					moveDirection = moveDirection.normalized;
				}
			}

			// Smooth the speed based on the current target direction
			float curSmooth = speedSmoothing * Time.deltaTime;

			// Choose target speed
			//* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
			float targetSpeed = Mathf.Min (targetDirection.magnitude, 1.0f);

			_characterState = CharacterState.Idle;

			// Pick speed modifier
			// Pick speed modifier
			if (Time.time - runAfterSeconds > walkTimeStart)
			{
				targetSpeed *= runSpeed;
				_characterState = CharacterState.Running;
			}
			else
			{
				targetSpeed *= walkSpeed;
				_characterState = CharacterState.Walking;
			}
				
			moveSpeed = Mathf.Lerp (moveSpeed, targetSpeed, curSmooth);

			// Reset walk time start when we slow down
			if (moveSpeed < walkSpeed * 0.6f)
				walkTimeStart = Time.time;
		}
		// In air controls
		else {
			// Lock camera while in air
			if (jumping)
				lockCameraTimer = 0.0f;

			if (isMoving)
				inAirVelocity += targetDirection.normalized * Time.deltaTime * inAirControlAcceleration;
		}
	}

	public void  ApplyJumping ()
	{
		// Prevent jumping too fast after each other
		if (lastJumpTime + jumpRepeatTime > Time.time)
			return;

		if (IsGrounded ()) {
			// Jump
			// - Only when pressing the button down
			// - With a timeout so you can press the button slightly before landing		
			if (canJump && Time.time < lastJumpButtonTime + jumpTimeout) {
				verticalSpeed = CalculateJumpVerticalSpeed (jumpHeight);
				SendMessage ("DidJump", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void  ApplyGravity ()
	{
		if (isControllable) {	// don't move player at all if not controllable.
			// Apply gravity
			bool jumpButton = Input.GetButton ("Jump");

			// When we reach the apex of the jump we send out a message
			if (jumping && !jumpingReachedApex && verticalSpeed <= 0.0f) {
				jumpingReachedApex = true;
				SendMessage ("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);
			}

			if (IsGrounded ())
				verticalSpeed = 0.0f;
			else
				verticalSpeed -= gravity * Time.deltaTime;
		}
	}

	public float  CalculateJumpVerticalSpeed (float targetJumpHeight)
	{
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt (2 * targetJumpHeight * gravity);
	}

	public void  DidJump ()
	{
		jumping = true;
		jumpingReachedApex = false;
		lastJumpTime = Time.time;
		lastJumpStartHeight = transform.position.y;
		lastJumpButtonTime = -10;

		_characterState = CharacterState.Jumping;
	}

	public void  FixedUpdate ()
	{

		if (cardNumber >= 4) {
			GameObject[] fence = GameObject.FindGameObjectsWithTag ("wayToPrize");
			for (int i = 0; i < fence.Length; i++) {
				fence [i].SetActive (false);
			}
		}

		if (!isControllable) {
			// kill all inputs if not controllable.
			Input.ResetInputAxes ();
		}


		//		if (Input.GetButtonDown ("Jump")) {
		if (CnInputManager.GetButtonDown("Jump")) {
			lastJumpButtonTime = Time.time;
		}

		UpdateSmoothedMovementDirection ();

		// Apply gravity
		// - extra power jump modifies gravity
		// - controlledDescent mode modifies gravity
		ApplyGravity ();

		// Apply jumping logic
		ApplyJumping ();

		// Calculate actual motion
		Vector3 movement = moveDirection * moveSpeed + new Vector3 (0.0f, verticalSpeed, 0.0f) + inAirVelocity;
		movement *= Time.deltaTime;

		// Move the controller
		CharacterController controller = GetComponent<CharacterController> ();
		collisionFlags = controller.Move (movement);

		// ANIMATION sector
		if (_animation) {
			if (_characterState == CharacterState.Jumping) {
				if (!jumpingReachedApex) {
					_animation [jumpPoseAnimation.name].speed = jumpAnimationSpeed;
					_animation [jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
					_animation.CrossFade (jumpPoseAnimation.name);
				} else {
					_animation [jumpPoseAnimation.name].speed = -landAnimationSpeed;
					_animation [jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
					_animation.CrossFade (jumpPoseAnimation.name);				
				}
			} else {
				if (controller.velocity.sqrMagnitude < 0.1f) {
					_animation.CrossFade (idleAnimation.name);
				} else {
					if (_characterState == CharacterState.Running) {
						_animation [runAnimation.name].speed = Mathf.Clamp (controller.velocity.magnitude, 0.0f, runMaxAnimationSpeed);
						_animation.CrossFade (runAnimation.name);	
					} else if (_characterState == CharacterState.Trotting) {
						_animation [walkAnimation.name].speed = Mathf.Clamp (controller.velocity.magnitude, 0.0f, trotMaxAnimationSpeed);
						_animation.CrossFade (walkAnimation.name);	
					} else if (_characterState == CharacterState.Walking) {
						_animation [walkAnimation.name].speed = Mathf.Clamp (controller.velocity.magnitude, 0.0f, walkMaxAnimationSpeed);
						_animation.CrossFade (walkAnimation.name);	
					}

				}
			}
		}

//		if (this.didReadWelcomeText == false && Input.GetMouseButtonDown(0)){
//			this.didReadWelcomeText = true;
//			this.welcomeText.SetActive (false);
//		}
//		if (this.didReadWelcomeText == false) {
//			return;
//		}
		// ANIMATION sector

		// Set rotation to the move direction
		if (IsGrounded ()) {
			transform.rotation = Quaternion.LookRotation (moveDirection);
		} else {
			Vector3 xzMove = movement;
			xzMove.y = 0;
			if (xzMove.sqrMagnitude > 0.001f) {
				transform.rotation = Quaternion.LookRotation (xzMove);
			}
		}	

		// We are in jump mode but just became grounded
		if (IsGrounded ()) {
			lastGroundedTime = Time.time;
			inAirVelocity = Vector3.zero;
			if (jumping) {
				jumping = false;
				SendMessage ("DidLand", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void  OnControllerColliderHit (ControllerColliderHit hit)
	{
		//	Debug.DrawRay(hit.point, hit.normal);
		if (hit.moveDirection.y > 0.01f)
			return;
	}

	public float GetSpeed ()
	{
		return moveSpeed;
	}

	public bool  IsJumping ()
	{
		return jumping;
	}

	public bool  IsGrounded ()
	{
		return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
	}

	public Vector3 GetDirection ()
	{
		return moveDirection;
	}

	public bool IsMovingBackwards ()
	{
		return movingBack;
	}

	public float GetLockCameraTimer ()
	{
		return lockCameraTimer;
	}

	public bool IsMoving ()
	{
		return Mathf.Abs (Input.GetAxisRaw ("Vertical")) + Mathf.Abs (Input.GetAxisRaw ("Horizontal")) > 0.5f;
	}

	public bool HasJumpReachedApex ()
	{
		return jumpingReachedApex;
	}

	public bool IsGroundedWithTimeout ()
	{
		return lastGroundedTime + groundedTimeout > Time.time;
	}

	public void  Reset ()
	{
		gameObject.tag = "Player";
	}
}