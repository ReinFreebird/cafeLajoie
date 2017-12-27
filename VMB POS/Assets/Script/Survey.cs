using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Survey : MonoBehaviour {

	// Use this for initialization
	public void connectToForm(){
		this.GetComponent<AudioSource> ().Play ();
		Application.OpenURL ("https://goo.gl/forms/jdG3D2XJIAx2kHsm1");
	}
	
	// Update is called once per frame
	public void toSceneOne () {
		SceneManagerClass.changeScene (1);
	}
}
