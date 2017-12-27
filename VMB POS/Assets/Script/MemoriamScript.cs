using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class MemoriamScript : MonoBehaviour {
	public GameObject text1,text2,text3;
	public Button continueBut;
	public SceneManagerClassv2 scene;
	// Use this for initialization
	void Start () {
		text1.SetActive (false);
		text2.SetActive (false);
		text3.SetActive (false);
		StartCoroutine (textStart());
	}
	IEnumerator textStart(){
		yield return new WaitForSeconds (1.5f);
		text1.SetActive (true);
		text2.SetActive (true);
		text3.SetActive (true);
		float fade = 0f;
		Color fadeColor;
		while (text1.GetComponent<Text>().color.a<1) {
			fade += 0.1f;
			fadeColor = new Color (1f, 1f, 1f, fade);
			text1.GetComponent<Text> ().color = fadeColor;
			yield return new WaitForSeconds (0.1f);
		
		}
		fade = 0f;
		yield return new WaitForSeconds (2f);
		while (text2.GetComponent<Text>().color.a<1) {
			fade += 0.05f;
			fadeColor = new Color (1f, 1f, 1f, fade);
			text2.GetComponent<Text> ().color = fadeColor;
			yield return new WaitForSeconds (0.1f);

		}
		fade = 0f;
		yield return new WaitForSeconds (3f);
		while (text3.GetComponent<Text>().color.a<1) {
			fade += 0.1f;
			fadeColor = new Color (1f, 1f, 1f, fade);
			text3.GetComponent<Text> ().color = fadeColor;
			yield return new WaitForSeconds (0.1f);

		}
		fade = 0f;
		yield return new WaitForSeconds (2f);
		continueBut.interactable = true;
	}
	IEnumerator endIE(){
		float fade = 1f;
		Color fadeColor;
		while (text1.GetComponent<Text>().color.a>0) {
			fade -= 0.1f;
			fadeColor = new Color (1f, 1f, 1f, fade);
			text1.GetComponent<Text> ().color = fadeColor;
			text2.GetComponent<Text> ().color = fadeColor;
			text3.GetComponent<Text> ().color = fadeColor;
			yield return new WaitForSeconds (0.1f);

		}
		yield return new WaitForSeconds (1.5f);
		StartCoroutine (scene.changeSceneWithLoadingEnumNoCon (1));

	}
	// Update is called once per frame
	void Update () {
	}
	public void endText(){
		continueBut.interactable = false;
		StartCoroutine (endIE ());
	}
}
