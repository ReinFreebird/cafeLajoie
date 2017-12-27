using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card {
	Sprite cardSprite;
	int value; //2=0,3=1,4=2,5=3,6=4,7=5,8=6,9=7,10=8,J=9,Q=10,K=11,A=12;
	int suit; //Spade=0,Club=1,Diamond=2,Heart=3;
	public Card(){
	
	}
	public Card(Sprite sprite,int value,int suit){
		cardSprite = sprite;
		this.value = value;
		this.suit = suit;
	}
	public Sprite getCardSprite(){
		return cardSprite;
	}
	public int getValue(){
		return value;
	}
	void showCardStat(){
		
	}
}