using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	delegate void UpdateFunction();
	UpdateFunction currentUpdateFunction;
	GameObject player1;
	GameObject player2;
	GameObject player1Reticle;
	GameObject player2Reticle;
	int player1RoundWins, player1Wins, player2RoundWins, player2Wins;
	public float roundTime;
	private float currentRoundTime;
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


    SpriteRenderer titleLogo;
    SpriteRenderer infoScreen;
    Text pressStart;
    Text roundTimer;

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
		currentRoundTime = roundTime;
        titleLogo = GameObject.FindGameObjectWithTag("TitleLogo").GetComponent<SpriteRenderer>();
        pressStart = titleLogo.GetComponent<Text>();
        infoScreen = GameObject.FindGameObjectWithTag("InfoScreen").GetComponent<SpriteRenderer>();
        roundTimer = GameObject.FindGameObjectWithTag("RoundTimer").GetComponent<Text>();
        titleLogo.enabled = true;
        pressStart.enabled = true;
        currentUpdateFunction = TitleScreen;
    }

    void TitleScreen()
    {
        if (Input.GetButtonUp("ButtonA0"))
        {
            titleLogo.enabled = false;
            pressStart.enabled = false;
            InitializeGameSettings();
        }
        else if (Input.GetButtonUp("ButtonC0"))
        {
            titleLogo.enabled = false;
            pressStart.enabled = false;
            infoScreen.enabled = true;
            currentUpdateFunction = InfoScreen;
        }
    }

    void InfoScreen()
    {
        if (Input.GetButton("ButtonD0") && Input.GetButton("ButtonA0"))
        {
            infoScreen.enabled = false;
            titleLogo.enabled = true;
            pressStart.enabled = true;
            currentUpdateFunction = TitleScreen;
        }
    }

    /*	// Use this for initialization
        void Start () {
            // Fill in the MenuUpdate function
            // then uncomment line 27 and delete line 28
            // currentUpdateFunction = MenuUpdate;
            //InitializeGameSettings();
        }*/

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

	void InitializeGameSettings() {
		SetLifeBar = GameObject.FindGameObjectsWithTag("SetLifeBar");
		HorusLifeBar = GameObject.FindGameObjectsWithTag("HorusLifeBar");
		player1RoundWins = 0;
		player1Wins = 0;
		player2RoundWins = 0;
		player2Wins = 0;
		dialoguePlayer = GetComponent<AudioSource>();
		sceneName = "intro";
		loadAudio();
		bullets = new BulletDepot();
		bullets.Load();
		player1Controls = CreateControlScheme(0);
		player2Controls = CreateControlScheme(1);
		StartRound();
	}

	void InGameUpdate(){
		currentRoundTime -= Time.deltaTime;
        roundTimer.text = Mathf.RoundToInt(currentRoundTime).ToString();

		string nextSceneCode = "T";
		UpdateLifeBars();

		if(player1Stats.health <= 0 || player2Stats.health <= 0 || currentRoundTime <= 0) {
			LockPlayers();
			if(player1Stats.health <= 0 && player2Stats.health <= 0) {
				switch(sceneName[sceneName.Length - 1]) {
					case 'o':
						nextSceneCode = "t";
						break;
					case 't':
						nextSceneCode = "t";
						break;
					case 's':
						player2RoundWins++;
						audioOutro(1);
						nextSceneCode = "s";
						break;
					case 'h':
						player1RoundWins++;
						audioOutro(0);
						nextSceneCode = "h";
						break;
					}
			}
			else if(player1Stats.health <= 0) {
				player2RoundWins++;
				audioOutro(1);
				nextSceneCode = "s";
			}
			else if (player2Stats.health <= 0){
				audioOutro(0);
				player1RoundWins++;
				nextSceneCode = "h";
			}
			if(sceneName == "intro") {
				sceneName = nextSceneCode;
			}
			else {
				sceneName = string.Concat(sceneName, nextSceneCode);
			}
			currentUpdateFunction = RoundEndUpdate;
		}
	}

	void RoundEndUpdate() {
		if(!dialoguePlayer.isPlaying)
		{
			if(player1RoundWins > 2 || player2RoundWins > 2){
				Destroy(player1Reticle);
				Destroy(player2Reticle);
				Destroy(player1);
				Destroy(player2);
				titleLogo.enabled = true;
                pressStart.enabled = true;
				roundTimer.enabled = false;
				currentUpdateFunction = TitleScreen;
				return;
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
		player1 = CreatePlayer(player1Controls, Color.blue, player1Pos);
		player2 = CreatePlayer(player2Controls, Color.red, player2Pos);
		player1Stats = player1.GetComponent<PlayerStats>();
		player2Stats = player2.GetComponent<PlayerStats>();
		currentUpdateFunction = InGameUpdate;
		currentRoundTime = roundTime;	
        roundTimer = GameObject.FindGameObjectWithTag("RoundTimer").GetComponent<Text>();
        roundTimer.enabled = true;
        StartCoroutine(audioIntro());
    }

	IEnumerator audioIntro() {
		LockPlayers();
		Invoke("UnlockPlayers", 2.5f);
		AudioSource backgroundMusic = Camera.main.GetComponent<AudioSource>();
		backgroundMusic.clip = Resources.Load<AudioClip>("audio/music/battleTheme/RenewYourSoul");
		for(int i = 0; i < dialogue.Length - 2; i++) {
			dialoguePlayer.clip = dialogue[i];
			dialoguePlayer.Play();
			while(dialoguePlayer.isPlaying) {
				yield return null;
			}
		}
		backgroundMusic.Play();
	}

	void audioOutro(int playerNum) {
		dialoguePlayer.clip = dialogue[dialogue.Length - 2 + playerNum];
		dialoguePlayer.Play();
	}

	void LockPlayers() {
		player1.GetComponent<PlayerMovement>().locked = true;
		player2.GetComponent<PlayerMovement>().locked = true;
	}

	void UnlockPlayers() {
		player1.GetComponent<PlayerMovement>().locked = false;
		player2.GetComponent<PlayerMovement>().locked = false;
	}

	void RoundReset() {
		Destroy(player1Reticle);
		Destroy(player2Reticle);
		Destroy(player1);
		Destroy(player2);
		loadAudio();
		StartRound();
	}

	void loadAudio() {
		dialogue = Resources.LoadAll<AudioClip>(string.Concat("audio/dialogue/", sceneName));
	}

	GameObject CreatePlayer(string[] controls, Color color, Vector3 position){
		GameObject temp = (GameObject)Instantiate(Resources.Load("prefabs/Player"), 
												position, Quaternion.identity);
		Reticle reticle = ((GameObject)Instantiate(Resources.Load("prefabs/Reticle"))).GetComponent<Reticle>();
		//SetControls(temp);
		//temp.GetComponent<Renderer>() = color;
		PlayerStats tempStats = temp.GetComponent<PlayerStats>();
		PlayerMovement tempMovement = temp.GetComponent<PlayerMovement>();
		InputManager tempInputManager = temp.GetComponent<InputManager>();

		tempStats.health = startingHealth;
		tempStats.maxHealth = startingHealth;
		tempStats.playerColor = color;
		temp.GetComponent<PlayerMovement>().InitializeAxes(controls);

		reticle.color = color;
		tempMovement.reticle = reticle;

		tempInputManager.bullets = bullets;
		tempInputManager.InitializeControls(controls);
		tempInputManager.reticle = reticle;

		if(color == Color.blue) {
			temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/playerStillBlackWhite");
			tempStats.number = 0;
			reticle.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Khopesh/khopeshHorus");
			player1Reticle = reticle.gameObject;
		} else if(color == Color.red) {
			temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/playerStillWhiteBlack");
			tempStats.number = 1;
			reticle.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Khopesh/khopeshSet");
			player2Reticle = reticle.gameObject;
		}
		return temp;
	}
}
