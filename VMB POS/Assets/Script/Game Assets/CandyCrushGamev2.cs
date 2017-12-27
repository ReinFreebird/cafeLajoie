using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
//public class Tile{
//	public GameObject tileObject;
//	public string type;
//	public Tile(GameObject tileO,string t){
//		tileObject = tileO;
//		type = t;
//	}
//}

public class CandyCrushGamev2 : MonoBehaviour {
	//Tiles game object
	GameObject tile1=null,tile2=null;
	public GameObject []tile;
	List<GameObject> tileBank = new List<GameObject>();
	public GameObject tileContainer;//Contain all tiles clone in grid
	bool tileVisible;
	//GameObject in Canvas
	public GameObject point;
	public GameObject turn;
	public GameObject[] Panels;// idx 0=point, idx 1=grid, idx 2=continue
	Text pointNum;
	Text turnNum;
	public Text resultNum;
	public Button hideShowB,continueB1,continueB2;
	bool endRenew=true;

	//Grid
	static int row =8;
	static int col=8;
	bool renewBoard=false;

	//Other
	static float speed=100f;
	int scale=6;//1-10, 10 is max


	int points,currentTurn;
	public int maxTurns;
	Tile[,] tiles= new Tile[col,row];
	// Use this for initialization

	//other Bools
	void shuffleList(){
		System.Random ran = new System.Random ();
		int r = tileBank.Count;
		while (r > 1) {
			r--;
			int n = ran.Next (r + 1);
			GameObject temp = tileBank [n];
			tileBank [n] = tileBank [r];
			tileBank [r] = temp;
		
		}
	}
	void Start () {
		//Tile visablity
		tileVisible=true;

		//input text component into pointNum and turnNum
		pointNum = point.GetComponent<Text>();
		turnNum = turn.GetComponent<Text>();

		//Assign number of turns;
		changeTurn (maxTurns);

		int numOfCopies =(row*col)/3;
		for (int i = 0; i < numOfCopies; i++) {
			for (int j = 0; j < tile.Length; j++) {
				GameObject o= (GameObject) Instantiate(tile[j],new Vector3(-15,-15,0),tile[j].transform.rotation);
				o.SetActive(false);
				tileBank.Add(o);
			}
		}
		shuffleList();
		for (int r = 0; r < row; r++) {
			for (int c = 0; c < col; c++) {
				// change tile scale
				float x= c*scale/10f;
				float y=r*scale/10f;
				Vector3 tilePos = new Vector3 (x, y,0);
				for (int n = 0; n < tileBank.Count; n++) {
					GameObject o=tileBank[n];
					if(!o.activeSelf){
						o.transform.position=new Vector3(tilePos.x,tilePos.y,0);
						o.SetActive(true);
						tiles[c,r]=new Tile(o,o.name);
						n= tileBank.Count+1;
					}
				}
			}
		}
	}
	// Update is called once per frame
	void Update () {
		//int point for player
		int pointsAdd;
		bool tileMoving;
		//check if first turn. If yes, reset point
		if (currentTurn == maxTurns) {
			resetPoint ();
			tileMoving=false;
		}
		//Unused line

		//Vector3 temp1,temp2;
		//temp1 = transform.localScale;
		//temp1.x =1f;
		//temp1.y =1f;
		//temp2 = transform.localScale;

		//to fix nullReferenceException at checkCol or checkRow
		//Performance ????
		//to be changed
		while (checkNull()&&endRenew) {
			renewGrid ();
		}
		//if currentTurn=0, no checkGrid;
		if (currentTurn > 0) {
			if (Input.GetMouseButtonDown (0)&&!tileMoving) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit2D hit = Physics2D.GetRayIntersection (ray, 1000);
				if (hit) {
					tile1 = hit.collider.gameObject;
					//tile1.transform.localScale = temp1;
				}
			} 
		//if finger up is detected after
		//an initial tile has been selected
			else if (Input.GetMouseButtonUp (0) && tile1&& !tileMoving) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit2D hit = Physics2D.GetRayIntersection (ray,1000);
				if (hit) {
					tile2 = hit.collider.gameObject;
				}
				if (tile1 && tile2&& !tileMoving) {
					//Capture all tile x&y, to turn it to int
					//index 0= tile1 x
					//index 1= tile1 y
					//index 2= tile2 x
					//index 3= tile2 y

					float[] posFloat = new float[4];
					posFloat [0] = tile1.transform.position.x * 10 / scale;
					posFloat [1] = tile1.transform.position.y * 10 / scale;
					posFloat [2] = tile2.transform.position.x * 10 / scale;
					posFloat [3] = tile2.transform.position.y * 10 / scale;
					int[] posInt = new int[4];
					for (int i = 0; i < 4; i++) {
						posInt [i] = (int)Mathf.Round (posFloat [i]);
					}
					int horDis = Mathf.Abs (posInt [0] - posInt [2]);
					int verDis = Mathf.Abs (posInt [1] - posInt [3]);
					if (horDis >= 1 ^ verDis >= 1) {
						//turn passed
						changeTurn (-1);
						//Tile position switch
						Vector3 tempPos = tile1.transform.position;
						tile1.transform.position = tile2.transform.position;
						tile2.transform.position = tempPos;
						//tileMoving=true;
						//StartCoroutine (swapTile(tile1,tile2,tileMoving));
						//tileMoving = false;
						//Tile Datas switch
						Tile tileTemp = tiles [posInt [0], posInt [1]];
						tiles [posInt [0], posInt [1]] = tiles [posInt [2], posInt [3]];
						tiles [posInt [2], posInt [3]] = tileTemp;


						Debug.Log (tile1.transform.position.x + " " + tile1.transform.position.y);
						Debug.Log (tile2.transform.position.x + " " + tile2.transform.position.y);


					} else {
						//temp1.x=scale/10f;
						//temp1.y=scale/10f;
						//tile1.transform.localScale = temp1;
						GetComponent<AudioSource> ().Play ();
					}
				}
			}
			CheckGrid ();
			//checkGridv2 ();
			//Game ends
			if(currentTurn==0){
				//things to do when game ended
				swapButton ();

			}
		} else {
			//update does nothing when game ended
		}

	}
	void CheckGrid(){
		int counter = 1;
		//check in coloumns
		for (int r = 0; r < row; r++) {
			counter = 1;
			for (int c = 1; c < col; c++) {
				if (tiles [c, r] != null && tiles [c - 1, r] != null)
					//if the tile exist
				{
					
					if (tiles [c, r].type == tiles [c - 1, r].type) {
						counter++;
					} else {
						counter = 1;//reset counter
						//if three are found remove them
					}
					if (counter == 3) {
						if (tiles [c, r] != null) {
							tiles [c, r].tileObject.SetActive (false);
						}	
						if (tiles [c - 1, r] != null) {
							tiles [c - 1, r].tileObject.SetActive (false);
						}
						if (tiles [c - 2, r] != null) {
							tiles [c - 2, r].tileObject.SetActive (false);
						}
						tiles [c, r] = null;
						tiles [c - 1, r] = null;
						tiles [c - 2, r] = null;
						renewBoard = true;
					}
				}
			}
			}
		//check row
		for (int c = 0; c < row; c++) {
			counter = 1;
			for (int r = 1; r < col; r++) {
				if (tiles [c, r] != null && tiles [c, r-1] != null)
					//if the tile exist
				{

					if (tiles [c, r].type == tiles [c , r-1].type) {
						counter++;
					} else {
						counter = 1;//reset counter
						//if three are found remove them
					}
					if (counter == 3) {
						if (tiles [c, r] != null) {
							tiles [c, r].tileObject.SetActive (false);
						}	
						if (tiles [c , r-1] != null) {
							tiles [c , r-1].tileObject.SetActive (false);
						}
						if (tiles [c , r-2] != null) {
							tiles [c , r-2].tileObject.SetActive (false);
						}
						tiles [c, r] = null;
						tiles [c, r-1] = null;
						tiles [c, r-2] = null;
						renewBoard = true;
					}
				}
			}
		}
		if (renewBoard) {
			renewGrid();
			renewBoard=false;
		}
	}

	void checkGridv2(){
		//checkedTile = Tiles to be change
		bool [,,] checkedTile =new bool[col,row,2];//3rd index to change col and row
		int[,] matchTilesCol = new int[col, row];
		int[,] matchTilesRow = new int[col, row];
		int addPoints = 0;
		bool renewBoard = false;//true if there is removed tiles
		for (int x = 0; x < 2; x++) {
			for (int i = 0; i < col; i++) {
				for (int j = 0; j < row; j++) {
					checkedTile [j, i,x] = false;
					//x=0 coloum, x=1 row
					if (x == 0) {
						matchTilesCol [j, i] = 0; //Is this necessary?-RF
					}else{
					matchTilesRow [j, i] = 0; //Is this necessary?-RF
					}
				}
			}
		}
		//check grid from bottom left to top right
		for (int x = 0; x < 2; x++) {
			for (int r = 0; r < row; r++) {
				for (int c = 0; c < col; c++) {
					if (checkedTile [c, r,x]) {//checked tile no need to be checked again
						continue;
					} else {
						if (x == 0) {
							matchTilesCol [c, r] = checkCol (c, r);
							//change all Col checkedTile
							for (int i = c; i < c + matchTilesCol [c, r]; i++) {
								checkedTile [i, r,0] = true;
							}
						} else {
							matchTilesRow [c, r] = checkRow (c, r);
							//change all Row checkedTile
							for (int i = r; i < r + matchTilesRow [c, r]; i++) {
								checkedTile [c, i,1] = true;
							}
						}
					}
				}
			}
		}
		//Remove checked tile
		for (int r = 0; r < row; r++) {
			for (int c = 0; c < col; c++) {
				if (checkedTile [c, r,0]) {
					tiles [c, r].tileObject.SetActive (false);
					tiles [c, r] = null;
				}
				if (checkedTile [c, r, 1]) {
					//check if tiles are not nulled;
					if (tiles [c, r] != null) {
						tiles [c, r].tileObject.SetActive (false);
						tiles [c, r] = null;
					}
				}

			}
		}
		//check for matched tile to calculate added points
		for (int r = 0; r < row; r++) {
			for (int c = 0; c < col; c++) {
				//if only match for col XOR row
				if (checkedTile [c, r, 0] ^ checkedTile [c, r, 1]) {
					addPoints += 10;
				}
				//if match for col AND row
				else if (checkedTile [c, r, 0] && checkedTile [c, r, 1]) {
					addPoints += 30;
				}
			}
		}
		//if have addPoints
		if (addPoints > 0) {
			addPoint (addPoints);
		}
		//check for removed Tile
		for (int r = 0; r < row; r++) {
			for (int c = 0; c < col; c++) {
				if (checkedTile [c, r,0]||checkedTile[c,r,1]) {
					renewBoard = true;
					break;
				}
			}
			if (renewBoard) {
				break;
			}
		}
		//if renewBoard=true
		if (renewBoard) {
			renewGrid();
			renewBoard = false;
		}
	}

	//check col for match, returns number of matching tiles. If only two, return 0
	//max match=5
	int checkCol(int indexC,int indexR){
		if (indexC >= 6) {
			//index more than 5, max match is 2
			return 0;
		}else if (tiles [indexC, indexR].type == tiles [indexC + 1, indexR].type&&
			tiles [indexC, indexR].type == tiles [indexC + 2, indexR].type) 
			// Check for three first tiles from the left
			{
				if (indexC <= 3) { //max index for 5 matches
					if (tiles [indexC, indexR].type == tiles [indexC + 3, indexR].type) {
						if (tiles [indexC, indexR].type == tiles [indexC + 4, indexR].type) {
							// 5 Matching tiles
							return 5;
						} else {
							// 4 Matching tiles
							return 4;
						}
					} else {
						// 3 Matching tiles
						return 3;
					}
				} else if (indexC == 4) {//max index for 4 matches
					if (tiles [indexC, indexR].type == tiles [indexC + 3, indexR].type) {
						// 4 Matching tiles
						return 4;
					} else {
						// 3 Matching tiles
						return 3;
					}
				} else {//max index for 3 matches
					//3 Matching tiles;
					return 3;
				}
			} else {
				//Matching tiles less than 3
				return 0;
			}
	}
	//check row for match, returns number of matching tiles. If only two, return 0
	//max match=5
	int checkRow(int indexC,int indexR){
		if (indexR >= 6) {
			//index more than 5, max match is 2
			return 0;
		}else if (tiles [indexC, indexR].type == tiles [indexC, indexR + 1].type&&
			tiles [indexC, indexR].type == tiles [indexC, indexR + 2].type) 
			// Check for three first tiles from the left
		{
			if (indexR <= 3) { //max index for 5 matches
				if (tiles [indexC, indexR].type == tiles [indexC, indexR + 3].type) {
					if (tiles [indexC, indexR].type == tiles [indexC, indexR + 4].type) {
						// 5 Matching tiles
						return 5;
					} else {
						// 4 Matching tiles
						return 4;
					}
				} else {
					// 3 Matching tiles
					return 3;
				}
			} else if (indexR == 4) {//max index for 4 matches
				if (tiles [indexC, indexR].type == tiles [indexC, indexR + 3].type) {
					// 4 Matching tiles
					return 4;
				} else {
					// 3 Matching tiles
					return 3;
				}

			} else {//max index for 3 matches
				//3 Matching tiles;
				return 3;
			}
		} else {
			//Matching tiles less than 3
			return 0;
		}
	}
	IEnumerator slideTile(bool anyMoved){
		for (int r = 1; r < row; r++) {
			for (int c = 0; c < col; c++) {
				if (r != 7) {
					Debug.Log ("bukan7");
					yield return new WaitForSeconds (0.02f);
				}
				//pos with scale
				float x= c*scale/10f;
				float y = r * scale/10f;
				if (r == row - 1 && tiles [c, r] == null) 
					//if in the top and no tile
				{
					Vector3 tilePos = new Vector3 (x, y,0);
					for (int i = 0; i < tileBank.Count; i++) {
						GameObject o = tileBank [i];
						if (!o.activeSelf) {
							
							//start corountine disini
							//to be updated
							//StartCoroutine("slideTile");

							o.transform.position = new Vector3 (tilePos.x, tilePos.y,0);
							o.SetActive (true);
							tiles [c, r] = new Tile (o, o.name);
							i = tileBank.Count + 1;
							anyMoved = true;
							//yield return new WaitForSeconds (5f);
						}
					}
				}
				if (tiles [c, r] != null) {
					//drop down if space below is empty
					if (tiles [c, r - 1] == null) {
						tiles [c, r - 1] = tiles [c, r];
						tiles [c, r - 1].tileObject.transform.position =
							new Vector3 (x, y - scale/10f,0);
						tiles [c, r] = null;
						anyMoved = true;
						//yield return new WaitForSeconds (5f);
					}
				}
			}
		}
		endRenew = true;
	}
	void renewGrid(){
		bool anyMoved = false;
		endRenew = false;
		shuffleList ();
		StartCoroutine ("slideTile", anyMoved);
		//if (checkNull()) {
		//	Debug.Log ("invoke used"+checkNull());
		//	renewGrid ();
		//	anyMoved = false;
		//}
	}
	void renewGridv2(){
		for (int r = 0; r < row; r++) {
			for (int c = 0; c < col; c++) {
				if (tiles [c, r] == null) {
					
				}
			}
		}
	}
	void addPoint(int x){
		points += x;
		pointNum.text = points.ToString ();
	}
	void resetPoint(){
		points = 0;
		pointNum.text = points.ToString ();
	}
	void changeTurn(int x){
		//this method can be used to inisialize game and substract turn
		//input maxTurn if you're about to inisialize game
		//else, input manual number;
		currentTurn += x;
		turnNum.text = currentTurn.ToString ();
	}
	void printNull(){
		for (int r = 0; r < row; r++) {
			for (int c = 0; c < col; c++) {
				Debug.Log (tiles [c, r] == null);
			}
		}
	}
	bool checkNull(){
		for (int r = 0; r < row; r++) {
			for (int c = 0; c < col; c++) {
				if (tiles [c, r] == null) {
					return true;
				}
			}
		}
		return false;
	}
		
	public void toggleTileVisiblity(){
		if (!tileVisible) {
			for (int r = 0; r < row; r++) {
				for (int c = 0; c < col; c++) {
					tiles [c, r].tileObject.SetActive (true);
					tileVisible = true;
				}
			}
		} else {
			for (int r = 0; r < row; r++) {
				for (int c = 0; c < col; c++) {
					tiles [c, r].tileObject.SetActive (false);
					tileVisible = false;
				}
			}
		}
	}
	void swapButton(){
		Vector3 temp = hideShowB.transform.position;
		hideShowB.transform.position = continueB1.transform.position;
		continueB1.transform.position = temp;
	}
	public void pressedContinueButton(){
		resultNum.text = points.ToString ();
		Panels [0].gameObject.SetActive (false);
		Panels [1].gameObject.SetActive (false);
		Panels [2].gameObject.SetActive (true);
		continueB1.gameObject.SetActive (false);
		toggleTileVisiblity ();
	}
	public void endGame(){
		SceneManagerClass.changeScene (0);
	}
	IEnumerator swapTile(GameObject t1,GameObject t2,bool x){
		x = true;
		Vector3 temp = t1.transform.position;
		t1.transform.DOMove (t2.transform.position, 1f);
		t2.transform.DOMove (temp, 1f);
		yield return new WaitForSeconds (2f);
		x = false;
	}

}