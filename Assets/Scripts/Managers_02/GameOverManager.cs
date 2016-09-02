using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameOverManager : MonoBehaviour
{
    public PlayerHealth playerHealth;
	public float restartDelay = 5f;


    Animator anim;

	public Transform restartButton;
	public Text restartText;
	public Text gameOverText;
	public Text homeText;
	public static bool missionComplete = false;

	private bool restart;
	void Start(){
		
		restart = false;
		restartText.text = "";
		gameOverText.text = "";
		restartButton.gameObject.SetActive (false);

	}
    void Awake()
    {
        anim = GetComponent<Animator>();
    }


    void Update()
    {
		if (playerHealth.currentHealth > 0 && missionComplete == true) {
			anim.SetTrigger ("GameOver");
			gameOverText.text = "Mission Complete!";
			restartButton.gameObject.SetActive (true);
			restartText.text = "Restart";
			restart = true;
		}

		if (playerHealth.currentHealth <= 0) {
			anim.SetTrigger ("GameOver");
			gameOverText.text = "Mission Failed!";
			restartButton.gameObject.SetActive (true);
			restartText.text = "Restart";
			restart = true;
		}
    }




	public void Restart ()
	{

		gameOverText.text = "";
		restartButton.gameObject.SetActive (false);
		GameOverManager.missionComplete = false;
		SceneManager.LoadScene("PartTwo");

	}
	public void Backtomain (){
		gameOverText.text = "";
		//finalScoreText.text = "";
		restartButton.gameObject.SetActive (false);
		//		backtomainButton.gameObject.SetActive (false);
		//Application.LoadLevel ("PartOne");
		SceneManager.LoadScene("StartMenu");
	}

}
