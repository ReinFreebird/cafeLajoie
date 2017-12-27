using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
public class MainMenuv2 : MonoBehaviour {

	//ListOfPlayerPrefsAvailable

	// Use this for initialization
	//Object in Canvas
	public GameObject charaSprite;
	public GameObject dialougeBubble;
	public Text dialougeText;
	public GameObject settingButton;
	public GameObject homeButton;
	public Sprite[] charaSpriteArray;
	public GameObject mainButton;
	public string[] miyukiDialouge;
	public string[] mizuoDialouge;

	//Object for Story
	public GameObject storySelect;
	public GameObject chapterCharaSelect;
	public GameObject chapterMiyukiSelect;
	public GameObject chapterMizuoSelect;
	public Button[] MiyukiChapter;
	public Button[] MizuoChapter;
	public GameObject storyContinuePanel;
	public GameObject extraChapter;
	bool charaStory;//True= miyuki, else Mizuo

	//Object for Gallery
	public GameObject gallerySelect;
	//public GameObject CGSelect;
	//public GameObject rewardSelect;

	//CG and Reward Object
	public GameObject CGMainPanel;
	public Sprite[] CGImage;
	public Sprite CGDefault;
	public GameObject CGDisplay;
	public Button[] CGButton;

	public Button[] RewardButton;
	public Sprite[] rewardBadge;
	public Sprite rewardDefault;
	public string[] rewardName;
	public string[] rewardTag;
	public GameObject rewardDisplay;
	public Text rewardNameText;
	public Text rewardTagText;
	public GameObject rewardLogo;

	//Object for Extra
	public GameObject extraSelect;
	public GameObject extraChoiceSelect;
	public GameObject freeModeSelect;
	public Button FWButton;
	public Button HnLButton;
	public GameObject freeContinuePanel;
	public GameObject highScoreSelect;
	bool gameSelected;//true= FW, else HnL

	//HighScores Object
	public GameObject FWDisplay;
	public GameObject HnLDisplay;
	public Text[] FWName;
	public Text[] FWScore;
	public Text[] HnLName;
	public Text[] HnLScore;

	//Object for Setting
	public GameObject settingPanel;
	public GameObject BGMToggle;
	public GameObject SFXToggle;
	public GameObject charaSelect;
	public GameObject charaDrop;
	public GameObject SpeedSlider;
	public GameObject resetPanel;
	public Text speedText;

	//Other
	public GameObject sceneTint;
	public AudioClip[] clips;
	public AudioSource SFXsource;
	public AudioSource BGMsource;
	public GameObject[] ScreenManagerArray;
	int currentScreen=0;
	//Home=0
	//Story=1
	//Gallery=2
	//Extra=3
	public SceneManagerClassv2 sceneManager;
	public GameObject touchBlocker;
	bool dialougeChanging;


	void resetChapter(){
	//For debug only
	
	}

	void freeModeSetup(){
		//Uncomment to unlock game

		//PlayerPrefsX.SetBool ("FWUnlocked", true);
		//PlayerPrefsX.SetBool ("HnLUnlocked", true);
		if(PlayerPrefsX.GetBool("FWUnlocked",false)){
			FWButton.interactable = true;
			FWButton.GetComponentInChildren<Text> ().text = "Food Wars";
		}else{
			FWButton.interactable = false;
			FWButton.GetComponentInChildren<Text> ().text = "LOCKED";
		}
		if (PlayerPrefsX.GetBool ("HnLUnlocked", false)) {
			HnLButton.interactable = true;
			HnLButton.GetComponentInChildren<Text> ().text = "High and Low";
		} else {
			HnLButton.interactable = false;
			HnLButton.GetComponentInChildren<Text> ().text = "LOCKED";
		}
	}


