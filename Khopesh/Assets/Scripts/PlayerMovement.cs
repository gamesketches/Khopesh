using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	public float speed;
	public float reticleRadius;
	public bool locked;

	public Reticle reticle;

	private int direction = 8;

	private float radians;
	private float degrees;

	private string horizontalAxis;
	private string verticalAxis;

	private Rigidbody2D rb2D;

	private Animator anim;

	private PlayerStats playerStats;

	void Awake() {
		locked = false;
	}

	void Start() {
		playerStats = GetComponent<PlayerStats>();
		gameObject.layer = playerStats.number + 8;

		reticle.gameObject.layer = gameObject.layer;
		rb2D = GetComponent<Rigidbody2D>();
		if(playerStats.number == 0) {
			radians = 0.0f;
			anim.SetTrigger("Idle East");
		} else if(playerStats.number == 1) {
			radians = Mathf.PI;
			anim.SetTrigger("Idle West");
		}
		degrees = radians * Mathf.Rad2Deg;
		SetReticle();
	}

	void Update() {
		HandleMovement();
	}

	void HandleMovement() {
		if(!locked) {
			rb2D.velocity = (new Vector2(Input.GetAxisRaw(horizontalAxis), Input.GetAxisRaw(verticalAxis))).normalized * speed;
				if(rb2D.velocity.x != 0.0f || rb2D.velocity.y != 0.0f) {
					radians = Mathf.Atan2(rb2D.velocity.y, rb2D.velocity.x);
					degrees = radians * Mathf.Rad2Deg;
					if(degrees < 0.0f) {
						degrees += 360.0f;
					}

					if(((degrees >= 0.0f && degrees <= 22.5f) || (degrees > 337.5f && degrees <= 359.0f)) && direction != 0) {
						anim.SetTrigger("Walk East");
						direction = 0;
					} else if(degrees > 22.5f && degrees <= 67.5f && direction != 1) {
						anim.SetTrigger("Walk Northeast");
						direction = 1;
					} else if(degrees > 67.5f && degrees <= 112.5f && direction != 2) {
						anim.SetTrigger("Walk North");
						direction = 2;
					} else if(degrees > 112.5f && degrees <= 157.5f && direction != 3) {
						anim.SetTrigger("Walk Northwest");
						direction = 3;
					} else if(degrees > 157.5f && degrees <= 202.5f && direction != 4) {
						anim.SetTrigger("Walk West");
						direction = 4;
					} else if(degrees > 202.5f && degrees <= 247.5f && direction != 5) {
						anim.SetTrigger("Walk Southwest");
						direction = 5;
					} else if(degrees > 247.5f && degrees <= 292.5f && direction != 6) {
						anim.SetTrigger("Walk South");
						direction = 6;
					} else if(degrees > 292.5f && degrees <= 337.5f && direction != 7) {
						anim.SetTrigger("Walk Southeast");
						direction = 7;
					}
					SetReticle();
				} else {
					if(direction == 0) {
						anim.SetTrigger("Idle East");
					} else if(direction == 1) {
						anim.SetTrigger("Idle Northeast");
					} else if(direction == 2) {
						anim.SetTrigger("Idle North");
					} else if(direction == 3) {
						anim.SetTrigger("Idle Northwest");
					} else if(direction == 4) {
						anim.SetTrigger("Idle West");
					} else if(direction == 5) {
						anim.SetTrigger("Idle Southwest");
					} else if(direction == 6) {
						anim.SetTrigger("Idle South");
					} else if(direction == 7) {
						anim.SetTrigger("Idle Southeast");
					}
				}
			}
		else {
			rb2D.velocity = Vector2.zero;
		}
	}

	public float CurrentShotAngle() {
			return radians * Mathf.Rad2Deg;
	}

	public void InitializeAxes(string[] controls) {
		horizontalAxis = controls[0];
		verticalAxis = controls[1];
	}

	public void SetReticle() {
		reticle.GetRigidbody2D().MovePosition((Vector2)transform.position + new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * reticleRadius);
		reticle.GetRigidbody2D().MoveRotation(degrees - 90.0f);
	}

	public void SetAnimator(RuntimeAnimatorController value) {
		anim = GetComponent<Animator>();
		anim.runtimeAnimatorController = value;
	}

	public Rigidbody2D GetRigidbody2D() {
		return rb2D;
	}
}
