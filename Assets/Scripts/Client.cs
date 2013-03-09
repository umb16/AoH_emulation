using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

public enum Statuses
{
	zero,
	newPlayerMenu,
	loadPlayerMenu1,
	loadPlayerMenu2,
	logining,
	play,
	waitSecond,
	waitAnim
};

public class Client : MonoBehaviour
{
	public BattleScene battleScene;
	public GUISkin guiSkin;
	public string IPAddress = "192.168.2.9";
	public int port = 25565;
	public string code = "";
	bool statsInPlay = false;
	int currentTurn = 1;
	string tempName;
	int numberDef;
	int turnCount = 1;
	bool player1Auto = false;
	bool player2Auto = false;
	
	public class Turn
	{	
		public bool[] def = new bool[4];
		public bool[] att = new bool[4];
		public Player player;
		
		public void Clear ()
		{
			for (int i=0; i<def.Length; i++) {
				def [i] = false;
				att [i] = false;
			}
		}
	}
	
	public class Player
	{
		
		public Player ()
		{
		}
		
		public Player ShallowCopy ()
		{
			return (Player)this.MemberwiseClone ();
		}
		
		public Player (string name)
		{
			Lvl = 1;
			Health = 1;
			Strenght = 1;
			Magic = 1;
			Luck = 1;
			this.name = name;
		}
		
		public int statpoints {
			get{ return Lvl + Lvl / 2 - Health - Strenght - Magic - Luck + 7;}
		}
		
		public int maxHP {
			get { 
				//return Lvl * 2+ 34 + Health * 4 ;
				return (int)(100 * Mathf.Pow (1.05f, Lvl - 1) + Mathf.Pow (1.05f, Lvl - 1) * ((float)Health-1));
			}
		}
		
		public float luckChance {
			get { 
				//if(Luck<120)
				//return (1 - 70 / (70 + (float)Luck)) * 0.5f+.04f;
				return (1 - 200 / (200 + (float)Luck));
				//return (1 - 100 / (100 + (float)Luck));
				//else
				//return (float) Luck/330;
			}
		}
		
		public float critChance {
			get { 
				//if(Luck<120)
				return (1 - 100 / (100 + (float)Luck)) * 1.4f;
				//else
				//return (float) Luck/330;
			}
		}
		
		public float fleeChance {
			get { 
				//if(Luck<120)
				return (1 - 100 / (100 + (float)Luck)) * .6f;
				//else
				//return (float) Luck/330;
			}
		}
		
		public int weaponDmg {
			get {
				if (weaponId < 0)
					weaponId = Weapons.weaponMass.Length - 1;
				if (weaponId >= Weapons.weaponMass.Length)
					weaponId = 0;
				return  Weapons.weaponMass [weaponId];
			}
		}
		
		public int helmetDef {
			get {
				if (helmetId < 0)
					helmetId = Weapons.helmetMass.Length - 1;
				if (helmetId >= Weapons.helmetMass.Length)
					helmetId = 0;
				return  Weapons.helmetMass [helmetId];
			}
		}
		
		public int armorDef {
			get {
				if (armorId < 0)
					armorId = Weapons.armorMass.Length - 1;
				if (armorId >= Weapons.armorMass.Length)
					armorId = 0;
				return  Weapons.armorMass [armorId];
			}
		}
		
		public int shieldDef {
			get {
				if (shieldId < 0)
					shieldId = Weapons.shieldMass.Length - 1;
				if (shieldId >= Weapons.shieldMass.Length)
					shieldId = 0;
				return  Weapons.shieldMass [shieldId];
			}
		}
		
		public float helmetDefMultiplier {
			get { return 100 / (100 + (float)helmetDef); }
		}
		
		public float armorDefMultiplier {
			get { return 100 / (100 + (float)armorDef); }
		}
		
		public float shieldDefMultiplier {
			get { return 25 / (100 + (float)(shieldDef)); }
		}
		
