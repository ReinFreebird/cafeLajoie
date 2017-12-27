using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;
using UnityEngine.UI;
public class dialougeLineJsonTester : MonoBehaviour {
	private string jsonString;
	private JsonData itemData;
	public  GameObject picture;
	public Text textTest;
	Sprite sprite;
	string path;
	WWW url;
	// Use this for initialization
	void Start () {
		jsonString = File.ReadAllText (Application.dataPath + "/StreamingAssets/Character/charaMiyuki.json");
		itemData = JsonMapper.ToObject (jsonString);

		textTest.text =itemData.Keys.Count.ToString();//itemData ["id"].ToString()+" "+itemData ["name"].ToString()+" "+itemData ["sprites"].Count;
		path ="http://pre09.deviantart.net/2d12/th/pre/i/2014/311/d/e/_lovelive_koizumi_hanayo_png_ver__by_sr_png-d85nio4.png";
		StartCoroutine (downloadImage (path));
		Texture2D temp = new Texture2D(1,1);
		url.LoadImageIntoTexture (temp);
		sprite = Sprite.Create (temp, new Rect(0,0,temp.width,temp.height), Vector2.one/2);
		//picture.GetComponent<Image>().sprite = Sprite.Create (temp, new Rect (), new Vector2 ());
		picture.GetComponent<Image>().sprite=sprite;
	}
	IEnumerator downloadImage(string s){
		url = new WWW (s);
		while (!url.isDone) {
		
		}
		yield return url;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	//public void inb(){
	//	picture.GetComponent<Animation> ().Play ("spriteFadeIn");
	//}

	//public void outb(){
	//	picture.GetComponent<Animation> ().Play ("spriteFadeOut");
	//}
}
