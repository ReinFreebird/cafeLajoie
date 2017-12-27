using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

public class HnLGameDebug : MonoBehaviour {
	SpriteRenderer []spriteRenderer;// 0= main card, else is player
	public GameObject mainCard;
	public GameObject[] playerCard;
	public GameObject continuePanel;
	public GameObject selectionPanel;
	public Sprite[] cardFront;
	public Sprite backFront;
	public int cardIndex;
	public Text[] editableText;
	public Text[] pointsText;
	int numberOfCards=52;
	int[] playerPoints;
	Card[] cards= new Card[52];
	Card[] cardContainer; //Type  To be changed


	void Awake(){
		spriteRenderer = new SpriteRenderer[playerCard.Length + 1];
		spriteRenderer [0] = mainCard.GetComponent<SpriteRenderer> ();
		for (int i = 1; i < playerCard.Length+1; i++) {
			spriteRenderer [i] = playerCard[i-1].GetComponent<SpriteRenderer> ();	
		}
		cardContainer = new Card[playerCard.Length + 1];
		playerPoints = new int[playerCard.Length + 1];

	}
	void Start(){
		spriteRenderer [0].sprite = backFront;
		spriteRenderer [1].sprite = backFront;
		spriteRenderer [2].sprite = backFront;
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
			spriteRenderer [RendererAndCard].sprite = cardContainer [RendererAndCard].getCardSprite();
	
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
		continuePanel.SetActive (false);
		selectionPanel.SetActive (true);
		hideFront (0);
		hideFront (1);
		cardContainer[0] = drawCard ();
		spriteChange (0);
		cardContainer[1] = drawCard ();
		editableText [0].text = numberOfCards.ToString();
	}
	public void hideFront(int RendererAndCard){
			spriteRenderer [RendererAndCard].sprite=backFront;

	}
	public void chooseHighLow(int playerIndex, bool high){
		//if true,high. false, low
		//index>0
	
	}
	public void pressedHighButton(){
		editableText[1].text= (checkHighLow (true, 1)).ToString();
		spriteChange (1);
		continuePanel.SetActive (true);
		selectionPanel.SetActive (false);
		if (checkHighLow (true, 1)) {
			playerPoints[1] += 1;
			pointsText [1].text = playerPoints [1].ToString();
		}
	}
	public void pressedLowButton(){
		editableText[1].text= (checkHighLow (false, 1)).ToString();
		spriteChange (1);
		continuePanel.SetActive (true);
		selectionPanel.SetActive (false);
		if (checkHighLow (false, 1)) {
			playerPoints[1] += 1;
			pointsText [1].text = playerPoints [1].ToString();
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

}