using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {
	public int mashBufferSize;

	public float shotCooldownTime;
	public float exponentCooldownTime;
	public float fullBufferScale;

	public string buttonA;
	public string buttonB;
	public string buttonC;
	public string buttonD;

	public GameObject hippoBulletPrefab;
	public GameObject gatorBulletPrefab;
	public GameObject craneBulletPrefab;

	private int bufferIter;

	private float shotCooldownTimer;
	private float meleeCooldownTimer;
	private float exponentCooldownTimer;

	private char[] mashBuffer;

	private bool mashing;

	private PlayerStats playerStats;
	private PlayerMovement playerMovement;

	void Start() {
		playerStats = GetComponent<PlayerStats>();
		playerMovement = GetComponent<PlayerMovement>();
		mashBuffer = new char[mashBufferSize];
		for(int i = 0; i < mashBufferSize; i++){
			mashBuffer.SetValue('*', i);
		}
	}

	void Update() {
		char button = GetButtonPress();
		if(button == 'D' && meleeCooldownTimer <= 0) {
		} else if(button != '0' && shotCooldownTimer <= 0) {
			shotCooldownTimer = shotCooldownTime;
			gameObject.transform.localScale = Vector3.Lerp(new Vector3(1f, 1f, 1f), new Vector3(fullBufferScale, fullBufferScale, fullBufferScale),(float)bufferIter / (float)mashBufferSize);
			mashBuffer.SetValue(button, bufferIter);
			if(!mashing) {
				mashing = true;
			}
			ExponentShot();
			bufferIter++;
			//playerMovement.PassBufferToReticle(bufferIter, mashBufferSize);
			if(bufferIter >= mashBufferSize) {
				Fire();
			}
		} else if(mashing && button == '0'){
			shotCooldownTimer -= Time.deltaTime;
			if(shotCooldownTimer <= 0.0f) {
				Fire();
			}
		}

		if(exponentCooldownTimer > 0) { 
			exponentCooldownTimer--;
			//GetComponentInChildren<Renderer>().material.color = noShootingColor;
		}
	}

	char GetButtonPress() {
		if(Input.GetButtonDown(buttonA)) {
			return 'A';
		}
		else if(Input.GetButtonDown(buttonB)) {
			return 'B';
		}
		else if(Input.GetButtonDown(buttonC)) {
			return 'C';
		}
		else if(Input.GetButtonDown(buttonD)) {
			return 'D';
		}
		else {
			return '0';
		}
	}

	void ExponentShot() {
		float incrementAngle = 45.0f;
		//AudioClip clip = bulletShot1;
		for(int i = 0; i < mashBuffer.Length; i++) {
			BulletType type = BulletType.Gator;
			float speed = (float)bufferIter * 2;
			switch(mashBuffer[i]) {
			case 'A':
				type = BulletType.Gator;
				speed = 28.0f;
				break;
			case 'B':
				type = BulletType.Crane;
				speed = 40.0f;
				break;
			case 'C':
				speed = 5.0f;
				type = BulletType.Hippo;
				//clip = bulletShot2;
				break;
			case 'D':
				Debug.LogError("Melee button sent to projectile buffer");
				break;
			default:
				continue;
				break;
			}

			if(bufferIter < 2) {
				CreateBullet(0.0f, speed, type);
				//playAudio(clip);
				return;
			}
			else {
				float baseAngle = 0.0f;
				for(int k = 1; k < i; k++) {
					speed = speed > 1 ? speed -= 1 : 1;
					CreateBullet(baseAngle + incrementAngle, speed, type);
					//playAudio(clip);
					k++;
					CreateBullet(-(baseAngle + incrementAngle), speed, type);
					//playAudio(clip);
					baseAngle += incrementAngle;
				}
			}
			if(i >= 1) {
				incrementAngle /= i;
			}
		}
	}

	void Fire() {
		//playerMovement.ResetReticle();
		if(exponentCooldownTimer <= 0) {
			InputEqualsNumber();
		}
		for(int i = 0; i < mashBufferSize; i++){
			mashBuffer.SetValue('*', i);
		}
		// This will be the hardest part to get right
		int lockFrames = (bufferIter * (bufferIter + 1));
		if(bufferIter < mashBufferSize) {
			lockFrames = lockFrames / 2;
		}
		exponentCooldownTimer = lockFrames;
		bufferIter = 0;
		mashing = false;
		gameObject.transform.localScale = new Vector3(1, 1, 1);
	}

	public void InputEqualsNumber() {
		int bulletNumber = 0;
		List<char> meaningfulInput = new List<char>();
		for(int i = 0; i < mashBufferSize; i++) {
			if(mashBuffer[i] != '*') {
				bulletNumber++;
				meaningfulInput.Add(mashBuffer[i]);
			}
		}
		float angleDifference = 90.0f / mashBufferSize;
		List<float> bulletAngles = new List<float>();
		List<BulletType> bulletTypes = new List<BulletType>();
		BulletType theBulletType;
		if(meaningfulInput.Count <= 0) {
			return;
		}
		char bulletTypeChar = meaningfulInput[meaningfulInput.Count - 1];
		if(bulletTypeChar == 'A') {
			theBulletType = BulletType.Gator;
		} else if(bulletTypeChar == 'B') {
			theBulletType = BulletType.Crane;
		} else {
			theBulletType = BulletType.Hippo;
		} 	
		bulletAngles.Add(0.0f);
		bulletTypes.Insert(0, theBulletType);
		if(bulletNumber == mashBufferSize) {
			bulletAngles.Add(90.0f);
			bulletAngles.Add(-90.0f);
			bulletTypes.Insert(0, theBulletType);
			bulletTypes.Insert(0, theBulletType);
		}

		if(bulletNumber > 1) {
			for(int i = 0; i < bulletNumber - 1; i++) {
				bulletAngles.Add(angleDifference * (i + 1));
				bulletAngles.Add(-angleDifference * (i + 1));
				bulletTypes.Add(theBulletType);
				bulletTypes.Add(theBulletType);	
			}
		}
		for(int i = 0; i < bulletAngles.Count; i++) {
			//Speed value needs to be addressed
			CreateBullet(bulletAngles[i], Random.Range(15.0f, 25.0f), bulletTypes[i]);
		}
	}

	public void CreateBullet(float angle, float speed = 10.0f, BulletType type = BulletType.Gator) {
		GameObject bullet = null;
		Rigidbody2D bulletRB;
		angle += playerMovement.CurrentShotAngle();
		if(type == BulletType.Hippo) {
			bullet = ((GameObject)Instantiate (hippoBulletPrefab, transform.position, 
				Quaternion.Euler (0.0f, 0.0f, 0.0f)));
		} else if(type == BulletType.Gator) {
			bullet = ((GameObject)Instantiate (gatorBulletPrefab, transform.position, 
				Quaternion.Euler (0.0f, 0.0f, 0.0f)));
		} else if(type == BulletType.Crane) { 
			bullet = ((GameObject)Instantiate (craneBulletPrefab, transform.position, 
				Quaternion.Euler (0.0f, 0.0f, 0.0f)));
		}
		bulletRB = bullet.GetComponent<Rigidbody2D> ();

		bulletRB.rotation = angle - 90f;

		bulletRB.velocity = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * speed;

		/*OwnerScript script = bullet.GetComponent<OwnerScript>();
		Player player = gameObject.GetComponent<Player>();
		script.Initialize(type, gameObject, gameObject.GetComponent<PlayerHealth>().opponent, player.number); 
		BulletManager.Instance.AddBullet(bullet);*/
	}
}