		public int totalDmg {
			get { 
				//return (int)(weaponDmg + Strenght/2 + weaponDmg * Strenght / 400); 
				return (int)(weaponDmg + weaponDmg * (float)Strenght / 100); 
			}
		}
		
		public int weaponId;
		public int helmetId;
		public int armorId;
		public int shieldId;
		
		public void SetToPositive ()
		{
			if (Lvl < 1)
				Lvl = 1;
			if (Health < 1)
				Health = 1;
			if (Strenght < 1)
				Strenght = 1;
			if (Magic < 1)
				Magic = 1;
			if (Luck < 1)
				Luck = 1;
		}
		
		public string name;
		public int Lvl;
		public int HP;
		public int Health;
		public int Strenght;
		public int Magic;
		public int Luck;
	}
	
	public class Players
	{
		public List<Player> list = new List<Player> ();
	}
	
	Players availablePlayers = new Players ();
	Player tempPlayer;
	Player player1;
	Player player2;
	public static Statuses status = Statuses.zero;
	int playerMenuFlag = -1;
	Vector2 scroll = new Vector2 (0, 0);
	Turn turnPlayer1;
	Turn turnPlayer2;
	Turn tempTurn = new Turn ();
	
	IEnumerator sendRequest (string request)
	{
		WWW www = new WWW (IPAddress + ":" + port.ToString () + "/" + request);
		yield return www;
		Debug.Log (www.text);
		Receiver (www.text);
	}
	
	void Receiver (string response)
	{
		if (response.Contains ("[code:")) {
			code = PushString.GetString ("code", response);
			status = Statuses.waitSecond;
		}
	}
	
	// Use this for initialization
	void Start ()
	{
		/* TcpClient Client = new TcpClient();
			try
            {
                Client.Connect(IPAddress, port);
            }
            catch
            {
                Debug.Log("Cannot connect to remote host!");
            }*/
		availablePlayers = (Players)Serialize.deserialization (availablePlayers, PlayerPrefs.GetString ("playerStore", Serialize.serialization (availablePlayers)));
		tempPlayer = new Player ("Player_" + availablePlayers.list.Count.ToString ());
	}
	
