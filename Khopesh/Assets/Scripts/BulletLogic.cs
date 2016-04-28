using UnityEngine;
using System.Collections;

public enum BulletType {Gator, Hippo, Crane};

public class BulletLogic : MonoBehaviour {

	BulletType type;
	int damage;
	float velocity;
	float lifetime;
	public float indirectCorrectionSpeed = 5f;
	delegate void BulletFunction();
	BulletFunction bulletFunction;
	private Vector2 travelVector;
	private Transform target;
	private float headingTime;
	private Sprite[] animation;
	private int animFrame;
	SpriteRenderer renderer;

	// Use this for initialization
	void Start () {
		renderer = GetComponent<SpriteRenderer>();
		animFrame = 0;
	}
	
	// Update is called once per frame
	void Update () {
		lifetime -= Time.deltaTime;
		if(lifetime <= 0f) {
			Destroy(gameObject);
		}
		bulletFunction();
		gameObject.transform.position += new Vector3(travelVector.x, travelVector.y) * Time.deltaTime;
		renderer.sprite = animation[animFrame];
		animFrame = animFrame + 1 >= animation.Length ? 0 : animFrame + 1;
	}

	public void Initialize(BulletType bulletType, int bulletDamage, float Velocity,
													float Lifetime, Color bulletColor, GameObject myMother){
		type = bulletType;
		damage = bulletDamage;
		velocity = Velocity;
		Vector3 tempVector = Quaternion.AngleAxis(gameObject.transform.rotation.z * Mathf.Rad2Deg, Vector3.forward) * new Vector3(velocity, 0, 0);
		travelVector = new Vector2(tempVector.x, tempVector.y);
		lifetime = Lifetime;
		GetComponent<SpriteRenderer>().color = bulletColor;
		foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")){
			if(player != myMother) {
				target = player.transform;
				break;
			}
		}
		switch(type) {
			case BulletType.Crane:
				bulletFunction = IndirectLogic;
				animation = Resources.LoadAll<Sprite>("sprites/craneAnimation");
				headingTime = 0f;
				break;
			case BulletType.Gator:
			animation = Resources.LoadAll<Sprite>("sprites/gatorAnimation");
			// TODO: change this
			bulletFunction = StraightLogic;
			 /* bulletFunction = sineWaveLogic;
			 */ break;
			// Hippo situation
			default:
			animation = new Sprite[1] {Resources.Load<Sprite>("sprites/hippo")};
				bulletFunction = StraightLogic;
				break;
		}
	}

	void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "player") {
			if(other.gameObject != gameObject.transform.parent) {
				other.gameObject.GetComponent<PlayerStats>().health -= damage;
				Destroy(gameObject);
			}
		}
	}

	void IndirectLogic(){
		// Might be better to handle this shit as a rotation
		Vector3 startVector = Quaternion.AngleAxis(gameObject.transform.rotation.z * Mathf.Rad2Deg, Vector3.forward) * new Vector3(velocity, 0, 0);
		Vector3 temp = Vector3.Lerp(startVector, target.position - gameObject.transform.position, 
			headingTime);
		travelVector.x = temp.x;
		travelVector.y = temp.y;
		headingTime += indirectCorrectionSpeed / (indirectCorrectionSpeed * 60);
	}

	void StraightLogic(){
		//travelVector = new Vector2(velocity, 0f);
	}

	void SlowShotLogic(){
		Debug.Log("Slow shot logic");
	}
}
