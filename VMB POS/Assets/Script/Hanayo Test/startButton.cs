using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class startButton : MonoBehaviour {

	public AudioSource startAudioSource;
	public AudioClip[] startClip;
	public string[] dialouge;
	public Text dialougeUI; 
	int clipIndex;
	// Use this for initialization
	void Start () {
		
	}

	public void startPressed(){
		clipIndex = Random.Range (0, startClip.Length - 1);
		startAudioSource.clip = startClip[clipIndex];
		dialougeUI.text = dialouge [clipIndex];
		startAudioSource.Play ();
	}
}
