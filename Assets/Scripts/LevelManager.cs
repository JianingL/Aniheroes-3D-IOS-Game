using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
	public Transform mainMenu, optionsMenu, levelMenu;
	public Text musicText;

	private string onOff;


	void Awake () {
		// load music options from PlayerPrefs
		this.onOff = (PlayerPrefs.GetInt("music") == 1) ? "On" : "Off";
		this.musicText.text = "Music " + this.onOff;
	} 

	public void LoadScence(string sceneName){
		//Application.LoadLevel (sceneName);

		SceneManager.LoadScene(sceneName);
	}
	public void QuitGame(){
		Application.Quit ();
	}
	public void OptionsMenu(bool clicked){
		if (clicked == true) {
			optionsMenu.gameObject.SetActive (clicked);
			mainMenu.gameObject.SetActive (false);
		} else {
			optionsMenu.gameObject.SetActive (clicked);
			mainMenu.gameObject.SetActive (true);
		}
		
	}
	public void LevelMenu(bool clicked){
		if (clicked == true) {
			levelMenu.gameObject.SetActive (clicked);
			mainMenu.gameObject.SetActive (false);
		} else {
			levelMenu.gameObject.SetActive (clicked);
			mainMenu.gameObject.SetActive (true);
		}
	}

	public void SetMusic () {
		if (PlayerPrefs.GetInt ("music") == 1) {
			PlayerPrefs.SetInt ("music", 0);
		} else {
			PlayerPrefs.SetInt ("music", 1);
		}

		this.onOff = (PlayerPrefs.GetInt("music") == 1) ? "On" : "Off";
		this.musicText.text = "Music " + this.onOff;
	}
}
