using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

public class FaceBulletCircle : MonoBehaviour {
	public int segmentNumber;

	public float radius;

	private VectorLine graphics;

	void Start() {
		graphics = new VectorLine("Face Bullet Circle", new List<Vector3>(segmentNumber * 2), null, 1.0f, LineType.Discrete, Joins.Weld);
		graphics.drawTransform = transform;
		graphics.MakeCircle(Vector3.zero, radius);
	}

	void Update() {
		graphics.Draw();
	}
}
