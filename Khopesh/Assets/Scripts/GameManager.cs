using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	delegate void UpdateFunction();
	UpdateFunction currentUpdateFunction;
	GameObject player1;
	GameObject player2;
	int player1Wins, player2Wins;
	public float roundTime;
	public Vector3 player1Pos, player2Pos;
	public int startingHealth;
	string[] player1Controls, player2Controls;
	PlayerStats player1Stats, player2Stats;
	string sceneName;
	AudioClip[] dialogue;
	AudioSource dialoguePlayer;
	BulletDepot bullets;
	GameObject[] SetLifeBar;
	GameObject[] HorusLifeBar;


    SpriteRenderer TitleLogo;
    Text RoundTimer;

    // Use this for initialization
    void Start () {
		SetLifeBar = GameObject.FindGameObjectsWithTag("SetLifeBar");
		HorusLifeBar = GameObject.FindGameObjectsWithTag("HorusLifeBar");
		dialoguePlayer = GetComponent<AudioSource>();
		sceneName = "intro";
		loadAudio();
		bullets = new BulletDepot();
		bullets.Load();
		player1Controls = CreateControlScheme(0);
		player2Controls = CreateControlScheme(1);
        TitleLogo = GameObject.FindGameObjectWithTag("TitleLogo").GetComponent<SpriteRenderer>();
        RoundTimer = GameObject.FindGameObjectWithTag("RoundTimer").GetComponent<Text>();
        TitleLogo.enabled = true;
        TitleScreen();
    }

    void TitleScreen()
    {
        if (Input.GetButtonUp("ButtonA0"))
        {
            TitleLogo.enabled = true;
            StartRound();
        }
    }
    string[] CreateControlScheme(int playerNum) {
	// Use this for initialization
	void Start () {
		// Fill in the MenuUpdate function
		// then uncomment line 27 and delete line 28
		// currentUpdateFunction = MenuUpdate;
		InitializeGameSettings();
	}

	string[] CreateControlScheme(int playerNum) {
		string[] controlArray = new string[6];
		controlArray[0] = string.Concat("Horizontal", playerNum.ToString());
		controlArray[1] = string.Concat("Vertical", playerNum.ToString());
		controlArray[2] = string.Concat("ButtonA", playerNum.ToString());
		controlArray[3] = string.Concat("ButtonB", playerNum.ToString());
		controlArray[4] = string.Concat("ButtonC", playerNum.ToString());
		controlArray[5] = string.Concat("ButtonD", playerNum.ToString());
		return controlArray;
	}
	
	// Update is called once per frame
	void Update () {
		currentUpdateFunction();
	}

	void MenuUpdate() {
		// here you go christian
	}

	void InitializeGameSettings() {
		SetLifeBar = GameObject.FindGameObjectsWithTag("SetLifeBar");
		HorusLifeBar = GameObject.FindGameObjectsWithTag("HorusLifeBar");
		dialoguePlayer = GetComponent<AudioSource>();
		sceneName = "intro";
		loadAudio();
		bullets = new BulletDepot();
		bullets.Load();
		currentUpdateFunction = InGameUpdate;
		player1Controls = CreateControlScheme(0);
		player2Controls = CreateControlScheme(1);
		StartRound();
	}

	void InGameUpdate(){
		roundTime -= Time.deltaTime;
        RoundTimer.text = Mathf.RoundToInt(roundTime).ToString();

		string nextSceneCode = "T";
		UpdateLifeBars();

		if(player1Stats.health <= 0 || player2Stats.health <= 0 || roundTime <= 0) {
			LockPlayers();
			if(player1Stats.health <= 0 && player2Stats.health <= 0) {
				Debug.Log("we have a tie");
			}
			else if(player1Stats.health <= 0) {
				player2Stats.IncrementRoundWins();	
				nextSceneCode = "H";
			}
			else if (player2Stats.health <= 0){
				player1Stats.IncrementRoundWins();
				nextSceneCode = "S";
			}
			else {
				// TODO: give a win to whoever is in the lead
				nextSceneCode = "T";
			}
			if(sceneName == "intro") {
				sceneName = nextSceneCode;
			}
			else {
				sceneName = string.Concat(sceneName, nextSceneCode);
			}
			RoundReset();
		}
	}

	void UpdateLifeBars() {
		foreach(GameObject lifebar in SetLifeBar) {
			lifebar.transform.localScale = new Vector3((player1Stats.health / player1Stats.maxHealth) * 10f, 6, 1);
		}
		foreach(GameObject lifebar in HorusLifeBar) {
			lifebar.transform.localScale = new Vector3((player2Stats.health / player1Stats.maxHealth) * 10f, 6, 1);
		}
	}

	void StartRound() {
		StartCoroutine(audioIntro());
		player1 = CreatePlayer(player1Controls, Color.red, player1Pos);
		player2 = CreatePlayer(player2Controls, Color.blue, player2Pos);
		player1Stats = player1.GetComponent<PlayerStats>();
		player2Stats = player2.GetComponent<PlayerStats>();
        RoundTimer.enabled = true;
    }

	IEnumerator audioIntro() {
		for(int i = 0; i < dialogue.Length - 2; i++) {
			dialoguePlayer.clip = dialogue[i];
			dialoguePlayer.Play();
			while(dialoguePlayer.isPlaying) {
				yield return null;
			}
		}
	}

	void audioOutro(int playerNum) {
		dialoguePlayer.clip = dialogue[dialogue.Length - 2 + playerNum];
		dialoguePlayer.Play();
	}

	void LockPlayers() {
		player1.GetComponent<PlayerMovement>().locked = true;
		player2.GetComponent<PlayerMovement>().locked = true;
	}

	void RoundReset() {
		Destroy(player1);
		Destroy(player2);
		loadAudio();
		StartRound();
	}

	void loadAudio() {
		dialogue = Resources.LoadAll<AudioClip>(string.Concat("audio/", sceneName));
	}

	GameObject CreatePlayer(string[] controls, Color color, Vector3 position){
		GameObject temp = (GameObject)Instantiate(Resources.Load("prefabs/Player"), 
												position, Quaternion.identity);
		//SetControls(temp);
		//temp.GetComponent<Renderer>() = color;
		PlayerStats tempStats = temp.GetComponent<PlayerStats>();
		InputManager tempInputManager = temp.GetComponent<InputManager>();

		tempStats.health = startingHealth;
		tempStats.maxHealth = startingHealth;
		tempStats.playerColor = color;
		temp.GetComponent<PlayerMovement>().InitializeAxes(controls);

		tempInputManager.bullets = bullets;
		tempInputManager.InitializeControls(controls);

		if(color == Color.red) {
			temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/playerStillBlackWhite");
			tempStats.number = 0;
		} else if(color == Color.blue) {
			temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/playerStillWhiteBlack");
			tempStats.number = 1;
		}
		return temp;
	}
}
