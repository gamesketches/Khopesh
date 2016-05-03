using UnityEngine;
using System.Collections;

public class Reticle : MonoBehaviour {
	public float damage;

	public Color color;

	public bool melee;

	private Rigidbody2D rb2D;

	void Awake() {
		rb2D = GetComponent<Rigidbody2D>();
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if(melee) {
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

	public Rigidbody2D GetRigidbody2D() {
		return rb2D;
	}
}
