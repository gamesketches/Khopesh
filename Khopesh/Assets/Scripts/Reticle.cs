using UnityEngine;
using System.Collections;

public class Reticle : MonoBehaviour {
	public float damage;

	public Color color;

	void OnTriggerEnter2D(Collider2D collider) {
		if(collider.gameObject.layer != gameObject.layer) {
			if(collider.gameObject.tag == "Player") {
				collider.gameObject.GetComponent<PlayerStats>().health -= damage;
				string hitSparkSpritePath = string.Concat("sprites/hitSparks/hit", color == Color.blue ? "BR" : "RB");
				GameObject sparks = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/HitSparks"), transform.position, Quaternion.identity);
				sparks.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(hitSparkSpritePath);	
				return;
			}
		}
	}
}
