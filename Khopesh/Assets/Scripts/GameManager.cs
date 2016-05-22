using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
	SpriteRenderer background;
    Text pressStart;
    Text roundTimer;
    Text victoryText;

    SpriteRenderer[] HorusWinsIconsSR;
    SpriteRenderer[] SetWinsIconsSR;

    // Use this for initialization
    void Start () {
        GameObject[] HorusWinsIcons;
        GameObject[] SetWinsIcons;

        SetLifeBar = GameObject.FindGameObjectsWithTag("SetLifeBar");
        HorusLifeBar = GameObject.FindGameObjectsWithTag("HorusLifeBar");
        SetWinsIcons = GameObject.FindGameObjectsWithTag("SetWinsIcon");
        HorusWinsIcons = GameObject.FindGameObjectsWithTag("HorusWinsIcon");
        HorusWinsIconsSR = new SpriteRenderer[HorusWinsIcons.Length];
        SetWinsIconsSR = new SpriteRenderer[SetWinsIcons.Length];
        int j = 0;
        for (int i = 2; i >= 0; i--)
        {
            HorusWinsIconsSR[j] = HorusWinsIcons[i].GetComponent<SpriteRenderer>();
            SetWinsIconsSR[j] = SetWinsIcons[i].GetComponent<SpriteRenderer>();
            j++;
        }

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
		background = GameObject.FindGameObjectWithTag("Background").GetComponent<SpriteRenderer>();
        roundTimer = GameObject.FindGameObjectWithTag("RoundTimer").GetComponent<Text>();
        victoryText = GameObject.FindGameObjectWithTag("VictoryText").GetComponent<Text>();
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
			background.enabled = true;
            InitializeGameSettings();
        }
        else if (Input.GetButtonUp("ButtonD0") || Input.GetButtonUp("ButtonC0") || Input.GetButtonUp("ButtonB0"))
        {
            titleLogo.enabled = false;
            pressStart.enabled = false;
            infoScreen.enabled = true;
            currentUpdateFunction = InfoScreen;
        }
    }

    void InfoScreen()
    {
        if (Input.GetButtonUp("ButtonD0") || Input.GetButtonUp("ButtonC0") || Input.GetButtonUp("ButtonB0"))
        {
            infoScreen.enabled = false;
            titleLogo.enabled = true;
            pressStart.enabled = true;
            currentUpdateFunction = TitleScreen;
        }
    }

    IEnumerator DisplayVictoryText(int playerNum, int roundsWon)
    {
		if(playerNum == 5) {
			victoryText.text = "DRAW\nGAME";
		}
		else {
			if(playerNum == 4 || playerNum == 3) {
				victoryText.text = "DRAW\nGAME";
				victoryText.enabled = true;
				yield return new WaitForSeconds(1.5f);
				playerNum -= 2;
			}
	        victoryText.text = playerNum == 1 ? "<color=Blue>HORUS" : "<color=Red>SET";
			victoryText.text += roundsWon == 3 ? "\n IS \n   VICTORIOUS</color>" : "\n</color>WINS";
		}
		victoryText.enabled = true;
        yield return new WaitForSeconds(3.0f);
        victoryText.enabled = false;
        victoryText.text = "";
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
		if(Input.GetAxis("Reset") != 0f) {
			SceneManager.LoadScene(0);
		}
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
			if(player1Stats.health <= 0 && player2Stats.health <= 0 ||
				player1Stats.health == player2Stats.health) {
				switch(sceneName[sceneName.Length - 1]) {
					case 'o':
						StartCoroutine(DisplayVictoryText(5, 0));
						nextSceneCode = "t";
						break;
					case 't':
						StartCoroutine(DisplayVictoryText(5, 0));
						nextSceneCode = "t";
						break;
					case 's':
                        StartCoroutine(DisplayVictoryText(4, player2RoundWins));
						player2RoundWins++;
                        SetWinsIconsSR[player2RoundWins - 1].enabled = true;
                        audioOutro(1);
						nextSceneCode = "s";
						break;
					case 'h':
                        StartCoroutine(DisplayVictoryText(3,player1RoundWins));
                        player1RoundWins++;
                        HorusWinsIconsSR[player1RoundWins - 1].enabled = true;
                        audioOutro(0);
                        nextSceneCode = "h";
						break;
					}
			}
			else if(player1Stats.health <= 0 || player1Stats.health < player2Stats.health) {
                audioOutro(1);
                player2RoundWins++;
                StartCoroutine(DisplayVictoryText(2, player2RoundWins));
                SetWinsIconsSR[player2RoundWins - 1].enabled = true;
				nextSceneCode = "s";
			}
			else if (player2Stats.health <= 0 || player2Stats.health < player1Stats.health){
                audioOutro(0);
                player1RoundWins++;
                StartCoroutine(DisplayVictoryText(1, player1RoundWins));
                HorusWinsIconsSR[player1RoundWins - 1].enabled = true;
                nextSceneCode = "h";
			}
			if(sceneName == "intro") {
				sceneName = nextSceneCode;
			}
			else {
				sceneName = string.Concat(sceneName, nextSceneCode);
			}
			currentUpdateFunction = RoundEndUpdate;
			ClearBullets();
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
				background.enabled = true;
				roundTimer.enabled = false;
				foreach(SpriteRenderer renderer in HorusWinsIconsSR) {
					renderer.enabled = false;
				}
				foreach(SpriteRenderer renderer in SetWinsIconsSR) {
					renderer.enabled = false;
				}
				AudioSource backgroundMusic = Camera.main.GetComponent<AudioSource>();
				backgroundMusic.clip = Resources.Load<AudioClip>("audio/music/menu/LandOfTwoFields");
				backgroundMusic.Play();
				currentUpdateFunction = TitleScreen;
				return;
			}
			RoundReset();
		}
	}

	void UpdateLifeBars() {
		float player1HealthProportion, player2HealthProportion;
		if(player1Stats.health > 0) {
			player1HealthProportion = (player1Stats.health / player1Stats.maxHealth);
		}
		else {
			player1HealthProportion = 0;
		}
		if(player2Stats.health > 0) {
			player2HealthProportion = (player2Stats.health / player2Stats.maxHealth);
		}
		else {
			player2HealthProportion = 0;
		}
		SetLifeBar[0].transform.localScale = new Vector3(player1HealthProportion * 10f, 6, 1);
		SetLifeBar[1].transform.localScale = new Vector3(player1HealthProportion * 10f, 6, 1);
		SetLifeBar[2].transform.localScale = new Vector3(player1HealthProportion * 10.5062f, 6, 1);
		HorusLifeBar[0].transform.localScale = new Vector3(player2HealthProportion * 10f, 6, 1);
		HorusLifeBar[1].transform.localScale = new Vector3(player2HealthProportion * 10f, 6, 1);
		HorusLifeBar[2].transform.localScale = new Vector3(player2HealthProportion * 10.5062f, 6, 1);
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

	void ClearBullets() {
		foreach(GameObject bullet in GameObject.FindGameObjectsWithTag("Gator")) {
			Destroy(bullet);
		}
		foreach(GameObject bullet in GameObject.FindGameObjectsWithTag("Hippo")) {
			Destroy(bullet);
		}
		foreach(GameObject bullet in GameObject.FindGameObjectsWithTag("Crane")) {
			Destroy(bullet);
		}
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
			tempMovement.SetAnimator(Resources.Load<RuntimeAnimatorController>("sprites/HorusAnimation/HorusAnimation_0"));
			tempStats.number = 0;
			reticle.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Khopesh/khopeshHorus");
			player1Reticle = reticle.gameObject;
		} else if(color == Color.red) {
			temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/playerStillWhiteBlack");
			tempMovement.SetAnimator(Resources.Load<RuntimeAnimatorController>("sprites/SetAnimation/SetAnimation_0"));
			tempStats.number = 1;
			reticle.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/Khopesh/khopeshSet");
			player2Reticle = reticle.gameObject;
		}
		return temp;
	}
}
