using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
//using UnityEditor;
public class Tile{
	public GameObject tileObject;
	public string type;
	public Tile(GameObject tileO,string t){
		tileObject = tileO;
		type = t;
	}
}

public class CandyCrushGame : MonoBehaviour {
	//Tiles game object
	GameObject tile1=null,tile2=null;
	public GameObject []tile;
	List<GameObject> tileBank = new List<GameObject>();
	public GameObject tileContainer;//Contain all tiles clone in grid
	bool tileVisible;
	Tile[,] tiles= new Tile[col,row];

	//GameObject in Canvas
	public GameObject point;
	public GameObject turn;
	public GameObject[] Panels;// idx 0=point, idx 1=grid, idx 2=continue
	public GameObject title;
	Text pointNum;
	Text turnNum;
	public Text resultNum;
	public Button hideShowB,continueB1,endButton;
	public GameObject charaSprite;
	public GameObject dialougeBubble;
	public Text dialougeText;
	public GameObject startButton;//Button at the start of game
	public GameObject startSign;//Text at the start of game
	public Button helpButton;
	public GameObject tutorialPanel;
	public GameObject finishPanel;
	//Grid
	static int row =8;
	static int col=8;
	bool renewBoard=false;

	//Other Object
	List<AudioSource> audiosource=new List<AudioSource>();
	public AudioClip[] audioClip;
	Animator charaAnim;
	public GameObject spriteRay;
	public SceneManagerClassv2 sceneManager;
	//Other
	static float speed=100f;
	float scale=6.5f;//1-10, 10 is max
	bool tileChanged=false;				//Is tiled changed
	bool tileMoving=false;				//is tile moving
	bool dialougeChanging=false;		//is dialouge changing
	bool gameStarted=false;
	bool tileIsChecking=false;
	bool gameEnded=false;
	int points,currentTurn;				//Round points and current turn
	public int maxTurns;				//Max turn of round
	int[] posInt = new int[4];			//Tile position converted form float to int, used in update()
	string[][]dialouge=new string [4][];	//Chara's dialouge. [x][y]. x = type of expression, y String
	public string[]dialougeStart;		//dialouge[0]
	public string[]dialougeIdle;		//dialouge[1]
	public string[]dialougeGreat;		//dialouge[2]
	public string[]dialougeTryAgain;	//dialouge[3]
	Vector3 bubbleStartPos=new Vector3(-166,-395);
	Vector3 bubbleFinPos = new Vector3 (88, -514);
	string afterEndFile;
	int afterEndLineCounter;

