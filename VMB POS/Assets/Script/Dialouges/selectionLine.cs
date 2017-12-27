using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selectionLine : dialougeLine {

	string selectionTag;
	string line;

	public selectionLine(string s,string l){
		selectionTag = s;
		line = l;
	}
	public string getSelectionTag(){
		return selectionTag;
	}
	public string getLineIndex(){
		return line;
	}
}
