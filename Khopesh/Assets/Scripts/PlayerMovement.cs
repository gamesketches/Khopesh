using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	public float speed;
	public float reticleRadius;

	public string horizontalAxis;
	public string verticalAxis;

	public GameObject reticle;

	private float radians;
	private float degrees;

	private Rigidbody2D rb2D;

	void Start() {
		rb2D = GetComponent<Rigidbody2D>();
	}

	void Update() {
		HandleMovement();
	}

	void HandleMovement() {
		rb2D.velocity = (new Vector2(Input.GetAxisRaw(horizontalAxis), Input.GetAxisRaw(verticalAxis))).normalized * speed;
		radians = Mathf.Atan2(rb2D.velocity.y, rb2D.velocity.x);
		degrees = radians * Mathf.Rad2Deg;
		//reticle.transform.localPosition = new Vector3(Mathf.Cos(radians) * reticleRadius, Mathf.Sin(radians) * reticleRadius, 0.0f);
		//reticle.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, degrees - 90.0f);
	}

	public float CurrentShotAngle() {
			return radians * Mathf.Rad2Deg;
	}
}