	public void mainMenuSpriteClicked(){
		if (!dialougeChanging) {
			if (currentScreen == 0) {
				if (PlayerPrefs.GetInt ("MenuChara", 0) == 0) {
					StartCoroutine(changeDialouge(miyukiDialouge[UnityEngine.Random.Range(0,miyukiDialouge.Length)]));
				} else {
					StartCoroutine(changeDialouge(mizuoDialouge[UnityEngine.Random.Range(0,mizuoDialouge.Length)]));
				}
			} else {
			
			}
		}
	}
	IEnumerator changeDialouge(string s){
		dialougeChanging = true;
		dialougeText.GetComponent<Text> ().text ="";
		dialougeBubble.GetComponent<Animation>().Play();
		yield return new WaitForSeconds (0.8f);
		dialougeText.GetComponent<Text> ().text = s;
		dialougeChanging = false;
	}

	public void toggleSetting(bool activate){
		if (activate) {
			settingPanel.SetActive (true);
			settingButton.SetActive (false);
			dialougeBubble.SetActive (false);
		} else {
			settingPanel.SetActive (false);
			settingButton.SetActive (true);
			dialougeBubble.SetActive (true);
		}
	}

	void updateSetting(){
		if (PlayerPrefs.GetInt ("BGM", 1) == 1) {
			BGMsource.mute = false;
			BGMToggle.GetComponent<Toggle> ().isOn = true;
		} else {
			BGMsource.mute = true;
			BGMToggle.GetComponent<Toggle> ().isOn = false;
		}
		if (PlayerPrefs.GetInt ("SFX", 1) == 1) {
			SFXsource.mute = false;
			SFXToggle.GetComponent<Toggle> ().isOn = true;
		} else {
			SFXsource.mute = true;
			SFXToggle.GetComponent<Toggle> ().isOn = false;
		}
		charaDrop.GetComponent<Dropdown> ().value = PlayerPrefs.GetInt ("MenuChara",0);
		charaSprite.GetComponent<Image> ().sprite = charaSpriteArray [PlayerPrefs.GetInt ("MenuChara", 0)];
		SpeedSlider.GetComponent<Slider> ().value = PlayerPrefs.GetFloat("Speed",2.5f);
		speedText.text = PlayerPrefs.GetFloat("Speed").ToString();
		//updateSettingGUI ();
	}
	void playClip(int x){
		SFXsource.clip = clips [x];
		SFXsource.Play ();
	}
	public void changeBGM(bool bgm){
		if (!bgm) {
			PlayerPrefs.SetInt ("BGM", 0);
			BGMsource.mute = true;
		} else {
			PlayerPrefs.SetInt ("BGM", 1);
			BGMsource.mute = false;
		}
		playClip (0);
	}
	public void changeSFX(bool sfx){
		if (!sfx) {
			PlayerPrefs.SetInt ("SFX", 0);
			SFXsource.mute = true;
		} else {
			PlayerPrefs.SetInt ("SFX", 1);
			SFXsource.mute = false;
		}
		playClip (0);
	}
	public void changeChara(int chara){
		PlayerPrefs.SetInt ("MenuChara", chara);
			charaSprite.GetComponent<Image> ().sprite = charaSpriteArray [chara];
	}
	public void changeSpeed(){
		PlayerPrefs.SetFloat ("Speed",(float)Math.Round( SpeedSlider.GetComponent<Slider> ().value,1));
		speedText.text =PlayerPrefs.GetFloat("Speed").ToString();
	}
	void updateSettingGUI(){
		if (PlayerPrefs.GetInt ("BGM") == 1) {
			BGMToggle.GetComponent<Toggle> ().isOn = true;
		} else {
			BGMToggle.GetComponent<Toggle> ().isOn = false;
		}
		if (PlayerPrefs.GetInt ("SFX") == 1) {
			SFXToggle.GetComponent<Toggle> ().isOn = true;
		} else {
			SFXToggle.GetComponent<Toggle> ().isOn = false;
		}
		if (!PlayerPrefs.HasKey("MenuChara")) {
			PlayerPrefs.SetInt ("MenuChara", 0);
			charaDrop.GetComponent<Dropdown> ().value = 0;
		} else {
			charaDrop.GetComponent<Dropdown> ().value = PlayerPrefs.GetInt ("MenuChara");
		}
		if (!PlayerPrefs.HasKey("Speed")) {
			PlayerPrefs.SetFloat ("Speed", 2.5f);
			SpeedSlider.GetComponent<Slider> ().value = 2.5f;
		} else {
			SpeedSlider.GetComponent<Slider> ().value = PlayerPrefs.GetFloat("Speed");
		}
	}
	public void applyChange(bool apply){
		if (apply) {
			int bgm = 0;
			int sfx = 0;
			int chara = 0;
			float speed = 0;
			if (BGMToggle.GetComponent<Toggle> ().isOn) {
				bgm = 1;
			}
			if (SFXToggle.GetComponent<Toggle> ().isOn) {
				bgm = 1;
			}
			PlayerPrefs.SetInt ("BGM", bgm);
			PlayerPrefs.SetInt ("SFX", sfx);
			PlayerPrefs.SetFloat ("Speed", SpeedSlider.GetComponent<Slider> ().value);
		}
		//updateSetting ();
		playClip (2);
		settingPanel.SetActive (false);
	}

