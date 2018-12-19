using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Load : MonoBehaviour {


	static DirectoryInfo rootPath = new DirectoryInfo(@"db2\");

	public Time time;
	FileInfo LoadFile = rootPath.GetFiles("Load.res")[0];
	public GameObject LoadOb;
	Resourcex LoadRes;
	Texturefiles LoadTex;
	Maskfiles LoadMsk;
	public Canvas Canvas;
	GameObject back_main;
	GameObject single = null;
	GameObject ExitMenu = null;
	List<GameObject> objs = new List<GameObject>();
	public Shader TC;

	void Start()
	{




	}
	void Awake () {

		LoadOb.transform.SetParent(transform);
		LoadOb.AddComponent<Resourcex>().file = LoadFile;
		LoadRes = LoadOb.GetComponent<Resourcex>();
		LoadRes.StartRes();
		LoadTex = LoadOb.GetComponent<Texturefiles>();
		LoadMsk = LoadOb.GetComponent<Maskfiles>();
		LoadMenu();




//h\backHost.txr


	}
	void ChangeToGame()
	{
		SceneManager.LoadScene(1);
	}

	void LoadMenu()
	{

		back_main = LoadTexture("back_main",@"m\back_main.txr",true,gameObject);


		GameObject MainMenu = new GameObject("MainMenu");
		MainMenu.transform.SetParent(Canvas.transform);
		MainMenu.transform.localPosition = new Vector3(0f,0f,0f);


		LoadButton("record",@"m\recor",false,new Vector3(-258.58f,-213.7f,0),MainMenu);
		LoadButton("demo",@"m\demo",false,new Vector3(-310.4f,-94.2f,0),MainMenu);
		LoadButton("control",@"m\contr",false,new Vector3(-310.4f,36.4f,0),MainMenu);
		LoadButton("graph",@"m\graph",false,new Vector3(-258.58f,155.8f,0),MainMenu);

		LoadButton("exit",@"m\exit",false,new Vector3(258.58f,-213.7f,0),MainMenu).GetComponent<Button>().onClick.AddListener(LoadExitMenu);
		LoadButton("change_driver",@"m\chang",false,new Vector3(310.4f,-94.2f,0),MainMenu);
		LoadButton("multiplayer",@"m\multi",false,new Vector3(310.4f,36.4f,0),MainMenu);
		LoadButton("singleplayer",@"m\singl",false,new Vector3(258.58f,155.8f,0),MainMenu).GetComponent<Button>().onClick.AddListener(LoadSinglePlayerMenu);

		LoadTexture("menu_mask",@"common.msk",false,MainMenu);	
	}
	void LoadExitMenu()
	{
		if (!ExitMenu)
		{
			ExitMenu = new GameObject("ExitMenu");
			ExitMenu.transform.SetParent(Canvas.transform);
			ExitMenu.transform.localPosition = new Vector3(0f,0f,0f);
			objs.Add(ExitMenu);
			SetActive(ExitMenu);

			ChangeTexture(back_main,@"e\backExit.txr");

			GameObject go = LoadButton("Yes",@"e\ok",true,new Vector3(-72.7f,-29f,0),ExitMenu);
			go.GetComponent<Button>().onClick.AddListener(Application.Quit);
			LoadButton("Cancel",@"e\canc",true,new Vector3(44.38f,-29f,0),ExitMenu);

		}
		else
		{
			ChangeTexture(back_main,@"e\backExit.txr");
			SetActive(ExitMenu);
		}

	}

	void LoadSinglePlayerMenu()
	{
		if (!single)
		{
			single = new GameObject("SinglePlayerMenu");
			single.transform.SetParent(Canvas.transform);
			single.transform.localPosition = new Vector3(0f,0f,0f);
			objs.Add(single);
			SetActive(single);
		

			ChangeTexture(back_main,@"s\backSing.txr");

			GameObject go = LoadButton("newGame",@"s\new",true,new Vector3(0f,156.09f,0),single);
			go.GetComponent<Button>().onClick.AddListener(ChangeToGame);
			LoadButton("arcade",@"s\arcad",true,new Vector3(-4.2f,108.46f,0),single);
			LoadButton("simulator",@"s\simul",true,new Vector3(90.3f,108.46f,0),single);

			LoadButton("startTruck",@"s\metruck",true,new Vector3(90.3f,-19.48f,0),single);
			LoadButton("startCar",@"s\mecar",true,new Vector3(-4.2f,-19.48f,0),single);
		}
		else
		{
			ChangeTexture(back_main,@"s\backSing.txr");
			SetActive(single);
		}
		

		//back_main = LoadTexture("back_main",@"h\backHost.txr",true,Single);


	}
	void SetActive(GameObject go)
	{
		foreach(var ob in objs)
		{
			if (ob == go)
			{
				ob.SetActive(true);
			}
			else
			{
				ob.SetActive(false);
			}
		}
	}

	void ChangeTexture(GameObject gob,string txr)
	{
		Image img = gob.GetComponent<Image>();
		Texture2D msk = LoadTex.textures[FindIndexByString(txr,true)];
		Sprite sp = Sprite.Create(msk,new Rect(0,0,msk.width,msk.height),new Vector2(0,0)); 

		img.sprite = sp;
	}
	GameObject LoadTexture(string name,string txr,bool texture, GameObject root)
	{
		GameObject maskOb = new GameObject(name);
		maskOb.transform.SetParent(Canvas.transform);
		Image imag = maskOb.AddComponent<Image>();

		Texture2D msk;

		if(!texture)
		{
			msk = LoadMsk.textures[FindIndexByString(txr,texture)];
		}
		else
		{
			msk = LoadTex.textures[FindIndexByString(txr,texture)];
		}
		Sprite sp = Sprite.Create(msk,new Rect(0,0,msk.width,msk.height),new Vector2(0,0)); 

		imag.rectTransform.localPosition = new Vector3(0,0,0);
		imag.rectTransform.sizeDelta = new Vector2(msk.width,msk.height);
		imag.sprite = sp;
		imag.raycastTarget = false;
		if (!texture)
		{
			var ma = new Material(TC);
			ma.mainTexture = msk;
			ma.color = Color.black;
			ma.SetColor("_TransparentColor",Color.black);
			ma.SetFloat("_Cutoff",0.1f);
			imag.material = ma;
		}
		return maskOb;
	}

	GameObject LoadButton(string name,string txr,bool texture,Vector2 Position, GameObject root)
	{
		GameObject BtnOb = new GameObject(name);
		BtnOb.transform.SetParent(root.transform);
		Button graphBtn = BtnOb.AddComponent<Button>();
		graphBtn.transition = Selectable.Transition.SpriteSwap;


		Texture2D[] Grtx = new Texture2D[3];
		Image BtnIma = BtnOb.AddComponent<Image>();
		SpriteState spBtn = new SpriteState();


		string[] txrName = new string[3];



		txrName[0] = txr+"_ap";
		txrName[1] = txr+"_a";
		txrName[2] = txr+"_p";

		for (int i = 0; i<3;i++)
		{

		if(!texture)
		{
			txrName[i] = txrName[i]+".msk";
			Grtx[i] = LoadMsk.textures[FindIndexByString(txrName[i],texture)];
		}
		else
		{
			txrName[i] = txrName[i]+".txr";
			Grtx[i] = LoadTex.textures[FindIndexByString(txrName[i],texture)];
		}
		}

		spBtn.pressedSprite = Sprite.Create(Grtx[0],new Rect(0,0,Grtx[0].width,Grtx[0].height),new Vector2(0,0)); 
		spBtn.highlightedSprite = Sprite.Create(Grtx[1],new Rect(0,0,Grtx[1].width,Grtx[1].height),new Vector2(0,0)); 
		BtnIma.sprite = Sprite.Create(Grtx[2],new Rect(0,0,Grtx[2].width,Grtx[2].height),new Vector2(0,0)); 
		

		BtnIma.rectTransform.localPosition = Position; 
		BtnIma.rectTransform.sizeDelta = new Vector2(Grtx[0].width,Grtx[0].height);

		if (!texture)
		{
			var ma = new Material(TC);
			ma.mainTexture = Grtx[2];
			ma.color = Color.black;
			ma.SetColor("_TransparentColor",Color.black);
			ma.SetFloat("_Cutoff",0.1f);
			BtnIma.material = ma;
		}

		graphBtn.image = BtnIma;

		graphBtn.spriteState = spBtn;

		return BtnOb;
	}

	
	void Update () {

	}

	int FindIndexByString(string name,bool tex)
	{
		int pos = 0;
		List<string> texs = new List<string>();
		if (tex)
		{
			texs = LoadTex.Texturenames;
		}
		else
		{
			texs = LoadMsk.masks;
		}
		bool find = false;
		foreach(var a in texs)
		{

			if (a.Split(' ')[0] == name)
			{
				find = true;
				break;
			}
			pos++;
		}		
		if (find)
			return pos;
		else
		{
			Debug.LogError("Texture name: "+name+" not found");
			return 65565;
		}
	}
}