	void OnGUI ()
	{
		GUI.skin = guiSkin;
		if (status == Statuses.zero) {
			GUI_log.logString = "";
			if (GUI.Button (new Rect (10, 10, 150, 100), "Новый персонаж")) {
				status = Statuses.newPlayerMenu;
				tempPlayer = new Player ("Player_" + availablePlayers.list.Count.ToString ());
			}
			if (availablePlayers.list.Count > 0)
			if (GUI.Button (new Rect (160, 10, 150, 100), (player1 == null) ? "Выбрать player1" : player1.name + "\n (Reload)")) {
				status = Statuses.loadPlayerMenu1;
			}
			//player1Auto=GUI.Toggle (new Rect (200, 150, 150, 100),player1Auto,"Auto1");
			if (GUI.Button (new Rect (310, 10, 150, 100), (player2 == null) ? "Выбрать player2" : player2.name + "\n (Reload)")) {
				status = Statuses.loadPlayerMenu2;
			}
			player2Auto = GUI.Toggle (new Rect (350, 150, 150, 100), player2Auto, "Auto2");
			if (player1 != null && player2 != null)
			if (GUI.Button (new Rect (460, 10, 100, 100), "Play")) {
				status = Statuses.play;
				battleScene.gameObject.SetActive (true);
				turnCount = 1;
				player1.HP = player1.maxHP;
				player2.HP = player2.maxHP;
			}
		}
		if (status == Statuses.play) {
			if (GUI.Button (new Rect (70, 10, 60, 50), "Выход")) {
				battleScene.gameObject.SetActive (true);
				status = Statuses.zero;
				battleScene.gameObject.SetActive (false);
			}
			if (statsInPlay = GUI.Toggle (new Rect (10, 10, 60, 50), statsInPlay, "Хар-ки", "button")) {
				tempPlayer = player1;
				GUI.enabled = false;
				PlayerStats (10);
				tempPlayer = player2;
				GUI.enabled = false;
				PlayerStats (Screen.width / 2);
				GUI.enabled = true;
			} else {
				if (player1.HP > 0 && player2.HP > 0) {
					if (turnPlayer1 == null)
					if ((turnPlayer1 = BattleMenu ()) != null) {
						currentTurn = 2;
						numberDef = 0;
						turnPlayer1.player = player1;
						tempTurn = new Turn ();
					}
					if (turnPlayer1 != null && turnPlayer2 == null)
						
					if ((player2Auto) ? ((turnPlayer2 = RandomTurn ()) != null) : ((turnPlayer2 = BattleMenu ()) != null)) {
						currentTurn = 1;
						numberDef = 0;
						turnPlayer2.player = player2;
						tempTurn = new Turn ();
						Debug.Log ("Ход " + turnCount);
						turnCount++;
					}
					if (turnPlayer1 != null && turnPlayer2 != null) {
						status = Statuses.waitAnim;
						CalculateTurn (new Turn[]{turnPlayer1, turnPlayer2});
						turnPlayer1 = null;
						turnPlayer2 = null;
					}
				} else if (player1.HP < 1 && player2.HP < 1) {
					GUI.Label (new Rect (Screen.width / 2, Screen.height / 2, 400, 50), string.Format ("<size=40>НИЧЬЯ</size>"));
				} else if (player1.HP < 1) {
					GUI.Label (new Rect (Screen.width / 2, Screen.height / 2, 400, 50), string.Format ("<size=40>Player2 выйграл!</size>"));
				} else
					GUI.Label (new Rect (Screen.width / 2, Screen.height / 2, 300, 50), string.Format ("<size=40>Player1 выйграл!</size>"));
				GUI.Label (new Rect (0, Screen.height / 1.5f, 300, 50), string.Format ("<size=30> {0} {1}/{2}</size>", "Player1", player1.HP, player1.maxHP));
				GUI.Label (new Rect (Screen.width - 300, Screen.height / 1.5f, 300, 50), string.Format ("<size=30> {0} {1}/{2}</size>", "Player2", player2.HP, player2.maxHP));
				GUI.Label (new Rect (Screen.width / 2 - 150, 20, 300, 50), string.Format ("<size=40>Ходит Player {0}</size>", currentTurn));
			}
		}
		if (status == Statuses.newPlayerMenu)
			NewPlayerMenu ();
		else if (status == Statuses.loadPlayerMenu1) {
			if (LoadPlayerMenu () != null)
				player1 = tempPlayer.ShallowCopy ();
			//tempPlayer = new Player("Player_"+Random.value);
		} else if (status == Statuses.loadPlayerMenu2) {
			if (LoadPlayerMenu () != null)
				player2 = tempPlayer.ShallowCopy ();
		} else if (status == Statuses.logining) {
			GUI.Label (new Rect (Screen.width / 2 - 150, Screen.height / 2, 300, 30), "LOGINING");
		} else if (status == Statuses.waitSecond) {
			GUI.Label (new Rect (Screen.width / 2 - 150, Screen.height / 2, 300, 30), "WAIT SECOND PLAYER");
		}
	}
	
