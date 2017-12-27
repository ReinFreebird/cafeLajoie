using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class touchtest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetTouch (0).phase == TouchPhase.Began) {
			Debug.Log ("I");

		}
		if (Input.GetTouch (0).phase == TouchPhase.Moved) {
			Debug.Log ("Love");

		}
			if (Input.GetTouch (0).phase == TouchPhase.Ended) {
				Debug.Log ("You");
			
			}
	}


}

