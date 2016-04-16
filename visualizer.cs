using UnityEngine;
using System.Collections;

public class visualizer : MonoBehaviour {

	public int detail;
	public float minValue;
	
	public float randomAmplitude;
	private Vector3 startScale;

    public int tipo;

	void Start () {
		detail = 128;
		minValue = 1.0f;
		
		startScale = transform.localScale;
		randomAmplitude = Random.Range (0.1f, 0.9f);
	}

	void Update () {
		float[] info = new float[detail];

		AudioListener.GetOutputData (info, 0);

		float packagedData = 0.0f;

		for (int i = 0; i < info.Length; i++) {
			packagedData += Mathf.Abs(info[i]);
		}
        switch (tipo)
        {
            case 1: //esfera
                transform.localScale = new Vector3((packagedData * randomAmplitude) + startScale.x, startScale.y, (packagedData * randomAmplitude) + startScale.z);
                break;
            case 2: //tetra
                transform.localScale = new Vector3(startScale.x, (packagedData * randomAmplitude) + startScale.y, startScale.z);
                break;
        }
		
	}
}