	Player LoadPlayerMenu ()
	{
		GUILayout.BeginArea (new Rect (0, 0, Screen.width / 3, Screen.height));
		scroll = GUILayout.BeginScrollView (new Vector2 (scroll.x, scroll.y), GUILayout.Width (Screen.width / 3), GUILayout.Height (Screen.height));
		for (int i = 0; i < availablePlayers.list.Count; i++) {
			if (playerMenuFlag == i)
				GUILayout.Button (availablePlayers.list [i].name, "box", GUILayout.Height (60));
			else if (GUILayout.Button (availablePlayers.list [i].name, GUILayout.Height (50))) {
				tempPlayer = availablePlayers.list [i];
				playerMenuFlag = i;
			}
		}
		
		GUILayout.EndScrollView ();
		GUILayout.EndArea ();
		
		if (playerMenuFlag >= 0) {
			PlayerStats (Screen.width / 3 + 20);
		
			if (GUI.Button (new Rect (Screen.width / 2 - 100, Screen.height / 5 - 40 + 510, 100, 40), "OK")) {
				//status = Statuses.logining;
				//StartCoroutine (sendRequest ("[login]"));
				PlayerPrefs.SetString ("playerStore", Serialize.serialization (availablePlayers));
				status = Statuses.zero;
				return tempPlayer;
			}
		}
		if (GUI.Button (new Rect (Screen.width / 2 + 50, Screen.height / 5 - 40 + 510, 100, 40), "Отмена")) {
			status = Statuses.zero;
		}
		return null;
	}
	
