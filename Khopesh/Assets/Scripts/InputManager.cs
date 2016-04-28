using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {
	public int mashBufferSize;

	public float shotCooldownTime;
	public float exponentCooldownTime;
	public float fullBufferScale = 2f;

	private GameObject bulletPrefab;

	public Sprite hippoBulletSprite;
	public Sprite gatorBulletSprite;
	public Sprite craneBulletSprite;

	public BulletDepot bullets;

	private int bufferIter = 0;

	private float shotCooldownTimer;
	private float meleeCooldownTimer;
	private float exponentCooldownTimer;

	private string buttonA;
	private string buttonB;
	private string buttonC;
	private string buttonD;

	private char[] mashBuffer;

	private bool mashing;

	private PlayerStats playerStats;
	private PlayerMovement playerMovement;

	void Start() {
		bulletPrefab = Resources.Load<GameObject>("prefabs/Bullet");
		playerStats = GetComponent<PlayerStats>();
		playerMovement = GetComponent<PlayerMovement>();
		mashBufferSize = 8;
		mashBuffer = new char[mashBufferSize];
		for(int i = 0; i < mashBufferSize; i++){
			mashBuffer.SetValue('*', i);
		}
		ExponentShot();
	}

	void Update() {
		char button = GetButtonPress();
		if(button == 'D' && meleeCooldownTimer <= 0) {
		} else if(button != '0' && exponentCooldownTimer <= 0) {
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
			exponentCooldownTimer -= Time.deltaTime;
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
		BulletType type = BulletType.Gator;
		switch(GetButtonPress()) {
			case 'A':
				type = BulletType.Gator;
				break;
			case 'B':
				type = BulletType.Crane;
				break;
			case 'C':
				type = BulletType.Hippo;
				break;
			case 'D':
				return;
				break;
		}
		BulletDepot.Volley volley = bullets.types.projectileTypes[(int)type].volleys[bufferIter];
		foreach(BulletDepot.Bullet bullet in volley.volley) {
			CreateBullet(bullet, type);
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
		// TODO: CHANGE THIS TO SOMETHIGN REAL
		exponentCooldownTimer = lockFrames * Time.deltaTime;
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
			BulletDepot.Bullet bullet = new BulletDepot.Bullet();
			bullet.angle = (int)bulletAngles[i];
			CreateBullet(bullet, bulletTypes[i]);
		}
	}

	public void CreateBullet(BulletDepot.Bullet bullet, BulletType type = BulletType.Gator) {
		bullet.angle += (int)playerMovement.CurrentShotAngle();
		Sprite sprite = null;
		switch(type) {
			case BulletType.Hippo:
				sprite = hippoBulletSprite;
				break;

			case BulletType.Gator:
				sprite = gatorBulletSprite;
				break;

			case BulletType.Crane:
				sprite = craneBulletSprite;
				break;
		}
		BulletLogic bulletLogic = ((GameObject)Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, bullet.angle))).GetComponent<BulletLogic>();
		bulletLogic.Initialize(type, bullet.damage, bullet.speed, 5, playerStats.playerColor, gameObject);
	}

	public void InitializeControls(string[] controls) {
		buttonA = controls[2];
		buttonB = controls[3];
		buttonC = controls[4];
		buttonD = controls[5];
	}
}
