using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour{
	//public Animation animate1,animate2;
	public Image logo;
	public GameObject startButton;
	public GameObject sceneTint;
	bool startClicked=false;
	public SceneManagerClassv2 sceneManager;

	// Use this for initialization
	bool demoMode=false;


	void Start () {
		PlayerPrefsX.SetBool ("demoMode", demoMode);
	}
	public void startPressed(){
		if (!startClicked) {
			StartCoroutine (startGame ());
		}
	}
	IEnumerator startGame(){
		startClicked = true;
		startButton.GetComponent<Button> ().interactable = false;
		//PlayerPrefs.DeleteAll ();
		this.GetComponent<AudioSource> ().Play ();
//		startButton.GetComponent<Animation> ().Play ("mainMenuStartPressed");
		startButton.GetComponentInChildren<Animator>().SetBool("Clicked",true);

		yield return new WaitForSeconds(1f);
		startButton.GetComponentInChildren<Text>().text="";
//		SceneManagerClass.fadeToBlack (sceneTint);
//		yield return new WaitForSeconds (2f);
		if(!PlayerPrefsX.GetBool("demoMode")){
			if (PlayerPrefsX.GetBool ("WDWActivated", false)) {
				StartCoroutine (sceneManager.changeSceneWithLoadingEnumNoCon (1));
			} else {
				PlayerPrefsX.SetBool ("WDWActivated", true);
				StartCoroutine (sceneManager.changeSceneWithLoadingEnumNoCon (6));
			}
		
		}else{
			PlayerPrefs.SetString ("dialougeFile", "demoMode");
			PlayerPrefs.SetInt ("lineCounter",0);
			StartCoroutine (sceneManager.changeSceneWithLoadingEnum (2));
		}
	}
	// Update is called once per frame

}