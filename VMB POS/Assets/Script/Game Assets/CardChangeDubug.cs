using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardChangeDubug : MonoBehaviour {
	HnLGame cardModel;
	int cardIndex=0;
	public GameObject card;
	public Button btn;
	void Awake(){
		cardModel = card.GetComponent<HnLGame>();
	}
//	public void changeCard(){
//		if (cardIndex > 51) {
//			cardIndex = 0;
//			cardModel.toggleFront (false);
	//
	//	} else {
	//		cardModel.cardIndex = cardIndex;
	//		cardModel.toggleFront (true);
	//		cardIndex++;
	//	}
	//
		

	//}
}
