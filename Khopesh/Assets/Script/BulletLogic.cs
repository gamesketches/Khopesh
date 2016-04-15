using UnityEngine;
using System.Collections;

public enum BulletType {Gator, Hippo, Cat};

public class BulletLogic : MonoBehaviour {

	BulletType type;
	int damage;
	float velocity;
	float lifetime;
	delegate void BulletFunction();
	BulletFunction bulletFunction;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		bulletFunction();
	}

	public void Initialize(BulletType bulletType, int damage, float velocity, float lifetime){
		type = bulletType;
		damage = damage;
		velocity = velocity;
		lifetime = lifetime;
		switch(bulletType) {
			case BulletType.Cat:
				bulletFunction = IndirectLogic;
				break;
			case BulletType.Gator:
				bulletFunction = StraightLogic;
				break;
			case BulletType.Hippo:
				bulletFunction = SlowShotLogic;
				break;
			default:
				bulletFunction = IndirectLogic;
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
		Debug.Log("Firing an indirect bullet");
	}

	void StraightLogic(){
		Debug.Log("Straight Shot logic");
	}

	void SlowShotLogic(){
		Debug.Log("Slow shot logic");
	}
}
