﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	delegate void UpdateFunction();
	UpdateFunction currentUpdateFunction;
	GameObject player1;
	GameObject player2;
	int player1RoundWins, player1Wins, player2RoundWins, player2Wins;
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
		currentUpdateFunction = TitleScreen;
    }

    void TitleScreen()
    {
        if (Input.GetButtonUp("ButtonA0"))
        {
			AudioSource backgroundMusic = Camera.main.GetComponent<AudioSource>();
			backgroundMusic.clip = Resources.Load<AudioClip>("audio/music/battleTheme/RenewYourSoul");
			backgroundMusic.Play();
            TitleLogo.enabled = false;
			InitializeGameSettings();
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
				player2RoundWins++;
				audioOutro(1);
				nextSceneCode = "h";
			}
			else if (player2Stats.health <= 0){
				audioOutro(0);
				player1RoundWins++;
				nextSceneCode = "s";
			}
			else {
				// TODO: give a win to whoever is in the lead
				nextSceneCode = "t";
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
				Destroy(player1);
				Destroy(player2);
				TitleLogo.enabled = true;
				RoundTimer.enabled = false;
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

        RoundTimer = GameObject.FindGameObjectWithTag("RoundTimer").GetComponent<Text>();
        RoundTimer.enabled = true;
        StartCoroutine(audioIntro());
    }

	IEnumerator audioIntro() {
		LockPlayers();
		for(int i = 0; i < dialogue.Length - 2; i++) {
			dialoguePlayer.clip = dialogue[i];
			dialoguePlayer.Play();
			while(dialoguePlayer.isPlaying) {
				yield return null;
			}
		}
		UnlockPlayers();
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
		currentUpdateFunction = InGameUpdate;
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
		/*Rigidbody2D*/Reticle reticle = ((GameObject)Instantiate(Resources.Load("prefabs/Reticle"))).GetComponent<Reticle>();
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
		tempMovement.reticle = reticle.GetComponent<Rigidbody2D>();

		tempInputManager.bullets = bullets;
		tempInputManager.InitializeControls(controls);
		tempInputManager.reticle = reticle.GetComponent<Rigidbody2D>();

		if(color == Color.blue) {
			temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/playerStillBlackWhite");
			tempStats.number = 0;
		} else if(color == Color.red) {
			temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/playerStillWhiteBlack");
			tempStats.number = 1;
		}
		return temp;
	}
}
