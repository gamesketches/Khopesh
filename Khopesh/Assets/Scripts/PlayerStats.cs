using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour {
	public float health;
	public Color playerColor;
	public int shotCooldown;
	public int meleeCooldown;

	private int roundWins;

	public void IncrementRoundWins() {
		roundWins++;
	}

	public int GetRoundWins() {
		return roundWins;
	}
}