	void Start () {
		PlayerPrefsX.GetBool ("freeMode", true);
		postHighScore ();
		updateSetting ();
		chapterPlayerPrefsSetup ();
		setupCharacterChapterSelection ();
		highScorePrefsSetup ();
		CGRewardSetup ();
		freeModeSetup ();
		mainMenuSpriteClicked ();
	}

	void setupCharacterChapterSelection(){
		if (PlayerPrefsX.GetBool ("ChapterPrefsCreated", false)) {
			bool[] prefTempMiyuki = PlayerPrefsX.GetBoolArray ("MiyukiChapter");
			bool[] prefTempMizuo = PlayerPrefsX.GetBoolArray ("MizuoChapter");
			MiyukiChapter[0].interactable = prefTempMiyuki [0];
			MiyukiChapter[1].interactable = prefTempMiyuki [1];
			MiyukiChapter[2].interactable = prefTempMiyuki [2];
			MizuoChapter[0].interactable = prefTempMizuo [0];
			MizuoChapter [1].interactable = prefTempMizuo [1];
			MizuoChapter [2].interactable = prefTempMizuo [2];
		}
	}
	void CGRewardSetup(){
		bool extra=true;
		bool[] CG = new bool[3];
		bool[] reward = new bool[6];
		if(PlayerPrefsX.GetBool("CGRewardPrefsCreated",false)){
			CG = PlayerPrefsX.GetBoolArray ("CGArray");
			reward = PlayerPrefsX.GetBoolArray ("RewardArray");
			for (int i = 0; i <=5; i++) {
				if (i < 3) {
					Debug.Log (CG [i]);
					CGButton [i].interactable = CG [i];
					if (CG [i]) {
						CGButton [i].GetComponent<Image> ().sprite = CGImage [i];
						CGButton [i].GetComponentInChildren<Text> ().text = "";
					} else {
						CGButton [i].GetComponent<Image> ().sprite = CGDefault;
					}

				}
				RewardButton [i].interactable = reward [i];
				Debug.Log (reward [i]);
				if (reward [i]) {
					RewardButton [i].GetComponent<Image> ().sprite = rewardBadge [i];
					RewardButton [i].GetComponentInChildren<Text> ().text = "";
				} else {
					RewardButton [i].GetComponent<Image> ().sprite = rewardDefault;
					extra = false;
				}

			}
		
		}else{
			for (int i = 0; i <=5; i++) {
				if (i < 3) {
					CG [i] = false;
				}
				reward [i] = false;
			}
			PlayerPrefsX.SetBool ("CGRewardPrefsCreated", true);
			PlayerPrefsX.SetBoolArray ("CGArray", CG);
			PlayerPrefsX.SetBoolArray ("RewardArray", reward);
		}
		if (extra) {
			extraChapter.SetActive (true);
		} else {
			extraChapter.SetActive (false);
		}
	}
	public void showReward(int x){
		playClip (0);
		CGMainPanel.SetActive (false);
		rewardNameText.text = rewardName [x];
		rewardTagText.text = rewardTag [x];
		rewardLogo.GetComponent<Image> ().sprite = rewardBadge [x];
		rewardDisplay.SetActive (true);
		mainButton.SetActive (false);
		charaSprite.SetActive (false);
	}
	public void closeReward(){
		playClip (2);
		CGMainPanel.SetActive(true);
		rewardDisplay.SetActive (false);
		mainButton.SetActive (true);
		charaSprite.SetActive (true);
	}
	public void showCG(int x){
		playClip (0);
		Sprite getCG = CGButton [x].GetComponent<Image> ().sprite;
		CGDisplay.GetComponent<Image> ().sprite = getCG;
		CGDisplay.gameObject.SetActive (true);
		mainButton.SetActive (false);
		charaSprite.SetActive (false);
	}
	public void closeCG(){
		playClip (2);
		CGDisplay.gameObject.SetActive (false);
		mainButton.SetActive (true);
		charaSprite.SetActive (true);
	}
	void chapterPlayerPrefsSetup(){
		bool []miyukiChapter = new bool[4];
		bool []mizuoChapter = new bool[4];
		if (!PlayerPrefsX.GetBool ("ChapterPrefsCreated", false)) {
			miyukiChapter [0] = true;
			miyukiChapter [1] = false;
			miyukiChapter [2] = false;
			miyukiChapter [3] = false;
			mizuoChapter [0] = true;
			mizuoChapter [1] = false;
			mizuoChapter [2] = false;
			mizuoChapter [3] = false;
			PlayerPrefsX.SetBoolArray ("MiyukiChapter", miyukiChapter);
			PlayerPrefsX.SetBoolArray ("MizuoChapter", mizuoChapter);
			PlayerPrefsX.SetBool ("ChapterPrefsCreated", true);
		}
	}
	void highScorePrefsSetup(){
		string[] FWName = new string[5];
		int[] FWPoint = new int[5];
		string[] HnLName = new string[5];
		int[] HnLPoint = new int[5];
		for (int i = 0; i < 5; i++) {
			FWName [i] = "Miyuki";
			HnLName [i] = "Mizuo";
			FWPoint [i] = 250*(5-i);
			HnLPoint [i] = 300-(i*50);
		}
		if (!PlayerPrefsX.GetBool ("HighScorePrefsCreated", false)) {
			for (int i = 0; i < 5; i++) {
				FWName [i] = "Miyuki";
				HnLName [i] = "Mizuo";
				FWPoint [i] = 250*(5-i);
				HnLPoint [i] = 300-(i*50);
			}
			PlayerPrefsX.SetStringArray ("FWName", FWName);
			PlayerPrefsX.SetStringArray ("HnLName", HnLName);
			PlayerPrefsX.SetIntArray ("FWPoint", FWPoint);
			PlayerPrefsX.SetIntArray ("HnLPoint", HnLPoint);
			PlayerPrefsX.SetBool ("HighScorePrefsCreated", true);
		}

	}
	public void resetHighScore(int state){
		if (state == 1) {//Reset HighScore
			playClip (0);
			PlayerPrefsX.SetBool ("HighScorePrefsCreated", false);
			highScorePrefsSetup ();
			settingPanel.SetActive (true);
			resetPanel.SetActive (false);
			mainButton.SetActive (true);
			postHighScore ();
		} else if (state == 0) {//Don't reset
			playClip (2);
			settingPanel.SetActive (true);
			mainButton.SetActive (true);
			resetPanel.SetActive (false);
		} else {//Open Panel
			playClip(0);
			mainButton.SetActive (false);
			resetPanel.SetActive(true);
		}
	}
	void postHighScore(){
		if (PlayerPrefsX.GetBool ("HighScorePrefsCreated", false)) {
			string[] stringTemp;
			int[] intTemp;
			stringTemp = PlayerPrefsX.GetStringArray ("FWName");
			intTemp = PlayerPrefsX.GetIntArray ("FWPoint");
			for (int i = 0; i < 5; i++) {
				FWName [i].text = stringTemp [i];
				FWScore [i].text = intTemp [i].ToString ();
			}
			stringTemp = PlayerPrefsX.GetStringArray ("HnLName");
			intTemp = PlayerPrefsX.GetIntArray ("HnLPoint");
			for (int i = 0; i < 5; i++) {
				HnLName [i].text = stringTemp [i];
				HnLScore [i].text = "$" + intTemp [i].ToString ();
			}
		}
	}
	string getDate(){
		string day,date,month,year;

		day = System.DateTime.Today.Day.ToString();
		month = System.DateTime.Today.Month.ToString ();
		year = System.DateTime.Today.Year.ToString ();
		return day+" "+month+" "+year;
	}
	// Update is called once per frame
	void Update () {
		
	}
	public void subMenuSelect(int selectScreen){
		resetMenu ();
		playClip (1);
		switch (selectScreen) {
		case 0:
			StartCoroutine (activeHome ());
			break;
		case 1:
			StartCoroutine (activeStory ());
			break;
		case 2:
			StartCoroutine (activeGallery());
			break;
		case 3:
			StartCoroutine (activeExtra());;
			break;
		}
	}
	void resetMenu(){
		storySelect.SetActive (false);
		extraSelect.SetActive (false);
		gallerySelect.SetActive (false);
		chapterCharaSelect.SetActive (true);
		chapterMizuoSelect.SetActive(false);
		chapterMiyukiSelect.SetActive (false);
		extraChoiceSelect.SetActive (true);
		freeModeSelect.SetActive (false);
		highScoreSelect.SetActive (false);
		toggleSetting (false);
		settingButton.SetActive (false);
		dialougeBubble.SetActive (false);
		dialougeChanging = false;
	}
	IEnumerator activeHome(){
		charaSprite.SetActive (true);
		charaSprite.transform.DOMoveX (0, 0.5f);
		yield return new WaitForSeconds (0.5f);
		dialougeBubble.SetActive (true);
		settingButton.SetActive (true);
		charaSprite.SetActive (true);
		currentScreen = 0;
		mainMenuSpriteClicked ();
	}
	IEnumerator activeStory(){
		chapterMiyukiSelect.SetActive(false);
		chapterMizuoSelect.SetActive(false);
		charaSprite.SetActive (true);
		charaSprite.transform.DOMoveX (-1.1f, 0.5f);
		yield return new WaitForSeconds (0.5f);
		dialougeBubble.SetActive (false);
		storySelect.SetActive (true);
		currentScreen = 1;
	}
	IEnumerator activeGallery(){
		charaSprite.transform.DOMoveX (-5f, 0.5f);
		dialougeBubble.SetActive (false);
		yield return new WaitForSeconds (0.5f);
		gallerySelect.SetActive (true);
		currentScreen = 2;
	}
	IEnumerator activeExtra(){
		charaSprite.SetActive (true);
		dialougeBubble.SetActive (false);
		charaSprite.transform.DOMoveX (-1.1f, 0.5f);
		yield return new WaitForSeconds (0.5f);
		extraSelect.SetActive (true);
		currentScreen = 3;
	}
	public void highScoreToggle(bool score){
		if (score) {	//True, toggle Food Wars HighScore
			FWDisplay.SetActive(!FWDisplay.activeSelf);
			HnLDisplay.SetActive(false);
		} else {		//False, toggle High and Low HighScore
			HnLDisplay.SetActive(!HnLDisplay.activeSelf);
			FWDisplay.SetActive(false);
		}
	}
	public void charaChapterSelect(bool chara){
		chapterCharaSelect.SetActive (false);
		if (chara) {//Miyuki
			chapterMiyukiSelect.SetActive(true);
		} else {//Mizuo
			chapterMizuoSelect.SetActive(true);
		}
	}
	public void extraFirstSelect(bool choice){
		extraChoiceSelect.SetActive (false);
		if(choice){
			freeModeSelect.SetActive (true);
		}else{
			StartCoroutine (highScoreActive ());
		}
	}
	IEnumerator highScoreActive(){
		charaSprite.transform.DOMoveX (-5f, 0.5f);
		yield return new WaitForSeconds (0.5f);
		highScoreSelect.SetActive (true);
		FWDisplay.SetActive (false);
		HnLDisplay.SetActive (false);
	}
	public void chapterSelection(string s){
		PlayerPrefs.SetString("dialougeFile",s);
		if (s == "extraChapter") {
			chapterCharaSelect.SetActive (false);
			mainButton.SetActive (false);
			StartCoroutine (toConversation (true));
		} else {
			storyContinuePanel.SetActive (true);
			mainButton.SetActive (false);
			if (chapterMiyukiSelect.activeSelf) {
				chapterMiyukiSelect.SetActive (false);
				charaStory = true;
			} else {
				chapterMizuoSelect.SetActive (false);
				charaStory = false;
			}
		}
		playClip (0);
	}
	public void continueScene(bool b){
		if (b == true) {
			playClip (0);
			//Change Scene Mizuo
			storyContinuePanel.SetActive(false);
			if (!charaStory) {
				StartCoroutine (toConversation (true));
			}else {//Change Scene Miyuki
				StartCoroutine (toConversation (false));
			}
		} else {
			playClip (2);
			storyContinuePanel.SetActive(false);
			mainButton.SetActive (true);
			if (!charaStory) {//Mizuo
				chapterMizuoSelect.SetActive(true);
			} else {//Miyuki
				chapterMiyukiSelect.SetActive(true);
			}
		}
	}
	IEnumerator toConversation(bool b){
		PlayerPrefsX.SetBool ("freeMode", false);
		if (b) {//Mizuo
			charaSprite.transform.DOMoveX (0, 1f);
			//PlayerPrefs.SetString("dialougeFile","mizuoIntro");
			PlayerPrefs.SetInt("lineCounter",0);
			yield return new WaitForSeconds (1.5f);
		} else {//Miyuki
			charaSprite.transform.DOMoveX (0, 1f);
			//PlayerPrefs.SetString("dialougeFile","miyukiIntro");
			PlayerPrefs.SetInt("lineCounter",0);
			yield return new WaitForSeconds (1.5f);
		}
		StartCoroutine (sceneManager.changeSceneWithLoadingEnum (2));
	}
	public void freeGameSelect(bool b){
		freeContinuePanel.SetActive (true);
		mainButton.SetActive (false);
		freeModeSelect.SetActive (true);
		if (b) {// Food Wars
			gameSelected=true;
		} else {// High and Low
			gameSelected=false;
		}
	
	}
	public void freeGameContinue(bool b){
		if (b == true) {
			playClip (0);
			//Change Scene Mizuo
			freeContinuePanel.SetActive(false);
			freeModeSelect.SetActive (false);
			StartCoroutine (toGame (gameSelected));

		} else {
			playClip (2);
			mainButton.SetActive (true);
			freeContinuePanel.SetActive (false);
			freeModeSelect.SetActive (true);
		}
	}
	IEnumerator toGame(bool b){
		charaSprite.transform.DOMoveX (0, 1f);
		PlayerPrefsX.SetBool ("freeMode", true);
		if (b) {//FW
			yield return new WaitForSeconds (1.5f);
			StartCoroutine (sceneManager.changeSceneWithLoadingEnum (3));
		} else {//HnL
			yield return new WaitForSeconds (1.5f);
			StartCoroutine (sceneManager.changeSceneWithLoadingEnum (4));
		}

	
	}
}
