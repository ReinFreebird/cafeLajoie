using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//dialougeLine contain string written in dialouge file
public class dialougeLine{
	public string charaName=" ";
	public string wordLine=" ";
	public string lineType=" ";//added to have changeprefFunction
	public string functionName=" ";
	public string integerParameter="0";
	public string spriteIndex="0";
	public bool functionTrue=false;		//is this line a function?

	public dialougeLine(){
	}
	public dialougeLine(string chara,string line,string function,string para,string index,bool ftrue){
		charaName = chara;
		wordLine = line;
		functionName = function;
		integerParameter = para;
		spriteIndex = index;
		functionTrue = ftrue;
	}
	public dialougeLine (string chara, string word, string index)
	{
		charaName = chara;
		wordLine = word;
		spriteIndex = index;
		functionTrue = false;
	}
	public dialougeLine(string type,string function,string parameter,bool functionIsTrue){
		lineType = type;
		functionName = function;//F function,CS changeprefString,CI changeprefInt,CC changeCharaChapter
		integerParameter = parameter;
		functionTrue = functionIsTrue;
	}
	public string getCharaName(){
		return charaName;
	}
	public string getLine(){
		return wordLine;
	}
	public string getSpriteIndex(){
		return spriteIndex;
	}
	public string getFunctionName(){
		return functionName;
	}
	public string getFunctionParameter(){
		return integerParameter;
	}
	public string getLineType(){
		return lineType;
	}
	public bool isFunction(){
		return functionTrue;
	}
}