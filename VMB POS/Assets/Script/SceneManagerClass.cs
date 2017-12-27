using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
public class SceneManagerClass:MonoBehaviour {

	public static void changeScene (int i){
		SceneManager.LoadSceneAsync (i);
	}
	public static void fadeToBlack(GameObject gameO){
		gameO.SetActive (true);
		gameO.GetComponent<Animation> ().Play ("fadeSceneToBlack");
	}
	public static void fadeToWhite(GameObject gameO){
		gameO.SetActive (true);
		gameO.GetComponent<Animation> ().Play ("fadeSceneToWhite");
	}
	public static void fadeBlackToNull(GameObject gameO){
		gameO.SetActive (true);
		gameO.GetComponent<Animation> ().Play ("fadeSceneBlackToNull");
		gameO.SetActive (false);
	}
	public static void fadeWhiteToNull(GameObject gameO){
		gameO.GetComponent<Animation> ().Play ("fadeSceneWhiteToNull");
		gameO.SetActive (false);
	}
	IEnumerator fadeScene(GameObject gameO, string s){
		gameO.GetComponent<Animation> ().Play ("s");
		yield return null;
	}
	public static void conversationSetGamePref(string fileName,int lineCounter){
		PlayerPrefs.SetString ("dialougeFile", fileName);
		PlayerPrefs.SetInt ("lineCounter", lineCounter);
	}


}
