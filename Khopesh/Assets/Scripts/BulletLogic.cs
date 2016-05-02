using UnityEngine;
using System.Collections;

public enum BulletType {Gator, Hippo, Crane};

public class BulletLogic : MonoBehaviour {

	public BulletType type;
	int damage;
	float velocity;
	float lifetime;
	public float indirectCorrectionSpeed = 5f;
	public float indirectHomingTime = 0.5f;
	delegate void BulletFunction();
	BulletFunction bulletFunction;
	private Vector2 travelVector;
	private Transform target;
	private Vector3 targetPosition;
	private float headingTime;
	private Sprite[] animation;
	private int animFrame;
	AudioSource audio;
	SpriteRenderer renderer;

	// Use this for initialization
	void Start () {
		renderer = GetComponent<SpriteRenderer>();
		audio = GetComponent<AudioSource>();
		audio.clip = Resources.Load<AudioClip>("audio/soundEffects/rpsBulletCancel");
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
													float Lifetime, Color bulletColor, int playerNum){
		type = bulletType;
		damage = bulletDamage;
		gameObject.tag = bulletType.ToString();
		velocity = 10.0f;//Velocity;
		Vector3 tempVector = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.z, Vector3.forward) * new Vector3(velocity, 0, 0);
		travelVector = new Vector2(tempVector.x, tempVector.y);
		lifetime = Lifetime;
		GetComponent<SpriteRenderer>().color = bulletColor;
		gameObject.layer = 8 + playerNum;
		switch(type) {
			case BulletType.Crane:
				bulletFunction = IndirectLogic;
			animation = Resources.LoadAll<Sprite>(string.Concat("sprites/craneAnimation", bulletColor == Color.blue ? "B" : "R"));
				headingTime = 0f;
				foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")){
					if(player.layer != gameObject.layer) {
						target = player.transform;
						}
				}
				break;
			case BulletType.Gator:
			animation = Resources.LoadAll<Sprite>(string.Concat("sprites/gatorAnimation", bulletColor == Color.blue ? "B" : "R"));
			// TODO: change this
			bulletFunction = StraightLogic;
			 /* bulletFunction = sineWaveLogic;
			 */ break;
			// Hippo situation
			default:
			animation = new Sprite[1] {Resources.Load<Sprite>(string.Concat("sprites/hippo", bulletColor == Color.blue ? "B" : "R"))};
				bulletFunction = StraightLogic;
				break;
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if(other.gameObject.tag == "Boundary") {
			Destroy(gameObject);
			return;
		}
		if(other.gameObject.layer != gameObject.layer) {
			if(other.gameObject.tag == "Player") {
					other.gameObject.GetComponent<PlayerStats>().health -= damage;
				string hitSparkSpritePath = string.Concat("sprites/hitSparks/hit", renderer.color == Color.blue ? "B" : "R", type.ToString());
					GameObject sparks = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/HitSparks"), transform.position, Quaternion.identity);
					sparks.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(hitSparkSpritePath);	
					Destroy(gameObject);
					return;
			}
			else if(other.gameObject.tag == "Reticle") {
				return;
			}
			BulletType opposingType = (BulletType)System.Enum.Parse(typeof(BulletType), other.gameObject.tag);

			if(opposingType == type){
				GameObject temp = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/SoundEffectObject"), gameObject.transform.position, Quaternion.identity);
				temp.GetComponent<SoundEffectObjectScript>().PlaySoundEffect("identicalBulletRPS");
				Destroy(other.gameObject);
				Destroy(gameObject);
			}
			else if((int)opposingType == System.Enum.GetValues(typeof(BulletType)).Length - 1 && (int)type == 0) {
				GameObject temp = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/SoundEffectObject"), gameObject.transform.position, Quaternion.identity);
				temp.GetComponent<SoundEffectObjectScript>().PlaySoundEffect("rpsBulletCancel");
				Destroy(other.gameObject);
				//audio.Play();
				
			}
			else {
				GameObject destroyedObject = opposingType > type ? gameObject : other.gameObject;
				Destroy(destroyedObject);
			}
		}
	}

	void IndirectLogic(){
		if(headingTime < indirectHomingTime) {
			targetPosition = target.position;
		}
		// Might be better to handle this shit as a rotation
		if(headingTime < 1f) {
		Vector3 startVector = Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.z, Vector3.forward) * new Vector3(velocity, 0, 0);
		Vector3 temp = Vector3.Lerp(startVector, targetPosition - gameObject.transform.position, 
			headingTime);
		travelVector.x = temp.x;
		travelVector.y = temp.y;
		headingTime += indirectCorrectionSpeed / (indirectCorrectionSpeed * 60);
		}

	}

	void StraightLogic(){
		//travelVector = new Vector2(velocity, 0f);
	}

	void SlowShotLogic(){
		Debug.Log("Slow shot logic");
	}
}
