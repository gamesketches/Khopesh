using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	GameObject player1;
	GameObject player2;
	int player1Wins, player2Wins;
	public float roundTime;
	public Vector3 player1Pos, player2Pos;
	KeyCode[] player1Controls, player2Controls;
	// PlayerStats player1Stats, player2Stats;

	// Use this for initialization
	void Start () {
		//player1 = CreatePlayer(player1Controls, Color.red, player1Pos);
		//player2 = (GameObject)Instantiate(Resources.Load("Player"), player2Pos, Quaternion.identity);
		//player1Stats = player1.GetComponent<PlayerStats>();
		//player2Stats = player2.GetComponent<PlayerStats>();
	}
	
	// Update is called once per frame
	void Update () {
		roundTime -= Time.deltaTime;
		/* if(player1Stats <= 0 || player2Stats <= 0) {
		 * 		//LockPlayers();
				if(player1Stats <= 0 && player2Stats <= 0) {
					Debug.Log("we have a tie");
					}
				else if(player1Stats <= 0) {
					Debug.Log("player2 wins");		
				}
				else {
					Debug.Log("player1 wins");
				}
		*/
	}

	void StartRound() {
		
	}

	GameObject CreatePlayer(KeyCode[] controls, Color color, Vector3 position){
		//GameObject temp = (GameObject)Instantiate(Resources.Load("Player"), 
												//position, Quaternion.identity);
		//SetControls(temp);
		//temp.GetComponenet<Renderer>().color = color;
	}
}