	//HIghScore Panel
	public GameObject highScoreTable;
	public Text mainText;
	public Text[]Name;
	public Text[]PointText;
	public Button continueButton;
	public GameObject textInput;
	int[] pointTemp;
	string[] nameTemp;
	int changedRanking=-1;
	void Awake () {

		//insert dialouge arrays to double array dialouge
		dialouge[0]=dialougeStart;
		dialouge [1] = dialougeIdle;
		dialouge [2] = dialougeGreat;
		dialouge [3] = dialougeTryAgain;

	}
	void Start () {
		sceneManager.activeTint ();
		StartCoroutine (sceneManager.fadeFromBlack ());
		//Insert animation from chara sprite
		startButton.GetComponent<Button>().interactable=false;
		charaAnim=charaSprite.GetComponent<Animator>();
		changeDialouge (0, Random.Range(0,dialouge[0].Length));
		//Disable hide show button
		hideShowB.gameObject.SetActive(false);
		//get audiosources from eventSystem
		for (int i = 0; i < GetComponents<AudioSource>().Length; i++) {
			audiosource.Add(this.gameObject.GetComponents<AudioSource>()[i]);
			//Add audiosource to List
			//audiosource[0]= Sound Effect
			//audiosource[1]=BGM
		}
		if (PlayerPrefs.GetInt ("SFX",1) == 0) {
			audiosource[0].mute = true;
		} else {
			audiosource[0].mute = false;
		}
		if (PlayerPrefs.GetInt ("BGM", 1) == 0) {
			audiosource[1].mute = true;
		} else {
			audiosource[1].mute = false;
		}
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
				o.GetComponent<SpriteRenderer> ().enabled = false;
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
		Panels [0].SetActive (false);
		Panels [1].SetActive (false);
		if(PlayerPrefs.HasKey("dialougeFile")){
			afterEndFile= PlayerPrefs.GetString("dialougeFile");
			afterEndLineCounter= PlayerPrefs.GetInt("lineCounter");
			//PlayerPrefs.DeleteAll ();
		}
		StartCoroutine (firstCheck ());

		if (PlayerPrefsX.GetBool ("freeMode", true)&
			!PlayerPrefsX.GetBool ("demoMode", false)) {
			startButton.GetComponent<Button> ().interactable = true;
		} else {
			StartCoroutine (startTutorialShow (1f));
		}
	}
	void tileDisableRenderer(){
		for (int r = 0; r < row; r++) {
			for (int c = 0; c < col; c++) {
				tiles [c, r].tileObject.GetComponent<SpriteRenderer> ().enabled = false;
			}
		}
	}
	IEnumerator firstCheck(){
		tileDisableRenderer ();
		int timeOut = 20;
		checkGridv2 ();
		Debug.Log ("IE started");
		while (timeOut>=0) {
			Debug.Log ("Time out: " + timeOut);
			renewGrid ();
			yield return new WaitForSeconds(0.01f);
			checkGridv2 ();
			timeOut--;
		}
		toggleTileVisiblity ();
		yield return new WaitForSeconds (2f);


	}
	void enableTiles(){
		for (int i = 0; i < tileBank.Count; i++) {
			tileBank [i].GetComponent<SpriteRenderer> ().enabled = true;
		}
	}
	void shuffleTile(){
	}	
	IEnumerator startTutorialShow(float delay){
		yield return new WaitForSeconds (delay);
		tutorialOpen ();
	}
	void changeDialouge(int x,int y){
		dialougeBubble.SetActive (false);
		dialougeBubble.transform.DOMove (bubbleStartPos, 0.1f);
		dialougeBubble.transform.DOScale (0, 0.1f);
		dialougeBubble.SetActive (true);
		dialougeBubble.transform.DOScaleX (1.4f, 0.1f);
		dialougeBubble.transform.DOScaleY (2f, 0.1f);
		dialougeBubble.transform.DOMove (bubbleFinPos, 0.5f);
		dialougeText.GetComponent<Text> ().text = dialouge [x] [y];
	}
	void changeDialouge(string s){
		dialougeBubble.SetActive (false);
		dialougeBubble.transform.DOMove (bubbleStartPos, 0.1f);
		dialougeBubble.transform.DOScale (0, 0.1f);
		dialougeBubble.SetActive (true);
		dialougeBubble.transform.DOMove (bubbleFinPos, 0.5f);
		dialougeBubble.transform.DOScaleX (1.4f, 1f);
		dialougeBubble.transform.DOScaleY (2f, 1f);
		dialougeText.GetComponent<Text> ().text =s;
	}
	public void startGame(){//method for button
		StartCoroutine(startGameIE());

	}
	void playClip(int clipIndex){
		audiosource [0].clip = audioClip [clipIndex];
		audiosource [0].Play ();
	}
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
	// Update is called once per frame
	void Update () {
		if (gameStarted) {//Don't do anything until game is started
			//int point for player
			int pointsAdd;
			//check if first turn. If yes, reset point
			if (currentTurn == maxTurns) {
				resetPoint ();
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
			if (!tileMoving) {			//Check if tile is moving
				while (checkNull ()) {
					renewGrid ();
				}
				//if currentTurn=0, no checkGrid;
				if (currentTurn > 0) {
					if (Input.GetMouseButtonDown (0) && !tileMoving) {
						Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
						RaycastHit2D hit = Physics2D.GetRayIntersection (ray, 1000);
						//Debug.Log (hit.collider.gameObject.ToString());
						string spriteCollider; //contain spriteChanger object to avoid NullReferenceException
						try {
							spriteCollider = hit.collider.gameObject.ToString (); //contain spriteChanger object to avoid NullReferenceException

						} catch (System.Exception x) {
							spriteCollider = "null";
						}
						if (spriteCollider == "Image (UnityEngine.GameObject)") {//if  sprite is pressed
							StartCoroutine (spriteClicker ());
						} else if (hit) {
							tile1 = hit.collider.gameObject;
							Debug.Log (tile1.ToString ());
							Debug.Log(tile1.GetComponent<SpriteRenderer>().color.a);
							Debug.Log(tile1.GetComponent<SpriteRenderer>().sprite.ToString());
							tile1.transform.DOScale(1,0.5f);
						}
					} 
					//if finger up is detected after
					//an initial tile has been selected
					else if (Input.GetMouseButtonUp (0) && tile1 && !tileMoving) {
						Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
						RaycastHit2D hit = Physics2D.GetRayIntersection (ray, 1000);
						string spriteCollider; //contain spriteChanger object to avoid NullReferenceException
						try {
							spriteCollider = hit.collider.gameObject.ToString (); //contain spriteChanger object to avoid NullReferenceException

						} catch (System.Exception x) {
							spriteCollider = "null";
						}
						if (spriteCollider == "Image (UnityEngine.GameObject)") {

						}//if  sprite is pressed
						else if (hit) {
							tile2 = hit.collider.gameObject;
						}
						if (tile1 && tile2 && !tileMoving) {
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

							for (int i = 0; i < 4; i++) {
								posInt [i] = (int)Mathf.Round (posFloat [i]);
							}
							int horDis = Mathf.Abs (posInt [0] - posInt [2]);
							int verDis = Mathf.Abs (posInt [1] - posInt [3]);
							if (horDis >= 1 ^ verDis >= 1) {

								//Tile position switch normal
								//Vector3 tempPos = tile1.transform.position;
								//tile1.transform.position = tile2.transform.position;
								//tile2.transform.position = tempPos;

								//Tile Position switch DOTween
								//tileMoving=true;
								StartCoroutine (swapTile (tile1, tile2));
								//tileMoving = false;

								//Tile Data switched
								Tile tileTemp = tiles [posInt [0], posInt [1]];
								tiles [posInt [0], posInt [1]] = tiles [posInt [2], posInt [3]];
								tiles [posInt [2], posInt [3]] = tileTemp;



							} else {
								//temp1.x=scale/10f;
								//temp1.y=scale/10f;
								//tile1.transform.localScale = temp1;
								playClip (0);
								changeDialouge (3, Random.Range (0, dialouge [3].Length));
								tile1.transform.DOScale(0.65f,0.5f);
								tile1 = null;
								tile2 = null;
							}
						}
						if (tile1 && !tile2 & !tileMoving) {
							playClip (0);
							changeDialouge (3, Random.Range (0, dialouge [3].Length));
							tile1.transform.DOScale(0.65f,0.5f);
							tile1 = null;
							tile2 = null;

						}
					}
					//CheckGrid ();
					checkGridv2 ();
					//Game ends
					if (currentTurn == 0) {
						//things to do when game ended	


					}
				} else {
					//update does nothing when game ended
					//check one more time for the last move
					if (!checkGridv2 ()&& !continueB1.gameObject.activeSelf&& !gameEnded) {
						StartCoroutine (finalTurn ());
						gameEnded = true;
					}
				}
			} else {

			}
		} 
	}
	IEnumerator finalTurn(){
		yield return new WaitForSeconds (0.5f);
		StartCoroutine (tileOffEnd ());
		yield return new WaitForSeconds (3f);
		playClip (1);
		finishPanel.SetActive (true);
		yield return new WaitForSeconds (2f);
		finishPanel.SetActive (false);
		charaSprite.gameObject.SetActive (false);
		dialougeBubble.gameObject.SetActive (false);
		continueB1.gameObject.SetActive (true);
	}
	IEnumerator tileOffEnd(){
		for (int r = 0; r < row; r++) {
			for (int c = 0; c < col; c++) {
				tiles [c, r].tileObject.SetActive (false);
				yield return new WaitForSeconds (0.035f);
			}
		}
	
	}
	void CheckGrid(){
		//checkedTile = Tiles to be change
		bool[,,] checkedTile = new bool[col, row, 2];//3rd index to change col and row
		int[,] matchTilesCol = new int[col, row];
		int[,] matchTilesRow = new int[col, row];
		int addPoints = 0;
		bool renewBoard = false;//true if there is removed tiles
		for (int x = 0; x < 2; x++) {
			for (int i = 0; i < col; i++) {
				for (int j = 0; j < row; j++) {
					checkedTile [j, i, x] = false;
					//x=0 coloum, x=1 row
					if (x == 0) {
						matchTilesCol [j, i] = 0; //Is this necessary?-RF
					} else {
						matchTilesRow [j, i] = 0; //Is this necessary?-RF
					}
				}
			}
		}
		//check grid from bottom left to top right
		for (int x = 0; x < 2; x++) {
			for (int r = 0; r < row; r++) {
				for (int c = 0; c < col; c++) {
					if (checkedTile [c, r, x]) {//checked tile no need to be checked again
						continue;
					} else {
						if (x == 0) {
							matchTilesCol [c, r] = checkCol (c, r);
							//change all Col checkedTile
							for (int i = c; i < c + matchTilesCol [c, r]; i++) {
								checkedTile [i, r, 0] = true;
							}
						} else {
							matchTilesRow [c, r] = checkRow (c, r);
							//change all Row checkedTile
							for (int i = r; i < r + matchTilesRow [c, r]; i++) {
								checkedTile [c, i, 1] = true;
							}
						}
					}
				}
			}
		}
		//Remove checked tile with destroy Animation
		for (int r = 0; r < row; r++) {
			for (int c = 0; c < col; c++) {
				if (checkedTile[c,r,0]||checkedTile[c,r,1]) {
					//Animation when game is started
					if (gameStarted) {
						StartCoroutine (destroyTile (tiles [c, r]));
					}else {
						StartCoroutine (destroyTileBeforeStart (tiles [c, r]));
					}
				}
			}
		}
		//Remove checked tile normal
		for (int r = 0; r < row; r++) {
			for (int c = 0; c < col; c++) {
				if (checkedTile [c, r, 0]) {
					//	tiles [c, r].tileObject.SetActive (false);
					tiles [c, r] = null;
					Debug.Log ("tile nulled");
				}
				if (checkedTile [c, r, 1]) {
					//check if tiles are not nulled;
					if (tiles [c, r] != null) {
						//		tiles [c, r].tileObject.SetActive (false);
						tiles [c, r] = null;
						Debug.Log ("tile nulled");
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
				if (checkedTile [c, r, 0] || checkedTile [c, r, 1]) {
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
			//tileChanged = true;
			Debug.Log("entered renew board");
			renewGrid ();
			if ((currentTurn <= maxTurns)&&tile1 != null && tile2 != null) {		//Only play clip if not first turn
				playClip (1);
				//turn passed
				changeTurn (-1);
				int random = Random.Range (0, 100);
				if (random%2 == 1||random>25) {	//75% of dialouge changed after right match
					changeDialouge (2, Random.Range (0, dialouge [2].Length));
				}
				//Null the tile again
				tile1 = null;
				tile2 = null;
			}
		} else {
			if (tile1 != null & tile2 != null) {			//run coroutine if only tile1 and tile2 is not null
				Debug.Log("change is false");
				playClip(0);
				StartCoroutine (swapTile (tile1, tile2));
				tile1=null;
				tile2 = null;
				changeDialouge (3, Random.Range(0,dialouge[3].Length));
				//Tile Datas switch
				//change tile data to original position
				Tile tileTemp = tiles [posInt [0], posInt [1]];
				tiles [posInt [0], posInt [1]] = tiles [posInt [2], posInt [3]];
				tiles [posInt [2], posInt [3]] = tileTemp;
			}
		}
	}
	bool checkGridv2(){
		bool destroyed=false;
	//	Debug.Log ("check is called");
		if (!tileMoving) {	
	//		Debug.Log ("entered checking");
			//Done only when no tile animation is moving
			//checkedTile = Tiles to be change
			bool[,,] checkedTile = new bool[col, row, 2];//3rd index to change col and row
			int[,] matchTilesCol = new int[col, row];
			int[,] matchTilesRow = new int[col, row];
			int addPoints = 0;
			bool renewBoard = false;//true if there is removed tiles
			for (int x = 0; x < 2; x++) {
				for (int i = 0; i < col; i++) {
					for (int j = 0; j < row; j++) {
						checkedTile [j, i, x] = false;
						//x=0 coloum, x=1 row
						if (x == 0) {
							matchTilesCol [j, i] = 0; //Is this necessary?-RF
						} else {
							matchTilesRow [j, i] = 0; //Is this necessary?-RF
						}
					}
				}
			}
			//check grid from bottom left to top right
			for (int x = 0; x < 2; x++) {
				for (int r = 0; r < row; r++) {
					for (int c = 0; c < col; c++) {
						if (checkedTile [c, r, x]) {//checked tile no need to be checked again
							continue;
						} else {
							if (x == 0) {
								matchTilesCol [c, r] = checkCol (c, r);
								//change all Col checkedTile
								for (int i = c; i < c + matchTilesCol [c, r]; i++) {
									checkedTile [i, r, 0] = true;
								}
							} else {
								matchTilesRow [c, r] = checkRow (c, r);
								//change all Row checkedTile
								for (int i = r; i < r + matchTilesRow [c, r]; i++) {
									checkedTile [c, i, 1] = true;
								}
							}
						}
					}
				}
			}
			//Remove checked tile with destroy Animation
			for (int r = 0; r < row; r++) {
				for (int c = 0; c < col; c++) {
					if (checkedTile[c,r,0]||checkedTile[c,r,1]) {
							//Animation when game is started
						if (gameStarted) {
							StartCoroutine (destroyTile (tiles [c, r]));
							destroyed = true;
						}else {
							StartCoroutine (destroyTileBeforeStart (tiles [c, r]));
							destroyed = true;
						}
					}
				}
			}
			//Remove checked tile normal
			for (int r = 0; r < row; r++) {
				for (int c = 0; c < col; c++) {
					if (checkedTile [c, r, 0]) {
					//	tiles [c, r].tileObject.SetActive (false);
						tiles [c, r] = null;
						Debug.Log ("tile nulled");
					}
					if (checkedTile [c, r, 1]) {
						//check if tiles are not nulled;
						if (tiles [c, r] != null) {
					//		tiles [c, r].tileObject.SetActive (false);
							tiles [c, r] = null;
							Debug.Log ("tile nulled");
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
					if (checkedTile [c, r, 0] || checkedTile [c, r, 1]) {
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
				//tileChanged = true;
				Debug.Log("entered renew board");
					renewGrid ();//game has started

				if ((currentTurn <= maxTurns)&&tile1 != null && tile2 != null) {		//Only play clip if not first turn
					playClip (1);
					//turn passed
					changeTurn (-1);
					int random = Random.Range (0, 100);
					if (random%2 == 1||random>25) {	//75% of dialouge changed after right match
						changeDialouge (2, Random.Range (0, dialouge [2].Length));
					}
					//Null the tile again
					tile1 = null;
					tile2 = null;
				}
			} else {
				if (tile1 != null & tile2 != null) {			//run coroutine if only tile1 and tile2 is not null
					Debug.Log("change is false");
					playClip(0);
					StartCoroutine (swapTile (tile1, tile2));
					tile1=null;
					tile2 = null;
					changeDialouge (3, Random.Range(0,dialouge[3].Length));
					//Tile Datas switch
					//change tile data to original position
					Tile tileTemp = tiles [posInt [0], posInt [1]];
					tiles [posInt [0], posInt [1]] = tiles [posInt [2], posInt [3]];
					tiles [posInt [2], posInt [3]] = tileTemp;
				}
			}
			return destroyed;
		} else {
			//Don't do anything
			Debug.Log("is not moving");
			destroyed = true;
			return destroyed;
		}
	}
	void renewGrid(){
		if (!tileMoving||!gameStarted) {
			Debug.Log ("enter renewGrid");
			bool anyMoved = false;
			shuffleList ();
			for (int r = 1; r < row; r++) {
				for (int c = 0; c < col; c++) {
					//pos with scale
					float x = c * scale / 10f;
					float y = r * scale / 10f;
					if (r == row - 1 && tiles [c, r] == null) {					//if in the top and no tile
						Vector3 tilePos = new Vector3 (x, y, 0);
						for (int i = 0; i < tileBank.Count; i++) {
							GameObject o = tileBank [i];
							if (!o.activeSelf) {

								//start corountine disini
								//to be updated
								//StartCoroutine("slideTile");

								o.transform.position = new Vector3 (tilePos.x, tilePos.y, 0);
								o.SetActive (true);
								tiles [c, r] = new Tile (o, o.name);
								tiles [c, r].tileObject.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 1);
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
								new Vector3 (x, y - scale / 10f, 0);
							tiles [c, r] = null;
							anyMoved = true;
							//yield return new WaitForSeconds (5f);
						}
					}
				}
			}
			if (checkNull ()) {
				Debug.Log ("invoke used" + checkNull ());
				renewGrid ();
				anyMoved = false;
			}
		} else {
			Debug.Log ("did noting");
			//Don't do anything

		}
	}
	void renewGridv2(){
		//Used for first check only
		Debug.Log ("enter renewGridv2");
		bool anyMoved = false;
		shuffleList ();
		for (int r = 1; r < row; r++) {
			for (int c = 0; c < col; c++) {
				//pos with scale
				float x = c * scale / 10f;
				float y = r * scale / 10f;
				if (r == row - 1 && tiles [c, r] == null) {					//if in the top and no tile
					Vector3 tilePos = new Vector3 (x, y, 0);
					Debug.Log (x + " " + y);
					for (int i = 0; i < tileBank.Count; i++) {
						GameObject o = tileBank [i];
						if (!o.activeSelf) {

							//start corountine disini
							//to be updated
							//StartCoroutine("slideTile");
							Debug.Log("Add new tile from tileBank");
							o.transform.position = new Vector3 (tilePos.x, tilePos.y, 0);
							o.SetActive (true);
							tiles [c, r] = new Tile (o, o.name);
							tiles [c, r].tileObject.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 1);
							i = tileBank.Count + 1;
							anyMoved = true;
							tiles [c, r].tileObject.SetActive (false);
							//yield return new WaitForSeconds (5f);
						}
					}
				}
				if (tiles [c, r] != null) {
					//drop down if space below is empty
					if (tiles [c, r - 1] == null) {
						tiles [c, r - 1] = tiles [c, r];
						tiles [c, r - 1].tileObject.transform.position =
							new Vector3 (x, y - scale / 10f, 0);
						tiles [c, r] = null;
						anyMoved = true;
						//yield return new WaitForSeconds (5f);
					}
				}
			}
		}
		if (checkNull ()) {
			Debug.Log ("invoke used" + checkNull ());
			renewGridv2 ();
			anyMoved = false;
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
		hideShowB.gameObject.SetActive (true);
		Vector3 temp = hideShowB.transform.position;
		hideShowB.transform.position = continueB1.transform.position;
		continueB1.gameObject.SetActive (true);
		continueB1.transform.position = temp;
	}
	public void pressedContinueButton(){
		if(PlayerPrefsX.GetBool("demoMode")){
			PlayerPrefs.SetString ("dialougeFile", afterEndFile);
			PlayerPrefs.SetInt ("lineCounter", afterEndLineCounter);
			StartCoroutine (sceneManager.changeSceneWithLoadingEnum (2));
		}else{
		pointTemp = PlayerPrefsX.GetIntArray ("FWPoint");
		nameTemp = PlayerPrefsX.GetStringArray ("FWName");
		for (int i = 0; i < 5; i++) {
			PointText [i].text = pointTemp [i].ToString();
			Name [i].text = nameTemp [i];
		}
		resultNum.text = points.ToString ();
		Panels [0].gameObject.SetActive (false);
		Panels [1].gameObject.SetActive (false);
		Panels [2].gameObject.SetActive (true);
		continueB1.gameObject.SetActive (false);
		charaSprite.gameObject.SetActive (true);
		textInput.gameObject.SetActive (false);
		dialougeBubble.gameObject.SetActive (true);
		endButton.gameObject.SetActive (false);
		if (points < 1000) {
			changeDialouge ("Thank you for playing.");
		} else if (points < 1500) {
			changeDialouge ("You did amazing. Let's play again sometime.");
		} else{
			changeDialouge ("You managed to get a high score. Congratulation.");
		}
		//endButton.gameObject.SetActive (true);
		checkHighScore();
		toggleTileVisiblity ();
		}
	}
	void checkHighScore(){
		bool change = false;
		for (int i = 0; i < 5; i++) {
			if (points > pointTemp [i]) {
				change = true;
				changedRanking = i;
				//Name [i].text = "YOU";
				//nameTemp [i] = "YOU";
				//pointTemp [i] = points;

				for (int j = 4; j > i; j--) {
					nameTemp [j] = nameTemp [j - 1];
					pointTemp [j] = pointTemp [j - 1];
				}
				nameTemp [i] = "Very Generic Name";
				pointTemp [i] = points;
				PointText [i].text = points.ToString ();
				break;
			}
			change = false;
		}
		if (change) {
			textInput.gameObject.SetActive (true);
			mainText.gameObject.SetActive (true);
			endButton.gameObject.SetActive (false);
			textInput.SetActive (true);
			bool[] temp = PlayerPrefsX.GetBoolArray ("RewardArray");
			if (PlayerPrefsX.GetBool ("freeMode", true)) {
				temp [1] = true;
				PlayerPrefsX.SetBoolArray ("RewardArray", temp);
			} else {
			}
		} else {
			endButton.gameObject.SetActive (true);
			textInput.SetActive (false);
		}
	}
	public void nameInput(){
		resultNum.gameObject.SetActive (false);
		mainText.gameObject.SetActive (false);
		highScoreTable.SetActive (true);
		endButton.gameObject.SetActive (true);
		nameTemp [changedRanking] = textInput.GetComponent<InputField> ().text;
		Name [changedRanking].text = nameTemp [changedRanking];
		PlayerPrefsX.SetIntArray ("FWPoint", pointTemp);
		PlayerPrefsX.SetStringArray ("FWName", nameTemp);
		for (int i = 0; i < 5; i++) {
			PointText [i].text = pointTemp [i].ToString();
			Name [i].text = nameTemp [i];
		}
		textInput.gameObject.SetActive (false);
		endButton.gameObject.SetActive (true);
	
	}
	public void endGame(){
		endButton.interactable = false;
		if (PlayerPrefsX.GetBool ("freeMode", true)) {
			if(PlayerPrefsX.GetBool("MiyukiMissionStart")){
				bool[]temp=PlayerPrefsX.GetBoolArray("MiyukiChapter");
				temp[2]=true;
				PlayerPrefsX.SetBoolArray("MiyukiChapter",temp);
			}
		
		}

		if (!PlayerPrefsX.GetBool ("freeMode", false)|
			PlayerPrefsX.GetBool ("demoMode", false)) {
			PlayerPrefs.SetString ("dialougeFile", afterEndFile);
			PlayerPrefs.SetInt ("lineCounter", afterEndLineCounter);
			StartCoroutine (sceneManager.changeSceneWithLoadingEnum (2));
		} else {
			PlayerPrefsX.SetBool ("freeMode", true);
			StartCoroutine (sceneManager.changeSceneWithLoadingEnum (1));
		}

		//SceneManagerClass.changeScene (2);
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
	IEnumerator swapTile(GameObject t1,GameObject t2){
		float fadeSpeed = 0.5f;
		tileMoving = true;
		//Debug.Log (tileMoving);
		Vector3 temp = t1.transform.position;
		//Debug.Log (tileMoving);
		t1.transform.DOScale(1f,0.5f);
		t2.transform.DOScale(1f,0f);
		t1.transform.DOMove (t2.transform.position, fadeSpeed);
		//Debug.Log (tileMoving);
		t2.transform.DOMove (temp, fadeSpeed);
		//Debug.Log (tileMoving);
		yield return new WaitForSeconds (fadeSpeed);
		t1.transform.DOScale(0.65f,0.5f);
		t2.transform.DOScale(0.65f,0.5f);
		yield return new WaitForSeconds (fadeSpeed);
		//Debug.Log (tileMoving);
		tileMoving = false;
		//Debug.Log (tileMoving);
	}
	bool testDebug(){
		Debug.Log ("lol");
		return true;
	}
	IEnumerator destroyTile(Tile t){
		tileIsChecking = true;
		float fadeSpeed = 0.5f;
		playClip (1);
		tileMoving = true;
		t.tileObject.GetComponent<Animation> ().Play ("tileDestroy");
		//t.GetComponent<Animation> ().Play ("destroyTile");
		yield return new WaitForSeconds (1f);
		t.tileObject.SetActive (false);
		tileMoving = false;
	}
	IEnumerator destroyTileBeforeStart(Tile t){
		tileMoving = true;
		//t.GetComponent<Animation> ().Play ("destroyTile");
		yield return new WaitForSeconds (0.05f);
		t.tileObject.SetActive (false);
		tileMoving = false;
	}
	IEnumerator spriteClicker(){
		if (!dialougeChanging) {
			dialougeChanging = true;
			//	Debug.Log ("Sprite clicked");
			charaAnim.SetBool ("Clicked", true);
			int ran = Random.Range (0, dialouge [1].Length);
			changeDialouge (1, ran);
			yield return new WaitForSeconds (1f);
			charaAnim.SetBool ("Clicked", false);
			dialougeChanging = false;
		}
	}
	IEnumerator startGameIE(){
		startButton.GetComponent<Button> ().interactable = false;
		helpButton.interactable = false;
		startSign.gameObject.GetComponent<Animator> ().SetBool ("Clicked", true);
		playClip (1);
		enableTiles ();
		yield return new WaitForSeconds (1f);
		audiosource [1].Play ();
		startButton.SetActive (false);
		toggleTileVisiblity ();
		Panels [0].SetActive (true);
		Panels [1].SetActive (true);
		title.SetActive (false);
		helpButton.gameObject.SetActive (false);
		gameStarted = true;

	}
	public void tutorialOpen(){
		playClip (1);
		helpButton.gameObject.SetActive (false);
		tutorialPanel.SetActive (true);
		testDebug ();
		startButton.GetComponent<Button> ().interactable = false;
	}
	public void tutorialClose(){
		playClip (2);
		tutorialPanel.SetActive (false);
		helpButton.gameObject.SetActive (true);
		startButton.GetComponent<Button> ().interactable = true;
	}
}