	void PlayerStats (int x)
	{
		GUI.Label (new Rect (x, Screen.height / 5 - 40 - 40, 50, 25), "Имя");
		tempPlayer.name = GUI.TextField (new Rect (x + 60, Screen.height / 5 - 40 - 40, 250, 25), tempPlayer.name, 50);
		
		GUI.Label (new Rect (x, Screen.height / 5 - 40 + 10, 50, 25), "Уровень");
		int.TryParse (GUI.TextField (new Rect (x + 50, Screen.height / 5 - 40 + 10, 30, 25), tempPlayer.Lvl.ToString (), 3), out tempPlayer.Lvl);
		tempPlayer.Lvl += PlusMinus (x + 80, Screen.height / 5 - 40 + 10);

		GUI.Label (new Rect (x + 190, Screen.height / 5 - 40 + 10, 300, 25), "Доступные статпоинты: " + tempPlayer.statpoints.ToString ());
		
		GUI.Label (new Rect (x, Screen.height / 5 - 40 + 60, 50, 25), "Здоровье");
		int.TryParse (GUI.TextField (new Rect (x + 50, Screen.height / 5 - 40 + 60, 30, 25), tempPlayer.Health.ToString (), 3), out tempPlayer.Health);
		tempPlayer.Health += PlusMinus (x + 80, Screen.height / 5 - 40 + 60);
		
		GUI.Label (new Rect (x + 190, Screen.height / 5 - 40 + 60, 300, 25), "Очки здоровья: " + tempPlayer.maxHP.ToString ());
				
		GUI.Label (new Rect (x, Screen.height / 5 - 40 + 110, 50, 25), "Сила");
		int.TryParse (GUI.TextField (new Rect (x + 50, Screen.height / 5 - 40 + 110, 30, 25), tempPlayer.Strenght.ToString (), 3), out tempPlayer.Strenght);
		tempPlayer.Strenght += PlusMinus (x + 80, Screen.height / 5 - 40 + 110);
		
		GUI.Label (new Rect (x + 190, Screen.height / 5 - 40 + 110, 300, 25), "Общий урон: " + tempPlayer.totalDmg.ToString ());
		
		/*GUI.Label (new Rect (x, Screen.height / 5 - 40 + 160, 50, 25), "Защита");
		int.TryParse (GUI.TextField (new Rect (x + 50, Screen.height / 5 - 40 + 160, 30, 25), tempPlayer.Defence.ToString (), 3), out tempPlayer.Defence);
		tempPlayer.Defence += PlusMinus (x + 80, Screen.height / 5 - 40 + 160);*/
		
		GUI.Label (new Rect (x, Screen.height / 5 - 40 + 160, 50, 25), "Магия");
		int.TryParse (GUI.TextField (new Rect (x + 50, Screen.height / 5 - 40 + 160, 30, 25), tempPlayer.Magic.ToString (), 3), out tempPlayer.Magic);
		tempPlayer.Magic += PlusMinus (x + 80, Screen.height / 5 - 40 + 160);
		
		GUI.Label (new Rect (x, Screen.height / 5 - 40 + 210, 50, 25), "Удача");
		int.TryParse (GUI.TextField (new Rect (x + 50, Screen.height / 5 - 40 + 210, 30, 25), tempPlayer.Luck.ToString (), 3), out tempPlayer.Luck);
		tempPlayer.Luck += PlusMinus (x + 80, Screen.height / 5 - 40 + 210);
		
		//GUI.Label (new Rect (x + 190, Screen.height / 5 - 40 + 260, 300, 25), "Шанс удачи: " + (tempPlayer.luckChance * 100).ToString ("#") + "%");
		GUI.Label (new Rect (x + 190, Screen.height / 5 - 40 + 210, 300, 25), "Шанс крита: " + (tempPlayer.critChance * 100).ToString ("#") + "%");
		GUI.Label (new Rect (x + 190, Screen.height / 5 - 40 + 250, 300, 25), "Шанс уворота: " + (tempPlayer.fleeChance * 100).ToString ("#") + "%");
		
		GUI.Label (new Rect (x, Screen.height / 5 - 40 + 310, 50, 25), "Оружие");
		int.TryParse (GUI.TextField (new Rect (x + 50, Screen.height / 5 - 40 + 310, 30, 25), tempPlayer.weaponId.ToString (), 3), out tempPlayer.weaponId);
		tempPlayer.weaponId += PlusMinus (x + 80, Screen.height / 5 - 40 + 310);
		
		GUI.Label (new Rect (x + 190, Screen.height / 5 - 40 + 310, 300, 25), "Урон оружия: " + tempPlayer.weaponDmg.ToString ());
		
		GUI.Label (new Rect (x, Screen.height / 5 - 40 + 360, 50, 25), "Шлем");
		int.TryParse (GUI.TextField (new Rect (x + 50, Screen.height / 5 - 40 + 360, 30, 25), tempPlayer.helmetId.ToString (), 3), out tempPlayer.helmetId);
		tempPlayer.helmetId += PlusMinus (x + 80, Screen.height / 5 - 40 + 360);
		
		GUI.Label (new Rect (x + 190, Screen.height / 5 - 40 + 360, 300, 25), "Защита шлема: " + tempPlayer.helmetDef.ToString ());
		
		GUI.Label (new Rect (x + 340, Screen.height / 5 - 40 + 360, 300, 25), "Общая защита шлема: " + ((1 - tempPlayer.helmetDefMultiplier) * 100).ToString ("#") + "%");
		
		GUI.Label (new Rect (x, Screen.height / 5 - 40 + 410, 50, 25), "Доспех");
		int.TryParse (GUI.TextField (new Rect (x + 50, Screen.height / 5 - 40 + 410, 30, 25), tempPlayer.armorId.ToString (), 3), out tempPlayer.armorId);
		tempPlayer.armorId += PlusMinus (x + 80, Screen.height / 5 - 40 + 410);
		
		GUI.Label (new Rect (x + 190, Screen.height / 5 - 40 + 410, 300, 25), "Защита доспеха: " + tempPlayer.armorDef.ToString ());
		
		GUI.Label (new Rect (x + 340, Screen.height / 5 - 40 + 410, 300, 25), "Общая защита доспеха: " + ((1 - tempPlayer.armorDefMultiplier) * 100).ToString ("#") + "%");
		
		GUI.Label (new Rect (x, Screen.height / 5 - 40 + 460, 50, 25), "Щит");
		int.TryParse (GUI.TextField (new Rect (x + 50, Screen.height / 5 - 40 + 460, 30, 25), tempPlayer.shieldId.ToString (), 3), out tempPlayer.shieldId);
		tempPlayer.shieldId += PlusMinus (x + 80, Screen.height / 5 - 40 + 460);
		
		GUI.Label (new Rect (x + 190, Screen.height / 5 - 40 + 460, 300, 25), "Защита щита: " + tempPlayer.shieldDef.ToString ());
		
		GUI.Label (new Rect (x + 340, Screen.height / 5 - 40 + 460, 300, 25), "Защита при ударе в голову: " + ((1 - tempPlayer.helmetDefMultiplier * tempPlayer.shieldDefMultiplier) * 100).ToString ("#") + "%");
		GUI.Label (new Rect (x + 340, Screen.height / 5 - 40 + 480, 300, 25), "Защита при ударе в тело: " + ((1 - tempPlayer.armorDefMultiplier * tempPlayer.shieldDefMultiplier) * 100).ToString ("#") + "%");
		
		tempPlayer.SetToPositive ();
	}
	
