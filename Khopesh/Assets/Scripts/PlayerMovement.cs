using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	public float speed;
	public float reticleRadius;
	public bool locked;

	public Reticle reticle;

	private float radians;
	private float degrees;

	private string horizontalAxis;
	private string verticalAxis;

	private Rigidbody2D rb2D;

	private PlayerStats playerStats;

	void Start() {
		playerStats = GetComponent<PlayerStats>();
		gameObject.layer = playerStats.number + 8;
		reticle.gameObject.layer = gameObject.layer;
		locked = false;
		rb2D = GetComponent<Rigidbody2D>();
		if(playerStats.number == 0) {
			radians = 0.0f;
		} else if(playerStats.number == 1) {
			radians = Mathf.PI;
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
					SetReticle();
				}
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
}
