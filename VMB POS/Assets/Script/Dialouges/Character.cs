using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using LitJson;
public class Character:MonoBehaviour{
	//Chara files are now in Resources/Character/charaId


	//Image xxx = new Image();
	string charaID="";//charaID= fileDirectory
	string fileName="";//chara is the determined file name for all character
	string name;
	public Sprite[] charaSprite;
	public JsonData jsonString;
	bool useJason=false;		//to try json for debug purpose
	WWW wwwLink;
	public Character(string fileID){
	//	Debug.Log ("Chara called");
		string line="";
		string path1;
		charaID = fileID;
		if (Application.platform == RuntimePlatform.Android) {// if running in android
			path1 = "jar:file://" + Application.dataPath + "!/assets/Character/";
			this.fileName = path1 + "chara" + fileID + ".json";
			WWW charaLine = new WWW (this.fileName);
			while (!charaLine.isDone) {

			}
			string x = charaLine.text;
			jsonString = JsonMapper.ToObject (x);
			charaID = jsonString ["id"].ToString ();
			name = jsonString ["name"].ToString ();
			//string[] sprites = new string[jsonString ["sprites"].Count];
			charaSprite= new Sprite[jsonString ["sprites"].Count];
			Debug.Log ("jsonLine " + charaID + "_" + name+"_"+jsonString ["sprites"].Count);
			for (int i = 0; i < charaSprite.Length; i++) {
				//sprites [i] = jsonString ["sprites"] [i].ToString ();
//				StartCoroutine(downloadImage(i));
			}
		} else {
			if (!useJason) {
				path1 = Application.dataPath + "/StreamingAssets/Character/" ;
				this.fileName = path1 + "chara" + fileID + ".txt";
				StreamReader r = new StreamReader (this.fileName);
				//first Line charaID, no need to change
				line = r.ReadLine ();
				//	Debug.Log (line);
				//second line charaName
				line = r.ReadLine ();
				name = line;
				//	Debug.Log (name);
				//third line sprite file name. format=charaId[indexNum].png . remove bracket
				line = r.ReadLine ();
				string[] sprites = line.Split ('/');

				//	Debug.Log (sprites[0]);
				charaSprite = new Sprite[sprites.Length];
				for (int i = 0; i < charaSprite.Length; i++) {
					if (Application.platform == RuntimePlatform.Android) {// if running in android
					
					} else {
						string spritePath = "CharacterSprites/" + charaID + "/" + charaID + i.ToString ();
						//		Debug.Log (spritePath);
						charaSprite [i] = Resources.Load<Sprite> (spritePath)as Sprite;
					}
				}
				//	
			} else {//use json
				
			}
			//Debug.Log (charaSprite.Length);
		}
	}
	public string getCharaID(){
		return charaID;
	}
	public string getCharaName(){
		return name;
	}
	public Sprite[] getSpriteArray(){
		return charaSprite;
	}

	IEnumerator downloadImage(int x){
		string path2;
		path2 ="jar:file://" + Application.dataPath + "!/assets/Character/" + jsonString ["id"].ToString ()+ "/" + jsonString ["sprites"] [x].ToString ();
		Debug.Log ("downloadpath: " + path2);
		WWW texture=new WWW(path2);
		yield return texture;
		while (!texture.isDone) {
		}

		Texture2D temp = new Texture2D(1,1);
		texture.LoadImageIntoTexture (temp);
		charaSprite [x] = Sprite.Create (temp, new Rect(0,0,temp.width,temp.height), Vector2.one/2);
	}
}