	void NewPlayerMenu ()
	{
		PlayerStats ((int)(Screen.width * .3f));
		
		if (GUI.Button (new Rect (Screen.width / 2 - 100, Screen.height / 5 - 40 + 510, 100, 40), "Ok")) {
			availablePlayers.list.Add (tempPlayer);
			tempPlayer = new Player ("Player_" + availablePlayers.list.Count.ToString ());
			PlayerPrefs.SetString ("playerStore", Serialize.serialization (availablePlayers));
			Debug.Log ("New player created!");
			status = Statuses.zero;
		}
		
		if (GUI.Button (new Rect (Screen.width / 2 + 50, Screen.height / 5 - 40 + 510, 100, 40), "Отмена")) {
			tempPlayer = new Player ("Player_" + availablePlayers.list.Count.ToString ());
			status = Statuses.zero;
		}
	}
	
	int PlusMinus (int x, int y)
	{
		if (GUI.Button (new Rect (x, y, 50, 35), "-")) {
			return -1;
		}
		if (GUI.Button (new Rect (x + 50, y, 50, 35), "+")) {
			return 1;
		}
		return 0;
	}
	
	public Turn RandomTurn ()
	{
		int attRandom = Random.Range (0, 4);
		int blockRandom1 = Random.Range (0, 4);
		int blockRandom2 = Random.Range (0, 4);
		if (blockRandom1 == blockRandom2) {
			blockRandom2++;
			blockRandom2 %= 4;
		}
		for (int i=0; i<tempTurn.def.Length; i++) {
			if (i == attRandom)
				tempTurn.att [i] = true;
			if (i == blockRandom1 || i == blockRandom2)
				tempTurn.def [i] = true;
		}
		return tempTurn;
	}
	
	public Turn BattleMenu ()
	{
		GUI.enabled = true;
		numberDef = 0;
		GUI.Label (new Rect (100 * GUI_FPS.screenK, 40 * GUI_FPS.screenK, 100 * GUI_FPS.screenK, 60 * GUI_FPS.screenK), "<size=" + GUI_FPS.fontSize + ">" + "Защита" + "</size>");
		GUI.Label (new Rect (Screen.width - 200 * GUI_FPS.screenK, 40 * GUI_FPS.screenK, 100 * GUI_FPS.screenK, 60 * GUI_FPS.screenK), "<size=" + GUI_FPS.fontSize + ">" + "Атака" + "</size>");
		
		for (int i=0; i<tempTurn.def.Length; i++) {
			if (tempTurn.def [i])
				numberDef++;
		}
		if (numberDef == 2 && (tempTurn.att [0] || tempTurn.att [1] || tempTurn.att [2] || tempTurn.att [3]))
		if (GUI.Button (new Rect (Screen.width / 2 - 50 * GUI_FPS.screenK, Screen.height - 60 * GUI_FPS.screenK, 100 * GUI_FPS.screenK, 60 * GUI_FPS.screenK), "<size=" + GUI_FPS.fontSize + ">" + "ok" + "</size>")) {
			return tempTurn;
		}
		for (int i=0; i<tempTurn.def.Length; i++) {
			if (i == 0)
				tempName = "Голова";
			else if (i == 1)
				tempName = "Грудь";
			else if (i == 2)
				tempName = "Пах";
			else if (i == 3)
				tempName = "Ноги";
			
			if (!tempTurn.def [i] && numberDef == 2)
				GUI.enabled = false;
			tempTurn.def [i] = GUI.Toggle (new Rect (100 * GUI_FPS.screenK, 70 * GUI_FPS.screenK + i * 70 * GUI_FPS.screenK, 100 * GUI_FPS.screenK, 60 * GUI_FPS.screenK), tempTurn.def [i], "<size=" + GUI_FPS.fontSize + ">" + tempName + "</size>", "button");
			GUI.enabled = true;
			if (!tempTurn.att [i] && (tempTurn.att [0] || tempTurn.att [1] || tempTurn.att [2] || tempTurn.att [3]))
				GUI.enabled = false;
			tempTurn.att [i] = GUI.Toggle (new Rect (Screen.width - 200 * GUI_FPS.screenK, 70 * GUI_FPS.screenK + i * 70 * GUI_FPS.screenK, 100 * GUI_FPS.screenK, 60 * GUI_FPS.screenK), tempTurn.att [i], "<size=" + GUI_FPS.fontSize + ">" + tempName + "</size>", "button");
			GUI.enabled = true;
			
		}
		return null;
		
	}

