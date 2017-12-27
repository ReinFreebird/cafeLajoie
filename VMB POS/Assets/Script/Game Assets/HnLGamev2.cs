using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class HnLGamev2 : MonoBehaviour {

	//GameObjects
	public Sprite[] cardFront;
	public Sprite backFront;

	//Object in canvas
	public GameObject highLowPanel;
	public GameObject[] sprite=new GameObject[5];
	public GameObject charaSprite;
	public GameObject dialougeBubble;
	public GameObject continueButton;
	public GameObject mainPanel;
	public GameObject startButton;
	public GameObject endGamePanel;
	public GameObject title;
	public Text finalScore;
	public Text startText;
	public Text dialougeText;
	public Text currentPotText;
	public Text playerBalanceText;
	public Text nextRoundBet;
	public Text winningText;
	public Text roundNumber;
	public Text trueOrFalse;
	public Button highButton, lowButton;
	public GameObject tutorialPanel;
	public Button[] tutorialButton;// 0 is right button, 1 is left button
	public Sprite[] tutorialPage;//0 is page 1, 1 is page 2
	public Button helpButton,endButton;
	//Other Game Object
	List<AudioSource> audiosource=new List<AudioSource>();
	public AudioClip [] soundEffect;
	public AudioClip BGM;
	Animator charaAnim,startAnim;
	public SceneManagerClassv2 sceneManager;
	//Variabels
	bool gamePlaying=true;		//turn to false if round =10 in endRound
	bool continuePhase=false;//check if high or low is true, if false, go to next round
	bool continueRound=false;//check if  game should continue to next round, either by player guessed wrong or guessed all right
	bool dialougeChanging=false; //to avoid dialouge change when dialouge is changing
	bool busted;				//When player can no longer play (balance <current pot)
	bool cardFlipping;
	int currentRound=0;		//Current round (max 10)
	int playerBalance;		//Player's point
	int winning;			//Player's Winning
	int currentPot;			//Current pot (round 1-4=$10,5-7=$20,8-10=$50
	int phase;				//Round phase (1-4)
	int numberOfCard=52;		//Number of Cards in deck
	Card[] cards= new Card[52];	//Contain cards in deck
	Card[] cardContainer;		//Contain cards in round
	bool[] flipped=new bool[5];	//Check if card is face up
	string[][]dialouge=new string [4][];	//Chara's dialouge. [x][y]. x = type of expression, y String
	public string[]dialougeStart;		//dialouge[0]
	public string[]dialougeIdle;		//dialouge[1]
	public string[]dialougeGreat;		//dialouge[2]
	public string[]dialougeTryAgain;	//dialouge[3]
	Vector3 bubbleStartPos=new Vector3(213,-367);
	Vector3 bubbleFinPos = new Vector3 (-74, -497);
	bool tutorialPageBool;//(false page 1, true page2)

	//HighScore
	public GameObject highScoreTable;
	public Text mainText;
	public Text[]Name;
	public Text[]PointText;
	public Button continueButtonEnd;
	public GameObject textInput;
	int[] pointTemp;
	string[] nameTemp;
	int changedRanking=-1;
	void Start(){
		
		//Get animator from sprite and start
		startButton.GetComponent<Button>().interactable=false;
		charaAnim = charaSprite.GetComponent<Animator> ();
		startAnim = startText.GetComponent<Animator> ();
		//player start Balance
		playerBalance = 100;
		//Card data
		cardContainer=new Card[sprite.Length];
		sprite [0].GetComponent<Image>().sprite = backFront;
		sprite [1].GetComponent<Image>().sprite = backFront;
		sprite [2].GetComponent<Image>().sprite = backFront;
		sprite [3].GetComponent<Image>().sprite = backFront;
		sprite [4].GetComponent<Image>().sprite = backFront;
		for (int i = 0; i < flipped.Length; i++) {
			flipped [i] = false;
		}
		int x = 0;
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 13; j++) {
				cards [x] = new Card (cardFront [x], j, i);
				x++;
			}
		}
		//assign all dialouges to one array
		dialouge[0]=dialougeStart;
		dialouge [1] = dialougeIdle;
		dialouge [2] = dialougeGreat;
		dialouge [3] = dialougeTryAgain;
		StartCoroutine(changeDialouge (0, Random.Range(0,dialouge[0].Length)));
		currentRound = 0;
		shuffleCard ();
		//get audiosources from eventSystem
		for (int i = 0; i < GetComponents<AudioSource>().Length; i++) {
			audiosource.Add(this.gameObject.GetComponents<AudioSource>()[i]);
			//Add audiosource to List
			//audiosource[0]= Sound Effect
			//audiosource[1]=BGM
		}
		if (PlayerPrefs.GetInt ("SFX",1) == 0) {
			audiosource[0].mute = true;
		} else {
			audiosource[0].mute = false;
		}
		if (PlayerPrefs.GetInt ("BGM", 1) == 0) {
			audiosource[1].mute = true;
		} else {
			audiosource[1].mute = false;
		}
		audiosource [1].clip = BGM;
		//audiosource [1].Play ();

		//disable display before click start
		mainPanel.SetActive(false);
		highLowPanel.SetActive (false);
		charaSprite.GetComponent<Button> ().interactable = false;

		if (PlayerPrefsX.GetBool ("freeMode", true)&
			!PlayerPrefsX.GetBool("demoMode",false)) {
			startButton.GetComponent<Button> ().interactable = true;
		} else {
			StartCoroutine (startTutorialShow (1f));
		}
		//inisiateRound ();
	}
	IEnumerator startTutorialShow(float delay){
		yield return new WaitForSeconds (delay);
		tutorialOpen ();
	}
	public void startButtonClicked(){
		StartCoroutine (startClicked ());
	}
	IEnumerator startClicked(){
		playClip (0);
		startButton.GetComponent<Button> ().interactable = false;
		helpButton.GetComponent<Button> ().interactable = false;
		startText.gameObject.GetComponent<Animator> ().SetBool ("Clicked", true);
		yield return new WaitForSeconds (1f);
		audiosource [1].Play();
		title.SetActive (false);
		startButton.SetActive (false);
		mainPanel.SetActive (true);
		highLowPanel.SetActive (true);
		helpButton.gameObject.SetActive (false);
		currentRound = 1;
		charaSprite.GetComponent<Button> ().interactable = true;
		currentPot = 10; //first round pot
		inisiateRound ();
	}
	public void spriteButtonClicked(){
		if (!dialougeChanging) {
			StartCoroutine (spriteClicked ());
		}
	}
	public void tutorialOpen(){
		playClip (0);
		helpButton.gameObject.SetActive (false);
		tutorialPanel.SetActive (true);
		tutorialPanel.GetComponent<Image> ().sprite = tutorialPage [0];
		tutorialButton [0].gameObject.SetActive (true);
		tutorialButton [1].gameObject.SetActive (false);
		startButton.GetComponent<Button> ().interactable = false;
	}
	public void tutorialClose(){
		playClip (4);
		tutorialPanel.SetActive (false);
		helpButton.gameObject.SetActive (true);
		startButton.GetComponent<Button> ().interactable = true;
	}
	public void tutorialChangePage(bool x){
		if (!x) { //false, page 1
			tutorialPanel.GetComponent<Image> ().sprite = tutorialPage [0];
			tutorialButton [0].gameObject.SetActive (true);
			tutorialButton [1].gameObject.SetActive (false);
		} else {
			tutorialPanel.GetComponent<Image> ().sprite = tutorialPage [1];
			tutorialButton [1].gameObject.SetActive (true);
			tutorialButton [0].gameObject.SetActive (false);
		}
	}
	IEnumerator spriteClicked(){
		dialougeChanging = true;
		//	Debug.Log ("Sprite clicked");
			charaAnim.SetBool ("Clicked", true);
			int ran = Random.Range (0, dialouge [1].Length);
		StartCoroutine(changeDialouge (1, ran));
			yield return new WaitForSeconds (1f);
			charaAnim.SetBool ("Clicked", false);
			dialougeChanging = false;
	}
	// Update is called once per frame
	void Update () {
	}
	IEnumerator changeDialouge(int x,int y){
		dialougeChanging = true;
		dialougeText.GetComponent<Text> ().text ="";
		dialougeBubble.GetComponent<Animation>().Play();
		yield return new WaitForSeconds (0.8f);
		dialougeText.GetComponent<Text> ().text = dialouge [x] [y];
		yield return new WaitForSeconds (0.2f);
		dialougeChanging = false;
	}
	IEnumerator changeDialouge(string s){
		dialougeChanging = true;
		dialougeText.GetComponent<Text> ().text ="";
		dialougeBubble.GetComponent<Animation>().Play();
		yield return new WaitForSeconds (0.8f);
		dialougeText.GetComponent<Text> ().text = s;
		dialougeChanging = false;
	}
	Card drawCard(){
		Card drawnCard=cards[(numberOfCard--)-1];
		return drawnCard;
	}
	IEnumerator flipCard(int cardIndex){		//Flip card by inputing cardIndex, 0-4
		cardFlipping=true;
		sprite[cardIndex].transform.DOScaleX(0f,0.25f);
		yield return new WaitForSeconds (0.25f);
		sprite [cardIndex].GetComponent<Image> ().sprite = cardContainer [cardIndex].getCardSprite ();
		playClip (2);
		sprite[cardIndex].transform.DOScaleX(0.2f,0.25f);
		flipped [cardIndex] = true;
		yield return new WaitForSeconds (0.25f);
		cardFlipping=false;
	}
	IEnumerator flipAllCardBack(){		//Flip card to back again
		cardFlipping=true;
		for (int i = 0; i < 5; i++) {
			sprite[i].transform.DOScaleX(0f,0.25f);
		}
		yield return new WaitForSeconds (0.25f);
		for (int i = 0; i < 5; i++) {
			sprite [i].GetComponent<Image> ().sprite = backFront;
		}
		for (int i = 0; i < 5; i++) {
			sprite[i].transform.DOScaleX(0.2f,0.25f);
		}
		yield return new WaitForSeconds (1f);
		cardFlipping=false;
	}
	void inisiateRound(){
		for (int i = 0; i < flipped.Length; i++) {//return flipped to false;
			flipped [i] = false;
		}
		if (currentRound+1 < 5) {
			nextRoundBet.text = "$10";
		} else if (currentRound+1 < 8) {
			nextRoundBet.text = "$20";
		} else if(currentRound+1<11){
			nextRoundBet.text = "$40";
		}else{
			nextRoundBet.text="No more bets";
		}
		highLowPanel.SetActive (true);
		continueButton.SetActive (false);
		continueRound = false;	//change function when continue button pressed, false as long round is playing
		trueOrFalse.text = "";
		winning = 0;
		winningText.text = "$0";
		roundNumber.text = "Round "+currentRound.ToString();
		currentPotText.text = "Bet: $" + currentPot.ToString();
		StartCoroutine (flipAllCardBack ());
		for (int i = 0; i < 5; i++) {
			cardContainer [i] = drawCard ();
		}
		playerBalance -= currentPot;
		playerBalanceText.text = "$" + playerBalance.ToString();
		StartCoroutine (flipCard (0));
		phase = 1;
		inisiatePhase (phase);
	}
	void inisiatePhase(int phase){
		trueOrFalse.text = "Higher or Lower?";
		highLowPanel.SetActive (true);
		continueButton.SetActive (false);
	}
	public void pressedHighButton(){
		if (!cardFlipping) {	//dont do anything if card is flipping
			StartCoroutine (highLowPressed (true));
		}
	}
	public void pressedLowButton(){
		if (!cardFlipping) {//dont do anything if card is flipping
			StartCoroutine (highLowPressed (false));
		}
	}
	IEnumerator highLowPressed(bool x){
		StartCoroutine (flipCard (phase));
		continuePhase = false;
		yield return new WaitForSeconds (0.5f);
		if (x) {
			trueOrFalse.text= (checkHighLow (true, phase)).ToString();
			//continuePanel.SetActive (true);
			//selectionPanel.SetActive (false);
			if (checkHighLow (true, phase)) {

				if (phase == 1) {
					winning = currentPot / 2;
				} else {
					winning += winning;
				}
				//playerPoints[i] += 1;
				//pointsText [i].text = playerPoints [i].ToString();
				phase++;
				winningText.text ="$"+ winning.ToString ();
				continuePhase = true;
				highLowPanel.SetActive (false);
				continueButton.SetActive (true);
				playClip (0);
		//		Debug.Log("Coro used");
				StartCoroutine(changeDialouge (2, Random.Range (0, dialouge [2].Length)));
				
				if (phase == 5) {
					continueRound = true;
					continuePhase = false;
				}
			} else {
				playClip (1);
				StartCoroutine (changeDialouge (3, Random.Range (0, dialouge [3].Length)));
				highLowPanel.SetActive (false);
				continueButton.SetActive (true);
				continueRound = true;
			}
		} else {
			trueOrFalse.text= (checkHighLow (false, phase)).ToString();
			//continuePanel.SetActive (true);
			//selectionPanel.SetActive (false);
			if (checkHighLow (false, phase)) {
				if (phase == 1) {
					winning = currentPot / 2;
				} else {
					winning += winning;
				}
				//playerPoints[i] += 1;
				//pointsText [i].text = playerPoints [i].ToString();
				phase++;
				winningText.text ="$"+ winning.ToString ();
				continuePhase = true;
				highLowPanel.SetActive (false);
				continueButton.SetActive (true);
				playClip (0);
//					Debug.Log("Coro used");
					StartCoroutine(changeDialouge (2, Random.Range (0, dialouge [2].Length)));
				if (phase == 5) {
					continueRound = true;
					continuePhase = false;
				}
				//playerPoints[i] += 1;
				//pointsText [i].text = playerPoints [i].ToString();
			} else{
				playClip (1);
				StartCoroutine (changeDialouge (3, Random.Range (0, dialouge [3].Length)));
				highLowPanel.SetActive (false);
				continueButton.SetActive (true);
				continueRound = true;
			}
		}
	}
	void playClip(int clipIndex){
		audiosource [0].clip = soundEffect [clipIndex];
		audiosource [0].Play ();
	}
	IEnumerator playClipDelayed(int i,float delay){
		yield return new WaitForSeconds (delay);
		playClip (i);
	}
	bool checkHighLow(bool high,int index){
		//if true,high. false, low
		//index>0
		if (high) {
			return cardContainer [index-1].getValue () <= cardContainer [index].getValue ();
		} else {
			return cardContainer [index-1].getValue () >= cardContainer [index].getValue ();
		}
	}
	void shuffleCard(){
		Card temp;
		int random;
		for (int i = 0; i < cards.Length; i++) {
			temp = cards [i];
			random = Random.Range (0, cards.Length);
			cards[i]=cards[random];
			cards [random] = temp;
		}

	}
	IEnumerator endRound(){
		//flip all unflipped card
		continueButton.GetComponent<Button>().interactable=false;
		for (int i = 0; i < 5; i++) {
			if (!flipped [i]) {
				StartCoroutine (flipCard (i));
			}
		}
		continuePhase = true;
		StartCoroutine (playClipDelayed (3, 0.4f));
		roundNumber.text = "";
		currentPotText.text = "";
		yield return new WaitForSeconds (0.5f);
		trueOrFalse.text = "You get $" + winning.ToString ();
		playerBalance += winning;
		playerBalanceText.text = "$" + playerBalance.ToString ();
		winningText.text = "$" + 0.ToString ();
		++currentRound;
		if (currentRound < 5) {
			currentPot = 10;
		} else if (currentRound < 8) {
			currentPot = 20;
		} else {
			currentPot = 40;
		}
		roundNumber.text = "";

		//currentPotText.text = "Bet for round "+(currentRound).ToString()+"\n$" + currentPot.ToString();
		if (currentRound == 11) {
			currentPotText.text = "FINISH";
			gamePlaying = false;
		} else if (playerBalance<currentPot){
			gamePlaying = false;
			busted = true;
			trueOrFalse.text = "BUSTED";
		}
		continueButton.GetComponent<Button>().interactable=true;
	}
	void endGame(){
		if (PlayerPrefsX.GetBool ("demoMode")) {
			if (busted) {
				PlayerPrefs.SetString ("dialougeFile", "demoMode");
				PlayerPrefs.SetInt ("lineCounter", 187);
			} else {
				PlayerPrefs.SetString ("dialougeFile", "demoMode");
				PlayerPrefs.SetInt ("lineCounter", 182);
			}
			StartCoroutine (sceneManager.changeSceneWithLoadingEnum (2));
		} else {
			pointTemp = PlayerPrefsX.GetIntArray ("HnLPoint");
			nameTemp = PlayerPrefsX.GetStringArray ("HnLName");
			for (int i = 0; i < 5; i++) {
				PointText [i].text = "$" + pointTemp [i].ToString ();
				Name [i].text = nameTemp [i];
			}
			mainPanel.SetActive (false);
			highLowPanel.SetActive (false);
			continueButton.SetActive (false);
			endGamePanel.SetActive (true);
			charaSprite.GetComponent<Button> ().interactable = false;
			//endButton.interactable = false;
			if (!busted) {
				int finalBalance = playerBalance;
				finalScore.text = "$" + finalBalance.ToString ();
				if (finalBalance < 100) {
					StartCoroutine (changeDialouge ("Not sure you're unlucky or just plain bad. Whatever."));
				} else if (finalBalance < 300) {
					StartCoroutine (changeDialouge ("You did okay. Looking forward to our next meeting."));
				} else if (finalBalance < 600) {
					StartCoroutine (changeDialouge ("That was great. Let's play again sometimes"));	
				} else {
					StartCoroutine (changeDialouge ("I'm geniunely impressed. We should go hang out somewhere."));
				}
				if (PlayerPrefsX.GetBool ("demoMode", true)) {
					PlayerPrefs.SetString ("dialougeFile", "demoMode");
					PlayerPrefs.SetInt ("lineCounter", 182);
				} else {
					PlayerPrefs.SetString ("dialougeFile", "mizuoIntro");
					PlayerPrefs.SetInt ("lineCounter", 154);
				}
				endButton.gameObject.SetActive (false);
				checkHighScore ();
			} else {
				finalScore.text = "BUSTED";
				StartCoroutine (changeDialouge ("You've lost. Better luck next time"));

				if (PlayerPrefsX.GetBool ("demoMode", true)) {
					PlayerPrefs.SetString ("dialougeFile", "demoMode");
					PlayerPrefs.SetInt ("lineCounter", 187);
				} else {
					PlayerPrefs.SetString ("dialougeFile", "mizuoIntro");
					PlayerPrefs.SetInt ("lineCounter", 159);
				}
				textInput.SetActive (false);
				endButton.gameObject.SetActive (true);
			}
			StartCoroutine (turnOnEndButtonIn (1f));
		}
	}
	void checkHighScore(){
		bool change = false;
		for (int i = 0; i < 5; i++) {
			if (playerBalance > pointTemp [i]) {
				//mainText.text = "NEW HIGH SCORE";
				change = true;
				changedRanking = i;
				//Name [i].text = "YOU";
				for (int j = 4; j > i; j--) {
					nameTemp [j] = nameTemp [j - 1];
					pointTemp [j] = pointTemp [j - 1];
				}
				nameTemp [i] = "Very Generic Name";
				pointTemp [i] = playerBalance;
				PointText [i].text = "$"+pointTemp[i].ToString ();
				break;
			}
			change = false;
		}
		if (change) {
			textInput.gameObject.SetActive (true);
			endButton.gameObject.SetActive (false);
			bool[] temp = PlayerPrefsX.GetBoolArray ("RewardArray");
			if (PlayerPrefsX.GetBool ("freeMode", true)) {
				temp [4] = true;
				PlayerPrefsX.SetBoolArray ("RewardArray", temp);
			} else {
			}
		} else {
			endButton.gameObject.SetActive (true);
			textInput.gameObject.SetActive (false);
		}
	}
	public void nameInput(){
		mainText.gameObject.SetActive (false);
		highScoreTable.SetActive (true);
		finalScore.gameObject.SetActive (false);
		nameTemp [changedRanking] = textInput.GetComponent<InputField> ().text;
		Name [changedRanking].text = nameTemp [changedRanking];
		PlayerPrefsX.SetIntArray ("HnLPoint", pointTemp);
		PlayerPrefsX.SetStringArray ("HnLName", nameTemp);
		for (int i = 0; i < 5; i++) {
			PointText [i].text = "$"+pointTemp [i].ToString();
			Name [i].text = nameTemp [i];
		}
		textInput.gameObject.SetActive (false);
		endButton.gameObject.SetActive (true);

	}
	IEnumerator turnOnEndButtonIn(float delay){
		endButton.interactable = false;
		yield return new WaitForSeconds (delay);
		endButton.interactable = true;
	}
	public void pressedExitButton(){
		endButton.interactable = false;
		if (PlayerPrefsX.GetBool ("freeMode", true)) {
			if(PlayerPrefsX.GetBool("MizuoMissionStart")){
				bool[]temp=PlayerPrefsX.GetBoolArray("MizuoChapter");
				temp[2]=true;
				PlayerPrefsX.SetBoolArray("MizuoChapter",temp);
			}

		}
		if (!PlayerPrefsX.GetBool ("freeMode", false)|
			PlayerPrefsX.GetBool ("demoMode", false)) {
			StartCoroutine (sceneManager.changeSceneWithLoadingEnum (2));
		} else {
			PlayerPrefsX.SetBool ("freeMode", true);
			StartCoroutine (sceneManager.changeSceneWithLoadingEnum (1));
		}
	}
	public void continueButtonPressed(){
		if (!dialougeChanging) {		//Button does nothing when dialouge is moving
			if (!continuePhase) {	//if player guessed wrong or guessed all right
				if (!cardFlipping) {
					StartCoroutine (endRound ());
				}
			} else if (!gamePlaying) {	//end game when round 10 has been played
				endGame ();
			} else if (continueRound) {			//Start new Round
				inisiateRound ();
			} else {						//if player guessed right, and there is still unguessed card
				inisiatePhase (phase);
			}
		}
	}
}
