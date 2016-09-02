using UnityEngine;
using System.Collections;
using CnControls;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/* kitten */
// Require a character controller to be attached to the same game object
[RequireComponent (typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
	
	public float speed;
	public float jumpSpeed = 0.5f;
	public int boxScore = 0;
	private int count;

	public Text restartText;
	public Text backtomainText;
	public Text gameOverText;
	public Text resumeText;
	public Text scoreText;
	public Text HighScoreText; // highest score history
	//public Text finalScoreText;

	private bool gameOver;
	private bool gameComplete = false;
	private bool restart;
	public Transform restartButton;
	public Transform resumeButton;
//	public Transform backtomainButton;
	public Transform nextLevelButton;

	public GameObject checkpoint1_object;
	public GameObject checkpoint2_object;

	private List<Vector3> checkpoints;


	private static int giftScore = 0;
	private bool restartClicked;
	private bool stop = false;
	private bool becomeSmallerTriggerEnable = true;

//	for roomcamera
	public GameObject wall_y_min;
	public GameObject wall_y_max;
	public GameObject wall_z_min;
	public GameObject wall_z_max;
	public GameObject wall_x_min;
	public GameObject wall_x_max;
	public Camera roomcarmera;
	public Camera maincamera;

	/* kitten */
	public AnimationClip idleAnimation;
	public AnimationClip walkAnimation;
	public AnimationClip runAnimation;
	public AnimationClip jumpPoseAnimation;

	public Animator anim;

	public float walkMaxAnimationSpeed = 0.75f;
	public float trotMaxAnimationSpeed = 1.0f;
	public float runMaxAnimationSpeed = 1.0f;
	public float jumpAnimationSpeed = 1.15f;
	public float landAnimationSpeed = 1.0f;

	public GameObject triggerTest;
	public GameObject missionCanvas;
	public GameObject hintCanvas;
	public GameObject welcomeText;
	public GameObject fader;

	private bool didReadWelcomeText;
//	private List<Vector3> hintTag;
	Dictionary<GameObject, bool> hintTag;
	private GameObject currentHintObject;

	// for tutorial
	private bool jumpTutorialEnable=true;
	private bool rotateTutorialEnable=true;
	private bool jumpButtonChangeColor=false;
	private bool rotateCameraButtonChangeColor=false;
	public GameObject jumpButton;
	public GameObject rotateCameraButton;
	private bool tutorialColorChanged = false;
	private int updateCount=0;

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
	public float walkSpeed = 8.0f;
	// after runAfterSeconds of walking we trot with trotSpeed
	public float runSpeed = 13.0f;

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


	//for checkpoint
	public GameObject roomgift;
	public GameObject rotateplatform;
	private Vector3 rotateplatforminitialposition;

	public static int Score { 
		get { return giftScore; }
		set { giftScore = value; }

	}

	private static bool rotateMove = false;

	public static bool RotateMove { 
		get { return rotateMove; }
		set { rotateMove = value; }

	}

	/* level card */
	private int cardNumber = 1;


	// create the variable to hold the reference.
	private Rigidbody rb;
	//	bool isInAir = false;

	bool IsInAir = false;
	CharacterController controller;

	private AudioSource audio;
//	audio.Play();


	void Start ()
	{
		this.audio = GetComponent<AudioSource>();
//		audio.Play();
		this.hintTag = new Dictionary<GameObject, bool> ();
		this.hintTag.Add (GameObject.FindGameObjectWithTag("becomeSmallerTrigger"), false);

		checkpoints = new List<Vector3> ();
		Vector3 startpoint = new Vector3 (0, 1, 0);
		checkpoints.Add (startpoint);

		//for checkpoint
		rotateplatforminitialposition = rotateplatform.transform.position;

		fader.gameObject.SetActive (false);
		gameOver = false;
		restart = false;
		restartText.text = "";
		backtomainText.text = "";
		gameOverText.text = "";
		resumeText.text="";
		//finalScoreText.text = "";
		count = 0;
		scoreText.text = "Score: " + count.ToString();

//		rb = GetComponent<Rigidbody> ();
		IsInAir = false;

		this.triggerTest = GameObject.FindGameObjectWithTag("TestTrigger");
		this.hintCanvas = GameObject.FindGameObjectWithTag ("HintCanvas");
//		this.triggerTest.SetActive(false);
		this.hintCanvas.SetActive (false);
		this.welcomeText = GameObject.FindGameObjectWithTag("welcomeText");
		this.didReadWelcomeText = false;
		this.missionCanvas = GameObject.FindGameObjectWithTag ("MissionCanvas");
//		this.welcomeText.SetActive(false);

	}

	void Update ()
	{
		
		updateCount += 1;
		if (updateCount > 10) {
			updateCount = 0;
		}

		if (jumpButtonChangeColor) {
			Image jumpButtonImage = jumpButton.GetComponent<Image> ();
			if (updateCount == 10) {
				jumpButtonImage.color = jumpButtonImage.color == Color.blue ? Color.white : Color.blue;
//				updateCount = 0;
			}


			if (CnInputManager.GetButton ("Jump")) {
				Time.timeScale = 1;
				jumpButtonChangeColor = false;
				jumpButtonImage.color = Color.white;
			}
		}

		if (rotateCameraButtonChangeColor) {
			Image rotateCameraImage = rotateCameraButton.GetComponent<Image> ();
			if (updateCount == 10) {
				rotateCameraImage.color = rotateCameraImage.color == Color.blue ? Color.white : Color.blue;
				updateCount = 0;
			}

			if (CnInputManager.GetButton ("RightRotate")) {
				Time.timeScale = 1;
				rotateCameraButtonChangeColor = false;
				rotateCameraImage.color = Color.white;
			}

		}



		if (IsInGitfRoom ()) {
			roomcarmera.enabled = true;
			maincamera.enabled = false;
		} else {
			roomcarmera.enabled = false;
			maincamera.enabled = true;
		}

		controller = GetComponent<CharacterController> ();


		if (gameOver) {
			// Ying

			restartButton.gameObject.SetActive (true);
			resumeButton.gameObject.SetActive (true);
//			backtomainButton.gameObject.SetActive (true);

			restartText.text = "Restart";
			resumeText.text = "Resume";
//			backtomainText.text = "Back Home";
			//scoreText.text = count.ToString ();



			GameOver ();
			restart = true;

			PlayerController.RotateMove = false;
			PlayerController.Score = 0;
			giftScore = 0;
		}



	}

	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.CompareTag ("FirstMovingIsland")) {
			//			GameObject player = GameObject.FindGameObjectWithTag ("Player");
			GameObject otherObject = GameObject.FindGameObjectWithTag ("FirstMovingId");

			this.transform.parent = otherObject.gameObject.transform;
		}

		if (other.gameObject.CompareTag ("SecondMovingIslandTriger")) {
			//			GameObject player = GameObject.FindGameObjectWithTag ("Player");
			GameObject otherObject = GameObject.FindGameObjectWithTag ("SecondMovingIsland");

			this.transform.parent = otherObject.gameObject.transform;
		}
			
		if (other.gameObject.CompareTag ("ThirdMovingIslandTriger")) {
			//			GameObject player = GameObject.FindGameObjectWithTag ("Player");
			GameObject otherObject = GameObject.FindGameObjectWithTag ("ThirdMovingIsland");
			this.transform.parent = otherObject.gameObject.transform;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag ("FirstMovingIsland")) {
			//			GameObject player = GameObject.FindGameObjectWithTag ("Player");
//			GameObject otherObject = GameObject.FindGameObjectWithTag ("FirstMovingId");

			this.transform.parent = null;
		}

		if (other.gameObject.CompareTag ("SecondMovingIslandTriger")) {
			//			GameObject player = GameObject.FindGameObjectWithTag ("Player");
//			GameObject otherObject = GameObject.FindGameObjectWithTag ("SecondMovingIsland");

			this.transform.parent = null;
		}
			

		if (other.gameObject.CompareTag ("ThirdMovingIslandTriger")) {
			//			GameObject player = GameObject.FindGameObjectWithTag ("Player");
//			GameObject otherObject = GameObject.FindGameObjectWithTag ("ThirdMovingIsland");
			this.transform.parent = null;
		}
		
	}


	void OnTriggerEnter (Collider other)
	{

		if (other.gameObject.CompareTag ("CardV2")) {
			other.gameObject.SetActive (false);
			GameObject.FindGameObjectWithTag(string.Concat("LevelCard", this.cardNumber)).GetComponent<Image>().color = Color.green;
			this.cardNumber = this.cardNumber + 1;
		}

		if (other.gameObject.CompareTag ("jumptutorial")) {
			if (jumpTutorialEnable) {
				jumpButtonChangeColor = true;
				jumpTutorialEnable = false;
				Time.timeScale = 0;
			}
		}

		if (other.gameObject.CompareTag ("rotatetutorial")) {
			if (rotateTutorialEnable) {
				rotateCameraButtonChangeColor = true;
				Time.timeScale = 0;
				rotateTutorialEnable = false;
			}
		}
		if (other.gameObject.CompareTag ("DeadSea")) {
			gameOver = true;
			//			other.gameObject.SetActive (false);
		}

//		if (other.gameObject.CompareTag ("EndPoint")) {
//			gameOver = true;
//
//			//			other.gameObject.SetActive (false);
//		}

		if (other.gameObject.CompareTag ("ground")) {
			IsInAir = false;
		}

		if (other.gameObject.CompareTag ("checkpoint2")) {
			this.transform.parent = null;
			GameObject curcheckobject = checkpoint2_object;

			if(! checkpoints.Contains(curcheckobject.transform.position))
			{
				checkpoints.Add (curcheckobject.transform.position);
			}
		}

		if (other.gameObject.CompareTag ("checkpoint1")) {
			this.transform.parent = null;
			print("check");
			GameObject curcheckobject = checkpoint1_object;

			if(! checkpoints.Contains(curcheckobject.transform.position))
			{
				checkpoints.Add (curcheckobject.transform.position);
			}
		}

		if (other.gameObject.CompareTag ("Poison")) {
			print ("this is knot p");
			other.gameObject.SetActive (false);

			GameObject arrow = GameObject.FindGameObjectWithTag("PoinsonArrow");
			arrow.SetActive(false);

			this.becomeSmallerTriggerEnable = false;

			transform.localScale -= new Vector3 (4F, 4F, 4F);


		}

		if (other.gameObject.CompareTag ("test")) {
			other.gameObject.SetActive (false);
			transform.localScale += new Vector3 (4F, 4F, 4F);

			GameObject arrow = GameObject.FindGameObjectWithTag("LargePoisonTag");
			arrow.SetActive(false);

//			this.becomeSmallerTriggerEnable = false;



		}

		if (other.gameObject.CompareTag ("becomeSmallerTrigger")) {
//			if (becomeSmallerTriggerEnable) {
//				rotateCameraButtonChangeColor = true;
//				Time.timeScale = 0;
//				hintCanvas.SetActive (true);
//				becomeSmallerTriggerEnable = false;
//			}
			if (this.becomeSmallerTriggerEnable == false)
				return;
			this.becomeSmallerTriggerEnable = false;
//			this.triggerTest.SetActive(true);
			this.hintCanvas.SetActive (true);
			this.stop = true;
			this.currentHintObject = this.triggerTest;
		}

		if (other.gameObject.CompareTag ("CaveGround")) {
			GameObject arrow = GameObject.FindGameObjectWithTag("CaveArrow");
			arrow.SetActive(false);

		}

		if (other.gameObject.CompareTag ("Gift")) {
			other.gameObject.SetActive (false);
			PlayerController.Score += 1;
			//			print ("player controller");
			//			print (PlayerController.Score);

		}

		if (other.gameObject.CompareTag ("EndPoint")) {
			gameOver = true;
			gameComplete = true;
//			other.gameObject.SetActive (false);
			CardBoxAnimationLast testLast = other.gameObject.GetComponent<CardBoxAnimationLast>();
			testLast.shrinking = true;
		}

		//GiftCube
		if (other.gameObject.CompareTag ("GiftCube")) {
			
			other.gameObject.SetActive (false);

			PlayerController.RotateMove = true;

			GameObject arrow = GameObject.FindGameObjectWithTag("CaveArrow");
			arrow.SetActive(false);

			CardBoxAnimationSecondCard testSecond = other.gameObject.GetComponent<CardBoxAnimationSecondCard>();
			testSecond.shrinking = true;

			CardBoxAnimation test = other.gameObject.GetComponent<CardBoxAnimation>();
			test.shrinking = true;
		}

		if (other.gameObject.CompareTag ("Icecream")) {
			other.gameObject.SetActive (false);
		}

		if (other.gameObject.CompareTag ("Arrow")) {
			other.gameObject.SetActive (false);
		}

		if (other.gameObject.CompareTag ("Star")) {
			other.gameObject.SetActive (false);
		}
			
		if (other.gameObject.CompareTag ("Coins")) {
			coin_score_animation_level1 test = other.gameObject.GetComponent<coin_score_animation_level1>();
			test.shrinking = true;

//			other.gameObject.SetActive (false);
			count = count + 1;
			scoreText.text = "Score: " + (count * 10).ToString ();
			this.audio.Play ();
		}

		if (other.gameObject.CompareTag ("CardBox")) {
			CardBoxAnimation test = other.gameObject.GetComponent<CardBoxAnimation>();
			test.shrinking = true;
			test.card = GameObject.FindGameObjectWithTag(string.Concat("CardLevel", this.cardNumber));
			test.card.SetActive (true);
//			other.gameObject.SetActive (false);


			// eat level card, set level card above
			GameObject.FindGameObjectWithTag(string.Concat("LevelCard", this.cardNumber)).GetComponent<Image>().color = Color.green;
			this.cardNumber = this.cardNumber + 1;
		}
			



	}

	// game over and restart
	public void GameOver ()
	{
		fader.gameObject.SetActive (true);
		// check whether the player complete the mission
		if (count * 10 >= 1000 && cardNumber == 3 && gameComplete == true) {
			
			gameOverText.text = "Mission Complete!";
			nextLevelButton.gameObject.SetActive (true);

			// update the high score in PlayerPref and update the high score text in game
			PlayerPrefs.SetInt("Level1High", count * 10);
			this.HighScoreText.text = "High: " + PlayerPrefs.GetInt("Level1High");
		} else {
			gameOverText.text = "Mission Failed!";
//			nextLevelButton.gameObject.SetActive (false);

		}

		if (gameComplete) {
			resumeButton.gameObject.SetActive (false);

		} else {
			resumeButton.gameObject.SetActive (true);
		}


		//finalScoreText.text = "Score: " + (count * 10).ToString();
		gameOver = true;
	}

	public void Restart ()
	{
		
		gameOverText.text = "";
		//finalScoreText.text = "";
		restartButton.gameObject.SetActive (false);
		resumeButton.gameObject.SetActive (false);
		nextLevelButton.gameObject.SetActive (false);

		fader.gameObject.SetActive (false);
//		backtomainButton.gameObject.SetActive (false);
		//Application.LoadLevel ("PartOne");
		SceneManager.LoadScene("PartOne");

//		PlayerController.Score = 0;
//		PlayerController.RotateMove = false;
	}
	public void Backtomain (){
		gameOverText.text = "";
		//finalScoreText.text = "";
		restartButton.gameObject.SetActive (false);
		resumeButton.gameObject.SetActive (false);
		nextLevelButton.gameObject.SetActive (false);
//		backtomainButton.gameObject.SetActive (false);
		//Application.LoadLevel ("PartOne");
		SceneManager.LoadScene("StartMenu");
	}

	public void NextLevel () {
		SceneManager.LoadScene("PartTwo");
	}
		

	/* kitten begin */
	public void  Awake ()
	{
		// load high score from PlayerPrefs
		this.HighScoreText.text = "High: " + PlayerPrefs.GetInt("Level1High");

		// TODO: load music options and control music
		if (PlayerPrefs.GetInt("music") == 1) {
			// TODO
		}



		moveDirection = transform.TransformDirection (Vector3.forward);
		anim = GetComponent<Animator>();
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
//		Transform cameraTransform = Camera.main.transform;
		bool grounded = IsGrounded ();
        
		// Forward vector relative to the camera along the x-z plane
		Vector3 forward =Vector3.forward;
		if (maincamera.enabled) {
			Transform cameraTransform = Camera.main.transform;
			forward = cameraTransform.TransformDirection (Vector3.forward);
		}
//		Vector3 forward = cameraTransform.TransformDirection (Vector3.forward);
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
		

		if (this.didReadWelcomeText == false && Input.GetMouseButtonDown(0)){
			this.missionCanvas.SetActive (false);
			this.didReadWelcomeText = true;
			this.welcomeText.SetActive (false);

		}
		if (this.didReadWelcomeText == false) {
			return;
		}
		if (Input.GetMouseButtonDown(0)){
			this.stop = false;
			if (this.currentHintObject != null) {
				this.currentHintObject.SetActive (false);
			}
			if (this.hintCanvas != null) {
				this.hintCanvas.SetActive (false);
			}
			if (this.missionCanvas != null) {
				this.missionCanvas.SetActive (false);
			}

		}
		if (this.stop == true)
			return;
		
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

	public void Resume()
	{

		if (checkpoints.Count == 1) {
			Restart ();
			return;
		} else if (checkpoints.Count == 2) {
			PlayerController.RotateMove = false;
			rotateplatform.transform.position = rotateplatforminitialposition;
			roomgift.SetActive (true);
			//			GameObject arrow = GameObject.FindGameObjectWithTag("CaveArrow");
			//			arrow.SetActive(true);
		} 
		fader.gameObject.SetActive (false);
		gameOver = false;
		restart = false;
		restartText.text = "";
		gameOverText.text = "";
		//finalScoreText.text = "";
		resumeText.text="";

		IsInAir = false;


		restartButton.gameObject.SetActive (false);
		resumeButton.gameObject.SetActive (false);
//		backtomainButton.gameObject.SetActive (false);

		//resumepoint store the position to resume
		Vector3 resumepoint = checkpoints [checkpoints.Count - 1];
		checkpoints.RemoveAt (checkpoints.Count - 1);

		SetBallStartPoint (resumepoint);
		//		PlayerController.Score = 3;

	}

	public void SetBallStartPoint (Vector3 resumepoint)
	{
		transform.position = resumepoint;

	}

	bool IsInGitfRoom ()
	{
		if (this.transform.position.x < wall_x_max.transform.position.x & this.transform.position.x > wall_x_min.transform.position.x) {
			if (this.transform.position.y < (wall_y_max.transform.position.y - 30) & this.transform.position.y > wall_y_min.transform.position.y) {
				if (this.transform.position.z < wall_z_max.transform.position.z & this.transform.position.z > wall_z_min.transform.position.z) {
					return true;
				}
			}
		}
		return false;
	}
}