	void CalculateTurn (Turn[] turn)
	{
		BattleCommand[] command = new BattleCommand[2];
		bool crit;
		bool flee;
		string name1;
		string name2;
		int alterIndex;
		string block = "";
		string partOfBody = "";
		string log = "";
		int dmg = 1;
		for (int j = 0; j < turn.Length; j++) {
			alterIndex = Mathf.Abs (j - 1);
			name1 = "Player" + (j + 1);
			block = "";
			log = "";
			name2 = "Player" + (alterIndex + 1);
			crit = (Random.Range(0,100) < Mathf.Floor( turn [j].player.critChance*100)) ? true : false;
			flee = (Random.Range(0,100) < Mathf.Floor( turn [alterIndex].player.fleeChance*100)) ? true : false;
			log += name1 + " бъет в {0}{1}";
			if (flee) {
				log += "и промахивается";
				command [j] = BattleCommand.block;
			} else if (crit) {
				dmg = 2;
				log += " и наносит !{2} критического урона";
			} else {
				dmg = 1;
				log += " и наносит {2} урона";
			}
			if (!flee) {
				for (int i = 0; i < turn[j].att.Length; i++) {
					if (turn [j].att [i] == true) {	
						if (i == 0)
							partOfBody = "голову ";
						else if (i == 1) {
							partOfBody = "грудь ";
						} else if (i == 2) {
							partOfBody = "пах ";
						} else {
							partOfBody = "ноги ";
						}
						if (turn [alterIndex].def [i] == false) {
							block = "";
							command [j] = BattleCommand.bit;
							if (i == 0) {
								dmg *= (int)((turn [j].player.totalDmg + turn [j].player.totalDmg * Random.value * .1f) * turn [alterIndex].player.helmetDefMultiplier);
							} else {
								dmg *= (int)((turn [j].player.totalDmg + turn [j].player.totalDmg * Random.value * .1f) * turn [alterIndex].player.armorDefMultiplier);
							}
						} else {
							command [j] = BattleCommand.block;
							block = ", попадает в блок, ";
							if (i == 0) {
								dmg *= (int)((turn [j].player.totalDmg + turn [j].player.totalDmg * Random.value * .1f) * turn [alterIndex].player.helmetDefMultiplier * turn [alterIndex].player.shieldDefMultiplier);
							} else {
								dmg *= (int)((turn [j].player.totalDmg + turn [j].player.totalDmg * Random.value * .1f) * turn [alterIndex].player.armorDefMultiplier * turn [alterIndex].player.shieldDefMultiplier);
							}
						}
					}
				}
				turn [alterIndex].player.HP -= dmg;
			}
			
			Debug.Log (string.Format (log, partOfBody, block, dmg));
			if (turn [alterIndex].player.HP < 1) {
				command [j] = BattleCommand.die;
				Debug.Log (name2 + " умирает");
			}
		}
		battleScene.setCommand (command [0], command [1]);
	}
	
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
