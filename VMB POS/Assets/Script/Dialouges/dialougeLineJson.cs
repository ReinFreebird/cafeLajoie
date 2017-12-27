using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dialougeLineJson{
	public int lineIndex;
	public string id;
	public string line;
	public string parameter;
	public bool functionTrue;

	public  dialougeLineJson(){
	
	}
	public dialougeLineJson(int i,string idID,string lineID, string param){
		lineIndex = i;
		id = idID;// F=Function, S= Selection, CS changeprefString,CI changeprefInt,CC changeCharaChapter, else chara
		line = lineID;
		parameter = param;	//for sprite index or other parameter;
		if (id.Length == 1) {	// id ID only
			functionTrue = true;
		} else {
			functionTrue = false;
		}
	}
}
