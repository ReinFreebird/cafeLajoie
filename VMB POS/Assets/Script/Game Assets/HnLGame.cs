using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HnLGame : MonoBehaviour {
	//GameObject in canvas
	Image [] sprite;// 0= main card, else is player
	public GameObject mainCard;
	public GameObject[] playerCard;
	public GameObject continuePanel;
	public GameObject selectionPanel;
	public GameObject exitPanel;
	public Sprite[] cardFront;
	public Sprite backFront;
	public Text[] editableText;
	public Text[] pointsText;

	//Other Game Object
	List<AudioSource> audiosource=new List<AudioSource>();
	public AudioClip [] soundEffect;
	public AudioClip BGM;
	//Variables
	int numberOfCards=52;
	int roundNumber;
	int[] playerPoints;
	Card[] cards= new Card[52];	//Contain cards in deck
	Card[] cardContainer;		//Contain cards in round
	bool gameIsActive;
	//public int cardIndex; //is it used?

	void Awake(){
		sprite = new Image[playerCard.Length + 1];
		sprite [0] = mainCard.GetComponent<Image> ();
		for (int i = 1; i < playerCard.Length+1; i++) {
			sprite [i] = playerCard[i-1].GetComponent<Image> ();	
		}
		cardContainer = new Card[playerCard.Length + 1];
		playerPoints = new int[playerCard.Length + 1];
		gameIsActive = true;
		editableText [3].gameObject.SetActive (false);
		exitPanel.gameObject.SetActive (false);
	}
	void Start(){
		sprite [0].sprite = backFront;
		sprite [1].sprite = backFront;
		sprite [2].sprite = backFront;
		int x=0;
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 13; j++) {
				cards [x] = new Card (cardFront[x], j, i);
				x++;
			}
		}
		shuffleCard ();
		inisializeGameDebug2 ();
	}
	void spriteChange(int RendererAndCard){
		sprite [RendererAndCard].sprite = cardContainer [RendererAndCard].getCardSprite();

	}
	void shuffleCard(){
		Card temp;
		int random;
		for (int i = 0; i < numberOfCards; i++) {
			temp = cards [i];
			random = Random.Range (0, numberOfCards);
			cards[i]=cards[random];
			cards [random] = temp;
		}

	}
	Card drawCard(){
		Card drawnCard = cards [(numberOfCards--)-1];
		return drawnCard;
	}
	void refreshCard(){
		numberOfCards = 52;
	}
	public void inisializeGameDebug1(){
		cardContainer[0] = drawCard ();
		spriteChange (0);
		cardContainer[1] = drawCard ();
		spriteChange (1);
		cardContainer[2] = drawCard ();
		spriteChange (2);
		editableText [0].text = numberOfCards.ToString();
		editableText [1].text = (cardContainer [0].getValue () >= cardContainer [1].getValue ()).ToString();
		editableText [2].text = (cardContainer [0].getValue () >= cardContainer [2].getValue ()).ToString();
	}
	public void inisializeGameDebug2(){
		if (roundNumber < 17) {
			roundNumber++;
			continuePanel.SetActive (false);
			selectionPanel.SetActive (true);
			hideFront (0);
			hideFront (1);
			hideFront (2);
			cardContainer [0] = drawCard ();
			spriteChange (0);
			cardContainer [1] = drawCard ();
			cardContainer [2] = drawCard ();
			editableText [0].text = "Round "+ roundNumber.ToString ();
		} else {
			checkWin ();
		}
	}
	public void hideFront(int RendererAndCard){
		sprite [RendererAndCard].sprite=backFront;

	}
	public void checkWin(){
		mainCard.gameObject.SetActive (false);
		for (int i = 0; i < 3; i++) {
			if (i < 2) {
				playerCard [i].gameObject.SetActive (false);
			}
			editableText [i].gameObject.SetActive (false);
		}
		playerCard [0].gameObject.SetActive (false);
		playerCard [1].gameObject.SetActive (false);
		selectionPanel.gameObject.SetActive (false);
		continuePanel.gameObject.SetActive (false);
		if (playerPoints [1] == playerPoints [2]) {
			editableText [3].text = "DRAW";
		} else if (playerPoints [1] > playerPoints [2]) {
			editableText [3].text = "YOU WON";
		} else {
			editableText [3].text = "MIZUO WON";
		}
		editableText [3].gameObject.SetActive (true);
		exitPanel.gameObject.SetActive (true);
	}
	public void pressedHighButton(int i){
		editableText[i].text= (checkHighLow (true, i)).ToString();
		spriteChange (i);
		continuePanel.SetActive (true);
		selectionPanel.SetActive (false);
		if (checkHighLow (true, i)) {
			playerPoints[i] += 1;
			pointsText [i].text = playerPoints [i].ToString();
		}
		if (i == 1) {
			opponentAIMedium();
		}
	}
	public void pressedLowButton(int i){
		editableText[i].text= (checkHighLow (false, i)).ToString();
		spriteChange (i);
		continuePanel.SetActive (true);
		selectionPanel.SetActive (false);
		if (checkHighLow (false, i)) {
			playerPoints[i] += 1;
			pointsText [i].text = playerPoints [i].ToString();
		}
		if (i == 1) {
			opponentAIMedium();
		}
	}
	bool checkHighLow(bool high,int index){
		//if true,high. false, low
		//index>0
		if (high) {
			return cardContainer [0].getValue () <= cardContainer [index].getValue ();
		} else {
			return cardContainer [0].getValue () >= cardContainer [index].getValue ();
		}
	}
	void opponentAIHard(){
		//Hard (83,3% right)
		int keyValue = cardContainer [0].getValue ();
		bool mainChoice = cardContainer [2].getValue () <= cardContainer [0].getValue ();
		bool[] choice = new bool[12];
		int random = Random.Range (0, 11);
		if (mainChoice) {
			if (cardContainer [0].getValue () == 0 || cardContainer [0].getValue () == 12) {
				pressedLowButton (2);
			} else {
				choice [0] = false;
				choice [11] = false;
				for (int i = 1; i < 11; i++) {
					choice [i] = true;
				}
				if (choice [random]) {
					pressedLowButton (2);
				} else {
					pressedHighButton (2);
				}
			}
		} else {
			if (cardContainer [0].getValue () == 0 || cardContainer [0].getValue () == 12) {
				pressedHighButton (2);
			} else {
				choice [0] = true;
				choice [11] = true;
				for (int i = 1; i < 11; i++) {
					choice [i] = false;
				}
				if (choice [random]) {
					pressedLowButton (2);
				} else {
					pressedHighButton (2);
				}
			}
		
		}
	}
	void opponentAIMedium(){
		//Medium
		int keyValue = cardContainer [0].getValue ();
		int higherRange = (12 - keyValue) * 4;
		int random = Random.Range (0, 47);
		bool[] choice = new bool[48];

			for (int i = 0; i < 48; i++) {
				choice [i] = false;
			}
			for (int i = 0; i < higherRange-1;) {
				do {
					random = Random.Range (0, 47);	
				} while(choice [random]);
				choice [random] = true;
				i++;
			}
			random = Random.Range (0, 47);
			if (choice [random]) {
				pressedHighButton (2);
			} else {
				pressedLowButton (2);
			}
		

	}
	void opponentAIEasy(){
		int random = Random.Range (0, 100);
		if (random%2==0) {
			pressedHighButton (2);
		} else {
			pressedLowButton (2);
		}
	}
	public void pressedExitButton(){
		refreshCard ();
		SceneManagerClass.changeScene (0);
	
	}

}

