using UnityEngine;
using System.Collections;

public class Reticle : MonoBehaviour {
	public float jabDamage;
	public float spinDamage;
	public float jabCooldown;
	public float spinCooldown;

	public Color color;

	public bool melee;
	public bool spinning;

	private Rigidbody2D rb2D;

	void Awake() {
		rb2D = GetComponent<Rigidbody2D>();
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if(melee) {
			if(collider.gameObject.layer != gameObject.layer) {
				if(collider.gameObject.tag == "Player") {
					if(spinning) {
						collider.gameObject.GetComponent<PlayerStats>().health -= spinDamage;
						collider.gameObject.GetComponent<InputManager>().SetExponentCooldownTimer(spinCooldown);
					} else {
						collider.gameObject.GetComponent<PlayerStats>().health -= jabDamage;
						collider.gameObject.GetComponent<InputManager>().SetExponentCooldownTimer(jabCooldown);
                        Debug.Log("cooldown now");
					}
					string hitSparkSpritePath = string.Concat("sprites/hitSparks/hit", color == Color.blue ? "BR" : "RB");
					GameObject sparks = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/HitSparks"), transform.position, Quaternion.identity);
					sparks.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(hitSparkSpritePath);	
					return;
				} else if(collider.gameObject.tag == "Hippo" || collider.gameObject.tag == "Crane" || collider.gameObject.tag == "Gator") {
					string hitSparkSpritePath = string.Concat("sprites/hitSparks/hit", color == Color.blue ? "BR" : "RB");
					GameObject sparks = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/HitSparks"), transform.position, Quaternion.identity);
					sparks.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(hitSparkSpritePath);	
					Destroy(collider.gameObject);
				}
			}
		}
	}

	public Rigidbody2D GetRigidbody2D() {
		return rb2D;
	}
}
