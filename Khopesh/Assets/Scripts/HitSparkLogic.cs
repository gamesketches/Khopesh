using UnityEngine;
using System.Collections;

public class HitSparkLogic : MonoBehaviour {

	public Color sparkColor;
	public int lifeTimeFrames = 10;
	
	// Update is called once per frame
	void Update () {
		lifeTimeFrames--;
		if(lifeTimeFrames <= 0) {
			Destroy(gameObject);
		}
	}

	public void SetSparkColor(Color newColor) {
		GetComponent<SpriteRenderer>().color = newColor;
	}
}
