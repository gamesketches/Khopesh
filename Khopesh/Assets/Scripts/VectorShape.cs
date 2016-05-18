using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

public class VectorShape : MonoBehaviour {
	public List<Vector3> graphicsPoints;

	private VectorLine graphics;

	void Start() {
		graphics = new VectorLine("Vector Shape", graphicsPoints, null, 1.0f, LineType.Discrete, Joins.Weld);
		graphics.drawTransform = transform;
	}

	void Update() {
		graphics.Draw();
	}
}
