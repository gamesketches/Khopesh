using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {
	public int mashBufferSize;

	public float shotCooldownTime;
	public float exponentCooldownTime;
	public float meleePressWindow;
	public float jabTime;
	public float jabSpeed;
	public float spinTime;
	public float spinSpeed;
	public float spinRadius;
	public float fullBufferScale = 2f;

	public Reticle reticle;

	private GameObject bulletPrefab;

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
	private bool melee;

	private PlayerStats playerStats;
	private SpriteRenderer renderer;
	private PlayerMovement playerMovement;
	private AudioSource soundEffects;

	void Start() {
		soundEffects = GetComponent<AudioSource>();
		bulletPrefab = Resources.Load<GameObject>("prefabs/Bullet");
		playerStats = GetComponent<PlayerStats>();
		playerMovement = GetComponent<PlayerMovement>();
		renderer = GetComponent<SpriteRenderer>();
		mashBufferSize = 8;
		mashBuffer = new char[mashBufferSize];
		for(int i = 0; i < mashBufferSize; i++){
			mashBuffer.SetValue('*', i);
		}
	}

	void Update() {
		if(playerMovement.locked) {
			return;
		}
		char button = GetButtonPress();
        meleeCooldownTimer -= Time.deltaTime;

        if (button == 'D' && meleeCooldownTimer <= 0) {
			Melee();
		} else if(button != '0' && exponentCooldownTimer <= 0 && !melee) {
			shotCooldownTimer = shotCooldownTime;
            if (button != 'D') // threw everything in here to get this cooldown not to interfere with sword. works.
            {
                gameObject.transform.localScale = Vector3.Lerp(new Vector3(1f, 1f, 1f), new Vector3(fullBufferScale, fullBufferScale, fullBufferScale),(float)bufferIter / (float)mashBufferSize);
                mashBuffer.SetValue(button, bufferIter);
            
			    if(!mashing) {
				    mashing = true;
			    }
			    ExponentShot();
			    bufferIter++;
            }
            //playerMovement.PassBufferToReticle(bufferIter, mashBufferSize);
            if (bufferIter >= mashBufferSize) {
				Fire();
				GameObject temp = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/SoundEffectObject"), gameObject.transform.position, Quaternion.identity);
				temp.GetComponent<SoundEffectObjectScript>().PlaySoundEffect("bufferFull");
			}
		} else if(mashing && button == '0' && !melee ){
			shotCooldownTimer -= Time.deltaTime;

            if (shotCooldownTimer <= 0.0f) {
				Fire();
			}
		}

		if(exponentCooldownTimer > 0) { 
			exponentCooldownTimer -= Time.deltaTime;
			renderer.color = new Color(0.5f, 0.5f, 0.5f);
		}
		else {
			renderer.color = new Color(1f, 1f, 1f);
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
		switch(type) {
		case BulletType.Crane:
			soundEffects.clip = Resources.Load<AudioClip>("audio/bulletSounds/craneSound");
			break;
		case BulletType.Gator:
			soundEffects.clip = Resources.Load<AudioClip>("audio/bulletSounds/alligatorSound");
			break;
		case BulletType.Hippo:
			soundEffects.clip = Resources.Load<AudioClip>("audio/bulletSounds/hippoSound");
			break;
		}
		soundEffects.Play();
	}

	void Fire() {
		//playerMovement.ResetReticle();
		if(exponentCooldownTimer <= 0) {
			InputEqualsNumber();
            Debug.Log("this");
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
			BulletDepot.Bullet bullet = bullets.types.projectileTypes[(int)bulletTypes[i]].volleys[0].volley[0];
			bullet.angle = (int)bulletAngles[i];
			CreateBullet(bullet, bulletTypes[i]);
		}
	}

	public void CreateBullet(BulletDepot.Bullet bullet, BulletType type = BulletType.Gator) {
		int angle = bullet.angle + (int)playerMovement.CurrentShotAngle();
		BulletLogic bulletLogic = ((GameObject)Instantiate(bulletPrefab, reticle.transform.position, Quaternion.Euler(0, 0, angle))).GetComponent<BulletLogic>();
		bulletLogic.Initialize(type, bullet.damage, bullet.speed, bullet.size, 5, playerStats.playerColor, playerStats.number);
	}

	public void InitializeControls(string[] controls) {
		buttonA = controls[2];
		buttonB = controls[3];
		buttonC = controls[4];
		buttonD = controls[5];
	}

	void Melee() {
		if(melee && !playerMovement.locked) {
			StopCoroutine("MeleeWindow");
			StartCoroutine("Spin");
		} else {
			StartCoroutine("MeleeWindow");
		}
	}

	IEnumerator MeleeWindow() {
		melee = true;
		float windowTimer = meleePressWindow;
		while(windowTimer > 0.0f) {
			windowTimer -= Time.deltaTime;
			yield return 0;
		}
		StartCoroutine("Jab");
	}

	IEnumerator Jab() {
		playerMovement.GetRigidbody2D().velocity = Vector2.zero;
		playerMovement.locked = true;
		reticle.melee = true;
		reticle.GetRigidbody2D().velocity = new Vector2(Mathf.Cos(playerMovement.CurrentShotAngle() * Mathf.Deg2Rad), Mathf.Sin(playerMovement.CurrentShotAngle() * Mathf.Deg2Rad)) * jabSpeed;
		yield return new WaitForSeconds(jabTime);
		reticle.GetRigidbody2D().velocity = new Vector2(Mathf.Cos((playerMovement.CurrentShotAngle() + 180.0f) * Mathf.Deg2Rad), Mathf.Sin((playerMovement.CurrentShotAngle() + 180.0f) * Mathf.Deg2Rad)) * jabSpeed;
		yield return new WaitForSeconds(jabTime);
		reticle.GetRigidbody2D().velocity = Vector2.zero;
		playerMovement.SetReticle();
		melee = false;
		reticle.melee = false;
		playerMovement.locked = false;
    }

    IEnumerator Spin() {
		playerMovement.GetRigidbody2D().velocity = Vector2.zero;
		playerMovement.locked = true;
		reticle.melee = true;
		reticle.spinning = true;
		float angle = reticle.GetRigidbody2D().rotation;
		float spinTimer = spinTime;
		while(spinTimer > 0.0f) {
			spinTimer -= Time.deltaTime;
			angle += spinSpeed * Time.deltaTime;
			float radians = angle * Mathf.Deg2Rad;
			reticle.GetRigidbody2D().MovePosition((Vector2)transform.position + new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * spinRadius);
			reticle.GetRigidbody2D().MoveRotation(angle - 90.0f);
			yield return 0;
		}
		playerMovement.SetReticle();
		melee = false;
		reticle.melee = false;
		reticle.spinning = false;
		playerMovement.locked = false;
    }

    public void SetExponentCooldownTimer(float value) {
		exponentCooldownTimer = value;
	}
}
