using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using LitJson;
public class Dialouge:MonoBehaviour{
	string fileName;
	JsonData jSon;
	public string jsonString;
	List<dialougeLine> lines = new List<dialougeLine> ();
	List<dialougeLineJson> linesJson = new List<dialougeLineJson> ();
	//list to contain strings. Will this be necessary?
	List<Character> charaList=new List<Character>();//file value index 0
	List<string> dialougeList=new List<string>();//file value index 1
	List<int> spriteList = new List<int> ();//file value index 2
	Sprite[][]  charaSprite;//[x][y] x = chara index(equals to list array), y=sprite index
	//for android use
	List<string>charaIDContainer= new List<string>();
	WWW url;
	public Dialouge(string fileName){
		//Debug.Log ("Dialouge called");
		//line is used to contain string from file
		string line = "";
		string path;
		string charaID;
		if(Application.platform == RuntimePlatform.Android){// if running in android
			path = "jar:file://" + Application.dataPath + "!/assets/Dialouge/";
			this.fileName = path+""+fileName+".json";
			url = new WWW (this.fileName);
			while (!url.isDone) {

			}
			jsonString = url.text;
			jSon = JsonMapper.ToObject (jsonString);
		//	reader = null;
			for (int i = 0; i < jSon.Count; i++) {
				linesJson.Add(new dialougeLineJson(linesJson.Count+1,jSon[i]["id"].ToString(),jSon[i]["line"].ToString(),jSon[i]["parameter"].ToString()));
				if (!linesJson [i].functionTrue) {//is not function
					if (!(linesJson [i].id == "NULL")) {	//is not narator speaking
						if (!checkCharaList (linesJson [i].id)) {//checking if chara is not in list. if true make new chara
							//charaList.Add (new Character (linesJson [i].id));	//Add character to charaList
							//charaIDContainer.Add (linesJson [i].id);
							//Debug.Log ("Chara added " + linesJson [i].id);
						} else {
						//do nothing
						}
					} else {
						//do nothing
					}
				} else {
					//do nothing
				}

			}
			//url.Dispose ();
			//loadSprites ();
		}else{
			path = Application.dataPath + "/StreamingAssets/Dialouge/";
			this.fileName = path+""+fileName+".txt";
			StreamReader r = new StreamReader (this.fileName);
			using (r) {
				line = r.ReadLine ();
				do {
					//Chara Index#Line#SpriteIndex
					string[] file_value = line.Split ('#');
					for (int i = 0; i < file_value.Length; i++) {

						//Check line contain
						//Debug.Log(file_value[i]);
					}

					//dialougeList.Add(file_value[1]);
					//spriteList.Add(int.Parse(file_value[2]));
					// check the value of file_value[0]
					//	Debug.Log (file_value [0]);
					if (file_value [0] == "F"||file_value [0] == "CC"||file_value [0] == "CI"||file_value [0] == "CS"||file_value [0] == "CB"||file_value [0] == "CCG") {//Line is function or changePref
						Debug.Log(file_value[1]);
						lines.Add (new dialougeLine (file_value [0],file_value [1],file_value [2],true));
						linesJson.Add(new dialougeLineJson(linesJson.Count+1,file_value[0],file_value [1],file_value [2]));
					} else if (file_value[0]=="S"){	//Line is selection
						//			Debug.Log (file_value[2]);
						lines.Add(new selectionLine(file_value[1],file_value[2]));
						linesJson.Add(new dialougeLineJson(linesJson.Count+1,file_value[0],file_value [1], file_value [2]));
					}else if (file_value [0] == "NULL") {	//No chara is speaking, narator
						//		Debug.Log ("process went inside N");
						//-1 means no sprite
						lines.Add (new dialougeLine ("", file_value [1], file_value [2]));
						linesJson.Add(new dialougeLineJson(linesJson.Count+1,file_value[0],file_value [1], file_value [2]));
					} else {
						//Debuging value of checkCharaList
						//		Debug.Log (checkCharaList (file_value [0]));
						if (!checkCharaList (file_value [0])) {//checking if chara is not in list. if true make new chara
							charaList.Add (new Character (file_value [0]));
						}
						Debug.Log(lines.Count);
						lines.Add (new dialougeLine (charaList [getCharaIndex (file_value [0])].getCharaName (), file_value [1], file_value [2]));
						linesJson.Add(new dialougeLineJson(linesJson.Count+1,charaList[getCharaIndex(file_value[0])].getCharaName(),file_value[1],file_value[2]));
					
					}
					line = r.ReadLine ();
				} while(line != null);
			}
			//Instansiate Sprite charaSprite array


				//Check charasprite[i]Length
				//	Debug.Log (charaSprite [i].Length);
				//	for (int j = 0; j < charaSprite [i].Length; j++) {
				//		Debug.Log (charaSprite [i] [j].ToString ());
				//	}
			}
			if (Application.platform == RuntimePlatform.Android) {
		
			}else{
			charaSprite = new Sprite[charaList.Count][];
		//Insert sprites into Dialouge charaSprite
			for (int i = 0; i < charaList.Count; i++) {
				charaSprite [i] = charaList [i].getSpriteArray ();
			
			}//File Format
			//charaID//"Line""//Sprite

			//Convert txt into json
			//jSon = JsonMapper.ToJson (linesJson);
			//Debug.Log (jSon);
			//File.WriteAllText (Application.dataPath + "/"+fileName+".json", jSon.ToString ());
			}
		}
	public string getFileName(){
		return fileName;
	}
	public List<string> getStringList(){
		return dialougeList;
	}
	public List<dialougeLine> getDialougeLine(){
		return lines;
	}
	public List<dialougeLineJson> getDialougeLineJson(){
		return linesJson;
	}
	public Sprite[][] getCharaSprite(){
		return charaSprite;
	}
	bool checkCharaList(string charID){
		//Check charaList for  charID. If list[0] is null, return false automatically
		if (charaList.Count==0) {
			return false;
		}
		for (int i = 0; i < charaList.Count; i++) {
			if (charID == charaList [0].getCharaID()) {
				return true;
			}
		}
		return false;
	}
	int getCharaIndex(string charID){
		for (int i = 0; i < charaList.Count; i++) {
			if (charID == charaList [i].getCharaID()) {
				return i;
			}
		}
		//if no match, return -1
		return -1;
	}
	public int getCharaNameIndex(string charName){
		for (int i = 0; i < charaList.Count; i++) {
			if (charName == charaList [i].getCharaName()) {
				return i;
			}
		}
		//if no match, return -1
		return -1;
	}
	void loadSprites(){
		string pathCharaJSon = "jar:file://" + Application.dataPath + "!/assets/Character/";
		string jsonFile;
		string pathCharaSprite;
		string spriteFile;
		JsonData jsonStringChara;
		Sprite [][] spriteContainer =new Sprite[charaIDContainer.Count][];
		Sprite[] singleContainer;
		for (int i = 0; i < charaIDContainer.Count; i++) {			
			jsonFile = pathCharaJSon+"chara"+charaIDContainer[i]+".json";//charaIDContainer[0]+".json";
			url = new WWW (jsonFile);
			while (!url.isDone){
			}
			jsonString = url.text;
			jsonStringChara = JsonMapper.ToObject (jsonString);
			if (jsonStringChara.Keys.Count == 0) {		//if json is not read
			}
			singleContainer = new Sprite[jsonStringChara ["sprites"].Count];
			pathCharaSprite = "jar:file://" + Application.dataPath + "!/assets/Character/" + jsonStringChara ["id"].ToString () + "/";
			url.Dispose ();
			for (int j = 0; j < jsonStringChara["sprites"].Count; j++) {
				spriteFile = pathCharaSprite + jsonStringChara ["sprites"] [j].ToString ()+".png";
				url = new WWW (spriteFile);
				while (!url.isDone) {
				}
				Texture2D temp = new Texture2D(1,1);
				url.LoadImageIntoTexture (temp);
				singleContainer[j]= Sprite.Create (temp, new Rect(0,0,temp.width,temp.height), Vector2.one/2);
				url.Dispose ();
			}
			spriteContainer [i] = singleContainer;
		}
		charaSprite = spriteContainer;
	}
}