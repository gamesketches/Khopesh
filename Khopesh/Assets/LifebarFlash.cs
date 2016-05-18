using UnityEngine;
using System.Collections;

public class LifebarFlash : MonoBehaviour {

	private Vector3 scaleLastFrame;
	private SpriteRenderer renderer;
	private Color startColor;
	public Color flashColor;
	// Use this for initialization
	void Start () {
		scaleLastFrame = transform.lossyScale;
		renderer = GetComponent<SpriteRenderer>();
		startColor = renderer.color;
	}
	
	// Update is called once per frame
	void Update () {
		if(scaleLastFrame != transform.lossyScale) {
			renderer.color = flashColor;
		}
		else {
			renderer.color = startColor;
		}
		scaleLastFrame = transform.lossyScale;
	}
}
