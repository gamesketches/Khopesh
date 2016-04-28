using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	GameObject player1;
	GameObject player2;
	int player1Wins, player2Wins;
	public float roundTime;
	public Vector3 player1Pos, player2Pos;
	public int startingHealth;
	string[] player1Controls, player2Controls;
	PlayerStats player1Stats, player2Stats;

	BulletDepot bullets;

	// Use this for initialization
	void Start () {
		bullets = new BulletDepot();
		bullets.Load();
		StartRound();
		CreateControlScheme(0, out player1Controls);
		CreateControlScheme(1, out player2Controls);
	}

	void CreateControlScheme(int playerNum, out string[] controlArray) {
		controlArray = new string[6];
		controlArray[0] = string.Concat("Horizontal", playerNum.ToString());
		controlArray[1] = string.Concat("Vertical", playerNum.ToString());
		controlArray[2] = string.Concat("ButtonA", playerNum.ToString());
		controlArray[3] = string.Concat("ButtonB", playerNum.ToString());
		controlArray[4] = string.Concat("ButtonC", playerNum.ToString());
		controlArray[5] = string.Concat("ButtonD", playerNum.ToString());
	}
	
	// Update is called once per frame
	void Update () {
		roundTime -= Time.deltaTime;
		if(player1Stats.health <= 0 || player2Stats.health <= 0 || roundTime <= 0) {
		  		LockPlayers();
			if(player1Stats.health <= 0 && player2Stats.health <= 0) {
					Debug.Log("we have a tie");
					}
			else if(player1Stats.health <= 0) {
					player2Stats.IncrementRoundWins();	
				}
				else {
					player1Stats.IncrementRoundWins();
				}
			RoundReset();
		}
	}

	void StartRound() {
		player1 = CreatePlayer(player1Controls, Color.red, player1Pos);
		player2 = CreatePlayer(player2Controls, Color.blue, player2Pos);
		player1Stats = player1.GetComponent<PlayerStats>();
		player2Stats = player2.GetComponent<PlayerStats>();
		player1Stats.health = startingHealth;
		player2Stats.health = startingHealth;
	}

	void LockPlayers() {
		player1.GetComponent<PlayerMovement>().locked = true;
		player2.GetComponent<PlayerMovement>().locked = true;
	}

	void RoundReset() {
		Destroy(player1);
		Destroy(player2);
		StartRound();
	}

	GameObject CreatePlayer(string[] controls, Color color, Vector3 position){
		GameObject temp = (GameObject)Instantiate(Resources.Load("prefabs/Player"), 
												position, Quaternion.identity);
		//SetControls(temp);
		//temp.GetComponent<Renderer>() = color;
		temp.GetComponent<PlayerStats>().playerColor = color;
		temp.GetComponent<InputManager>().bullets = bullets;
		return temp;
	}
}
