using UnityEngine;
using System.Collections;

public class audioSourceScript : MonoBehaviour {

	void Start () {
		AudioSource aud = gameObject.GetComponent<AudioSource> ();
		//aud.clip = Microphone.Start ("Built-in microphone", true, 60, 44000);
        aud.clip = Microphone.Start(Microphone.devices[1], true, 60, 44000);
        aud.Play ();
	}
	
	void Update () {
	}
}
