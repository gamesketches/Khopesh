using UnityEngine;
using System.Collections;

public class SoundEffectObjectScript : MonoBehaviour {

	AudioSource source;
	bool started = false;
	// Use this for initialization
	void Awake () {
		source = gameObject.GetComponent<AudioSource>();
	}

	public void PlaySoundEffect(string path){
		source.clip = Resources.Load<AudioClip>(string.Concat("audio/soundEffects/", path));
		source.Play();
		started = true;
	}
	// Update is called once per frame
	void Update () {
		if(started && !source.isPlaying) {
			Destroy(gameObject);
		}
	}
}
