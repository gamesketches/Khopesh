using UnityEngine;
using System.Collections;

public enum BulletType {Gator, Hippo, Crane};

public class BulletLogic : MonoBehaviour {

	BulletType type;
	int damage;
	float velocity;
	float lifetime;
	public float indirectCorrectionSpeed;
	delegate void BulletFunction();
	BulletFunction bulletFunction;
	private Vector2 travelVector;
	private Transform target;
	private float headingTime;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		lifetime -= Time.deltaTime;
		if(lifetime <= 0f) {
			Destroy(gameObject);
		}
		bulletFunction();
		transform.position += new Vector3(travelVector.x, travelVector.y) * Time.deltaTime;
	}

	public void Initialize(BulletType bulletType, int bulletDamage, float Velocity,
													float Lifetime, Sprite bulletSprite){
		type = bulletType;
		damage = bulletDamage;
		velocity = Velocity;
		lifetime = Lifetime;
		GetComponent<SpriteRenderer>().sprite = bulletSprite;
		foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")){
			if(player.transform != transform.parent) {
				target = player.transform;
				break;
			}
		}
		switch(type) {
			case BulletType.Crane:
				bulletFunction = IndirectLogic;
				headingTime = 0f;
				break;
			/*case BulletType.Gator:
			 * bulletFunction = sineWaveLogic;
			 * break;*/
			default:
				bulletFunction = StraightLogic;
				break;
		}
	}

	void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "player") {
			if(other.gameObject != gameObject.transform.parent) {
				Debug.Log("Hit opponent");
			}
		}
	}

	void IndirectLogic(){
		travelVector = Vector2.Lerp(new Vector2(velocity, 0), target.position - gameObject.transform.position, 
			headingTime);
		headingTime += indirectCorrectionSpeed / (indirectCorrectionSpeed * 60);
	}

	void StraightLogic(){
		travelVector = new Vector2(velocity, 0f);
	}

	void SlowShotLogic(){
		Debug.Log("Slow shot logic");
	}
}
