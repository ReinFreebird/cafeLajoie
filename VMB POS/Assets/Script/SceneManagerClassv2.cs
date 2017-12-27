using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
public class SceneManagerClassv2:MonoBehaviour {
	public GameObject tintScreen;
	public Text loadingText;
	AsyncOperation loading;
	public void changeSceneWithLoading(int i){
		StartCoroutine (changeSceneWithLoadingEnum (i));
	}
	public IEnumerator changeSceneWithLoadingEnum(int i){
		yield return new WaitForSeconds (0.2f);
		tintScreen.SetActive (true);
		tintScreen.gameObject.GetComponent<Image> ().DOColor (new Color(0,0,0,0.5f),0.75f);
		yield return new WaitForSeconds(1f);
		loadingText.gameObject.SetActive (true);
		loadingText.text = "LOADING...";
		loading = SceneManager.LoadSceneAsync (i);
		loading.allowSceneActivation = false;
		while (!loading.isDone) {
			if (loading.progress == 0.9f) {
				loadingText.text = "Tap to continue";
				if (Input.GetMouseButtonDown(0)) {
					tintScreen.gameObject.GetComponent<Image> ().DOColor (new Color(0,0,0,1f),0.75f);
					yield return new WaitForSeconds (1f);
					loading.allowSceneActivation = true;
				}
			}
			yield return null;
		}
	}
	public void activeTint(){
		tintScreen.SetActive (true);
	}
	public IEnumerator changeSceneWithLoadingEnumNoCon(int i){
		yield return new WaitForSeconds (0.2f);
		tintScreen.SetActive (true);
		tintScreen.gameObject.GetComponent<Image> ().DOColor (new Color(0,0,0,0.5f),0.75f);
		yield return new WaitForSeconds(1f);
		loadingText.gameObject.SetActive (true);
		loadingText.text = "LOADING...";
		loading = SceneManager.LoadSceneAsync (i);
		loading.allowSceneActivation = false;
		while (!loading.isDone) {
			if (loading.progress == 0.9f) {
				tintScreen.gameObject.GetComponent<Image> ().DOColor (new Color(0,0,0,1f),0.75f);
				yield return new WaitForSeconds (1f);
					loading.allowSceneActivation = true;

			}
			yield return null;
		}
	}
	public IEnumerator fadeFromBlack(){
		tintScreen.SetActive (true);
		loadingText.gameObject.SetActive (true);
		tintScreen.gameObject.GetComponent<Image> ().DOColor (new Color(0,0,0,0),1f);
		yield return new WaitForSeconds (1.2f);
		tintScreen.SetActive (false);
	}
}
